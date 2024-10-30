using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwipeControl : MonoBehaviour
{
    public static event System.Action<SwipeDirection> OnSwipe = delegate { };
    Vector2 swipeStart;
    Vector2 swipeEnd;
    float swipDistance = 10;
    

    public enum SwipeDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameController.instance.alive) return;
      
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
               
                swipeStart = touch.position;
            }
            else if(touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
                ProcessSwipe();
            }
        }
        //For Mouse
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {

            swipeEnd = Input.mousePosition;

            ProcessSwipe();
        }
    }
    void ProcessSwipe()
    {
       float distance = Vector2.Distance(swipeStart, swipeEnd);
        if (distance > swipDistance)
        {
            if(IsVerticalSwipe())
            {
                if(swipeEnd.y > swipeStart.y)
                {
                    OnSwipe(SwipeDirection.Up);
                }
                else
                {
                    OnSwipe(SwipeDirection.Down);
                }
            }
            else
            {
                if(swipeEnd.x > swipeStart.x)
                {
                    OnSwipe(SwipeDirection.Right);
                }
                else
                {
                    OnSwipe(SwipeDirection.Left);
                }
            }
        }
    }
    bool IsVerticalSwipe()
    {
        float vertical = Mathf.Abs(swipeEnd.y  - swipeStart.y);
        float horizontal = Mathf.Abs(swipeEnd.x - swipeStart.x);

        if(vertical > horizontal)
            return true;
        return false;

    }
}
