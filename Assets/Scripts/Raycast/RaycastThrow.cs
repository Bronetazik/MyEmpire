using UnityEngine;
using System.Collections;

public class RaycastThrow : MonoBehaviour 
{
    #region raycast_variables
    private Ray ray;
    private RaycastHit hit;
    private Vector3 touchPoint;
    #endregion

    public delegate void raycastThrow();
    public static event raycastThrow OnMouseUp;
    public static event raycastThrow OnMouseDown;

    public delegate void raycastThrowPosition(Vector3 position);
    public static event raycastThrowPosition OnMouseUpPosition;
    public static event raycastThrowPosition OnMouseDownPosition;

    private IInput inputControl;

    private const string detectLayerName = "World";

    void Start()
    {
        SelectInputType();
    }
        
    void Update () 
    {
        int touchCount = inputControl.GetTouchCount();
        if (touchCount == 0 || touchCount > 1) return;

        if (inputControl.MouseDown())
        {
            touchPoint = GetCastPosition();
            MouseDownReact();
        }
        else if (inputControl.MouseUp())
        {
            touchPoint = GetCastPosition();
            MouseUpReact();
        }
    }

    private Vector3 GetCastPosition()
    {
        ray = Camera.main.ScreenPointToRay(inputControl.GetTouchPosition());
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(detectLayerName)))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void MouseDownReact()
    {
        if(OnMouseDown != null)
        {
            OnMouseDown();
        }

        if(OnMouseDownPosition != null)
        {
            OnMouseDownPosition(touchPoint);
        }
    }

    private void MouseUpReact()
    {
        if(OnMouseUp != null)
        {
            OnMouseUp();
        }

        if(OnMouseUpPosition != null)
        {
            OnMouseUpPosition(touchPoint);
        }
    }

    private void SelectInputType()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
        inputControl = new MouseInput();
        #endif
    }
}
