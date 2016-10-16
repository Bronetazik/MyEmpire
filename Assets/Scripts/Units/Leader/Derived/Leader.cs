using UnityEngine;
using System.Collections;

public class Leader : MonoBehaviour, ILeader 
{
    private IUnit[] _squad;

    #region ILeader implementation
    public IUnit[] squad
    {
        get
        {
            return _squad;
        }
        set
        {
            _squad = value;
        }
    }

    public void CreateSquad()
    {
        throw new System.NotImplementedException();
    }
        
    public void AddUnit()
    {
        throw new System.NotImplementedException();
    }

    public void RemoveUnit()
    {
        throw new System.NotImplementedException();
    }

    public int GetSquadCount()
    {
        throw new System.NotImplementedException();
    }

    #endregion
}
