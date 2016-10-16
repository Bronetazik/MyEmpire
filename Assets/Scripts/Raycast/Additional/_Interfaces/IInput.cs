public interface IInput 
{
    bool MouseDown();
    bool MouseUp();
    bool MouseHold();
    int GetTouchCount();
    UnityEngine.Vector3 GetTouchPosition();
}
