using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float m_startTime  = 0.0f;
    private Vector2 m_startPos = Vector2.zero;

    private bool m_swiping = false;
    private bool m_holding = false;
    private float m_minDist  = 50.0f;
    private float m_maxTime = 0.5f;

    void Update () {
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
                    DetectSwipe(touch);
                    m_holding = false;
                    break;
                case TouchPhase.Moved :
                    DetectSwipe(touch);
                    break;
                case TouchPhase.Stationary :
                    DetectSwipe(touch);
                    break;
                }
            }
        }
    }

    void DetectSwipe (Touch touch) {
        float totalTime = Time.time - m_startTime;
        float totalDist = (touch.position - m_startPos).magnitude;
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
                 if(swipeType.x > 0.0f){
                     // HANDLE RIGHT SWIPE
                 }
                 else{
                     // HANDLE LEFT SWIPE
                 }
            }
            if(swipeType.y != 0.0f ){
                 m_swiping = false;  // <- THIS MAKES THE DIFFERENCE
                 if(swipeType.y > 0.0f){
                     // HANDLE UP SWIPE
                 }
                 else{
                     // HANDLE DOWN SWIPE
                 }
            }
        }
    }
}
