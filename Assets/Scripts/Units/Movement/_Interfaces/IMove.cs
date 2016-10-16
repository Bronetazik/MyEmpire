using UnityEngine;
using System.Collections;
using EnumsContainer;

public interface IMove 
{
    MoveState moveState{get;}
    void StartMove();
    void StopMove();
    void MoveToPosition(Vector3 position);
}
