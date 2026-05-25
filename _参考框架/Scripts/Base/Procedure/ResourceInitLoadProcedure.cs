using GameFramework.Fsm;
using GameFramework.Procedure;

public class ResourceInitLoadProcedure : GameBaseProcedure
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        EntityInst.Resource.InitResources(() =>
        {
            ChangeState<CodeInitProcedure>(procedureOwner);
        });
    }
}