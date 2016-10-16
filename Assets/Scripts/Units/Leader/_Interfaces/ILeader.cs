public interface ILeader 
{
    IUnit[] squad{get;set;}
    void CreateSquad();
    void AddUnit();
    void RemoveUnit();
    int GetSquadCount();
}
