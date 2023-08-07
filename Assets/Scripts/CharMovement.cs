using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SIDE {Left, Mid, Right}
public enum HitX {Left, Mid, Right, None}
public enum HitY {UP, Mid, Down, Low, None}
public enum HitZ {Forward, Mid, Backward, None}
public class CharMovement : MonoBehaviour
{
    private SIDE mSide = SIDE.Mid;
    [HideInInspector]
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    private Animator anim;
    private CharacterController charController;
    public GameObject gameOverScreen;
    private float x;
    private float speed = 5f;
    private float jumpForce = 35f;
    private float jumpVel;
    private bool inJump;
    private bool inSlide;
    private float colHeight;
    private float colCenterY;
    private SIDE lastSide;
    private bool stopAllStates = false;
    private Vector2 startPos;
    public float pixelDistToDetect = 50;
    private bool fingerDown;
    public float stumbleTolerance = 10f;
    private float stumbleTime;
    public bool canInput = true;
    private Collider collisionCol;
    void Start()
    {
        collisionCol = GetComponent<CapsuleCollider>();
        stumbleTime = stumbleTolerance;
        charController = GetComponent<CharacterController>();
        colHeight = charController.height;
        colCenterY = charController.center.y;
        anim = GetComponent<Animator>();
        transform.position = Vector3.zero;
        PlayAnimation("Running");
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalSwipe();
    }

