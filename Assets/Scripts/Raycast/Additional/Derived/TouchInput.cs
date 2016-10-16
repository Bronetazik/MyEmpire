using UnityEngine;
using System.Collections;

public class TouchInput : IInput 
{
    #region IInput implementation

    public bool MouseDown()
    {
        TouchPhase tPhase = Input.GetTouch(0).phase;
        if(tPhase == TouchPhase.Began) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool MouseUp()
    {
        TouchPhase tPhase = Input.GetTouch(0).phase;
        if(tPhase == TouchPhase.Ended) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public bool MouseHold()
    {
        TouchPhase tPhase = Input.GetTouch(0).phase;
        if(tPhase == TouchPhase.Moved || tPhase == TouchPhase.Stationary) 
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public int GetTouchCount()
    {
        return Input.touchCount;
    }

    public Vector3 GetTouchPosition()
    {
        Vector2 touchPosition = Input.GetTouch(0).position;
        return new Vector3(touchPosition.x, touchPosition.y, 0f);
    }

    #endregion
}
