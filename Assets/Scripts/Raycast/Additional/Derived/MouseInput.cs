using UnityEngine;
using System.Collections;

public class MouseInput : IInput 
{
    #region IInput implementation

    public bool MouseDown()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool MouseUp()
    {
        return Input.GetMouseButtonDown(0);
    }

    public bool MouseHold()
    {
        return Input.GetMouseButton(0);
    }

    public int GetTouchCount()
    {
        return 1;
    }

    public Vector3 GetTouchPosition()
    {
        return Input.mousePosition;
    }

    #endregion
}
