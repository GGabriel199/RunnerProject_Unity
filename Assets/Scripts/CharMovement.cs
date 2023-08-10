using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public enum SIDE {Left, Mid, Right}

public class CharMovement : MonoBehaviour
{
    private SIDE mSide = SIDE.Mid;

    private Animator anim;
    private CharacterController charController;
    public GameObject gameOverScreen;
    private float speed = 20f;
    private float jumpSpeed = 15f;
    private float jumpForce = 35f;
    private float jumpVel;
    private float gravity = 0.5f;
    private float newXPos = 0f;
    private float x;
    public float xValue;
    private bool inJump;
    private bool inSlide;
    public bool canInput = true;
    private Collider collisionCol;
    internal float rollCounter;
    private float colHeight;
    private float colCenterY;
    private float m_startTime  = 0.0f;
    private Vector2 m_startPos = Vector2.zero;

    private bool m_swiping = false;
    private bool m_holding = false;
    private float m_minDist  = 30.0f;
    private float m_maxTime = 0.5f;
    void Start()
    {
        collisionCol = GetComponent<CapsuleCollider>();
        charController = GetComponent<CharacterController>();
        colHeight = charController.height;
        colCenterY = charController.center.y;
        anim = GetComponent<Animator>();
        transform.position = Vector3.zero;
        anim.Play("Running");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Vector3.forward * speed;
        charController.Move(direction * Time.deltaTime);

        if (Input.touchCount > 0){
            foreach (Touch touch in Input.touches) {
                switch (touch.phase) {
                case TouchPhase.Began :
                    m_swiping = true;
                    m_startTime = Time.time;
                    m_startPos = touch.position;
                    break;
                case TouchPhase.Canceled :
                    m_holding = false;
                    m_swiping = false;
                    break;
                case TouchPhase.Ended :
                    HorizontalSwipe(touch);
                    m_holding = false;
                    break;
                case TouchPhase.Moved :
                    HorizontalSwipe(touch);
                    break;
                case TouchPhase.Stationary :
                    HorizontalSwipe(touch);
                    break;
                }
            }
        }
    }

    void HorizontalSwipe(Touch touch){
        Debug.Log("isGrounded? " + charController.isGrounded);
        float totalTime = Time.time - m_startTime;
        float totalDist = (touch.position - m_startPos).magnitude;
        Vector3 move = Vector3.forward * jumpSpeed;
        if (m_holding || (m_swiping && totalTime < m_maxTime && totalDist > m_minDist)) {
            Vector2 direction = touch.position - m_startPos;
            Vector2 swipeType = Vector2.zero;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                // Horizontal:
                swipeType = Vector2.right * Mathf.Sign(direction.x);
            }
            else{
                // Vertical:
                swipeType = Vector2.up * Mathf.Sign(direction.y);
            }

            if(swipeType.x != 0.0f){
                 m_holding = true;
                 if(swipeType.x < 0.0f){
                    // HANDLE LEFT SWIPE
                    anim.Play("StrafeLeft");
                    if (mSide == SIDE.Mid)
                    {
                        newXPos = -xValue;
                        mSide = SIDE.Left;
                    }
                    else if (mSide == SIDE.Right)
                    {
                        newXPos = 0f;
                        mSide = SIDE.Mid;
                    }
                 }
                 else if (!inSlide)
                 {
                    // HANDLE RIGHT SWIPE
                    anim.Play("StrafeRight");
                    if (mSide == SIDE.Mid)
                    {
                        newXPos = xValue;
                        mSide = SIDE.Right;
                    }
                    else if (mSide == SIDE.Left)
                    {
                        newXPos = 0f;
                        mSide = SIDE.Mid;
                    }
                 }
            }

            rollCounter -= Time.deltaTime;
            if(rollCounter <= 0f)
            {
                rollCounter = 0f;
                charController.center = new Vector3(0, colCenterY, 0);
                charController.height = colHeight;
                inSlide = false;
            }
            if(swipeType.y != 0.0f ){
                m_swiping = false;  // <- THIS MAKES THE DIFFERENCE
                if (charController.isGrounded)
                {
                    if (swipeType.y > 0.0f && !inSlide)
                    {
                        // HANDLE UP SWIPE
                        jumpVel = jumpForce;
                        inJump = true;
                        inSlide = false;
                        anim.Play("Jump");
                    }
                    if(swipeType.y <0.0f && !inJump)
                    {
                        //HANDLE DOWN SWIPE
                        rollCounter = 0.2f;
                        jumpVel -= 10f;
                        charController.center = new Vector3(0, colCenterY / 2f, 0);
                        charController.height = colHeight / 2;
                        anim.CrossFadeInFixedTime("Slide", 0.1f);
                        inSlide = true;
                        inJump = false;
                    }
                }
                else
                {
                    jumpVel -= gravity;
                }
            }
        }
        move.y = jumpVel;
        
        collisionCol.isTrigger = !canInput;
        if(!canInput){
            charController.Move(Vector3.down * 10f * Time.deltaTime);
            return;
        }

        x = Mathf.Lerp(x, newXPos, Time.deltaTime * speed);
        Vector3 moveVector = new Vector3(x - transform.position.x, jumpVel * Time.deltaTime, speed * Time.deltaTime);
        charController.Move(moveVector);
    }

    public void OnTriggerEnter(Collider col){
        if (col.gameObject.CompareTag("Obstacles"))
        {
            StartCoroutine(DeathAnim());
        }
    }
    IEnumerator DeathAnim()
    {
        anim.Play("RockFall");
        canInput = false;
        yield return new WaitForSeconds(1);
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        yield break;
    }
}
