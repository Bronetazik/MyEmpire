using UnityEngine;
using System.Collections;
using EnumsContainer;

public class BaseMove : MonoBehaviour, IMove 
{
    #region IMove implementation
    private MoveState _moveState;

    public MoveState moveState
    {
        get
        {
            return _moveState;
        }
    }

    public void StartMove()
    {
        _moveState = MoveState.WALK;
    }

    public void StopMove()
    {
        _moveState = MoveState.STAY;
    }

    public void MoveToPosition(Vector3 position)
    {
        StartMove();
    }

    #endregion

    [SerializeField] private float walkSpeed = 1.0f;

    private bool IsWalked;
    private Vector3 tempScale;
    private float t;

    private Vector2 startPosotion;
    private Vector2 finishPosition;
  
    private IEnumerator StartWalk()
    {
        if (t >= 1)
        {
            t = 0f;

            startPosotion = Vector3.zero;
            finishPosition = Vector3.zero;

            t += Time.deltaTime * walkSpeed;
            this.transform.localPosition = new Vector3(Mathf.Lerp(startPosotion.x, finishPosition.x, t),
                Mathf.Lerp(startPosotion.y, finishPosition.y, t),
                this.transform.localPosition.z);
            yield return null;
        }
    }
}
