using UnityEngine;
using System.Collections;

public abstract class Unit : MonoBehaviour, IUnit 
{
    private int _unitID;

    #region IUnit implementation

    public int unitID
    {
        get
        {
            return _unitID;
        }
    }

    public void SetID(int uniqueID)
    {
        if(_unitID == 0)
        {
            _unitID = uniqueID;
        }    
    }

    #endregion
}