    void HorizontalSwipe(){
        collisionCol.isTrigger = !canInput;
        if(!canInput){
            charController.Move(Vector3.down * 10f * Time.deltaTime);
            return;
        }
        if(fingerDown == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
            startPos = Input.touches[0].position;
            fingerDown = true;
        }
        else if(fingerDown == true && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended){
            fingerDown = false;
        }
        if(fingerDown){
            if(Input.touches[0].position.x < startPos.x + pixelDistToDetect && !inSlide)
            {
                PlayAnimation("StrafeLeft");
                if(fingerDown = true && mSide == SIDE.Mid)
                {
                    lastSide = mSide;
                    mSide = SIDE.Left;
                }
                else if(fingerDown = true && mSide == SIDE.Right)
                {
                    lastSide = mSide;
                    mSide = SIDE.Mid;
                }
                else if(fingerDown == true && mSide != lastSide){
                    lastSide = mSide;
                    PlayAnimation("");
                }
                    
            }
            else if(Input.touches[0].position.x > startPos.x + pixelDistToDetect && !inSlide)
            {
                PlayAnimation("StrafeRight");
                if(fingerDown == true && mSide == SIDE.Mid){
                    lastSide = mSide;
                    mSide = SIDE.Right;
                }
                else if(fingerDown == true && mSide == SIDE.Left){
                    lastSide = mSide;
                    mSide = SIDE.Mid;
                }  
                }
                else if(fingerDown == true && mSide != lastSide)
                {
                    lastSide = mSide;
                    PlayAnimation("");
                }
        }
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime>= 1){
            stopAllStates = false;
        }
        stumbleTime = Mathf.MoveTowards(stumbleTime, stumbleTolerance, Time.deltaTime);
        x = Mathf.Lerp(x, (int)mSide , Time.deltaTime * speed);
        charController.Move((x - transform.position.x) * Vector3.right);
        Jump();
        Roll();
    }
    
    private void ResetCollision(){
        hitX = HitX.None;
        hitY = HitY.None;
        hitZ = HitZ.None;
    }

    public IEnumerator DeathAnim(string animD){
        stopAllStates = true;
        anim.Play(animD);
        yield return new WaitForSeconds(0.2f);
        canInput = false;
    }

    private void PlayAnimation(string animName){
        if(stopAllStates) return;
        anim.Play(animName);
    }

    private void Stumble(string animS){
        anim.ForceStateNormalizedTime(0.0f);
        stopAllStates = true;
        anim.Play(animS);
        if(stumbleTime < stumbleTolerance /2f){
            StartCoroutine(DeathAnim("RockFall"));
            return;
        }
        stumbleTime = 6f;
        ResetCollision();
    }

    public void Jump(){
        if(fingerDown == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
            startPos = Input.touches[0].position;
            fingerDown = true;
        }
        if(fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended){
            fingerDown = false;
        }
        rollCounter -= Time.deltaTime;
        if(fingerDown){
            if(charController.isGrounded == true)
            {
                if(anim.GetCurrentAnimatorStateInfo(0).IsName("Falling")){
                    inJump = false;
                    PlayAnimation("Landing");
                }
                if(fingerDown == false && Input.touches[0].position.y > startPos.y + pixelDistToDetect && !inSlide)
                {
                    jumpVel = jumpForce;
                    inJump = true;
                    anim.CrossFadeInFixedTime("Jump", 0.1f);
                    fingerDown = true;
                }
            }
            else if(charController.isGrounded == false){
                jumpVel -= jumpForce * 2 * Time.deltaTime;
                PlayAnimation("Falling");
                if(charController.velocity.y < 0.1f){

                }
            }
        }
    }
    internal float rollCounter;
    public void Roll()
    {
        if(fingerDown == false && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began){
            startPos = Input.touches[0].position;
            fingerDown = true;
        }
        if(fingerDown && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended){
            fingerDown = false;
        }
        rollCounter -= Time.deltaTime;
        if(fingerDown){
            if(rollCounter < 0f){
                rollCounter = 0f;
                charController.center = new Vector3(0,colCenterY, 0);
                charController.height = colHeight;
                inSlide = false;
            }
            if(Input.touches[0].position.y < startPos.y + pixelDistToDetect && !inJump){
                rollCounter = 0.2f;
                jumpVel -= 10f;
                charController.center = new Vector3(0,colCenterY/2f, 0);
                charController.height = colHeight/2f;
                anim.CrossFadeInFixedTime("Slide", 0.1f);
                inSlide = true;
                inJump = false;
            }
        }
    }

    public void OnCharacterColliderHit(Collider col){
        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);
        if (hitZ == HitZ.Forward && hitX == HitX.Mid){
            if(hitY == HitY.Low){
                Stumble("");
            }
            else if(hitY == HitY.Down){
                StartCoroutine(DeathAnim("RockFall"));
            }
            else if(hitY == HitY.Mid){
                if(col.tag == "Jet"){
                    StartCoroutine(DeathAnim("RockFall"));
                }
                else if(col.tag != "Ramp")
                    StartCoroutine(DeathAnim("RockFall"));
            }
            else if(hitY == HitY.UP && !inSlide){
                StartCoroutine(DeathAnim("RockFall"));
            }
        }
        else if(hitZ == HitZ.Mid){
            if(hitX == HitX.Right){
                mSide = lastSide;
                Stumble("");
            }
            else if(hitX == HitX.Left){
                mSide = lastSide;
                Stumble("");
            }
        }
        else{
            if(hitX == HitX.Right){
                Stumble("");
            }
            else if(hitX == HitX.Left){
                Stumble("");
            }
        }
    }

    public HitX GetHitX(Collider col){
        Bounds char_bounds = charController.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col_bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Max(col_bounds.max.x, char_bounds.max.x);
        float average = (min_x + max_x) /2f -col_bounds.min.x;
        HitX hit;
        if(average > col_bounds.size.x - 0.33f)
            hit = HitX.Right;
        else if(average < 0.33f)
            hit = HitX.Left;
        else
            hit = HitX.Mid;
        return hit;
    } 

    public HitY GetHitY(Collider col){
        Bounds char_bounds = charController.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col_bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Max(col_bounds.max.y, char_bounds.max.y);
        float average = ((min_y + max_y) /2f - char_bounds.min.y) /char_bounds.size.y;
        HitY hit;
        if(average <0.17f)
            hit = HitY.Low;
        else if(average < 0.33f)
            hit = HitY.Down;
        else if(average < 0.66f)
            hit = HitY.Mid;
        else
            hit = HitY.UP;
        return hit;
    }

    public HitZ GetHitZ(Collider col){
        Bounds char_bounds = charController.bounds;
        Bounds col_bounds = col.bounds;
        float min_z = Mathf.Max(col_bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Max(col_bounds.max.z, char_bounds.max.z);
        float average = ((min_z + max_z) /2f - char_bounds.min.z) /char_bounds.size.z;
        HitZ hit;
        if(average < 0.33f)
            hit = HitZ.Backward;
        else if(average < 0.66f)
            hit = HitZ.Mid;
        else
            hit = HitZ.Forward;
        return hit;
    }

    public void Death(){
        StartCoroutine(DeathAnim());
    }
    IEnumerator DeathAnim()
    {
        PlayAnimation("Death");

        yield return new WaitForSeconds(1);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0;

        yield break;
    }
}
