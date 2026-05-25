using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

public class CheckResourcesProcedure : GameBaseProcedure
{
    private bool m_CheckResourcesComplete = false;
    private bool m_NeedUpdateResources = false;
    private int m_UpdateResourceCount = 0;
    private long m_UpdateResourceTotalCompressedLength = 0L;
    

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        m_CheckResourcesComplete = false;
        m_NeedUpdateResources = false;
        m_UpdateResourceCount = 0;
        m_UpdateResourceTotalCompressedLength = 0L;

        EntityInst.Resource.CheckResources(OnCheckResourcesComplete);
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        if (!m_CheckResourcesComplete)
        {
            return;
        }

        if (m_NeedUpdateResources)
        {
            procedureOwner.SetData<VarInt32>("UpdateResourceCount", m_UpdateResourceCount);
            procedureOwner.SetData<VarInt64>("UpdateResourceTotalCompressedLength", m_UpdateResourceTotalCompressedLength);
            ChangeState<ResourceUpdateProcedure>(procedureOwner);
        }
        else
        {
            ChangeState<CodeInitProcedure>(procedureOwner);
        }
    }

    private void OnCheckResourcesComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalCompressedLength)
    {
        var assetGroup = EntityInst.Resource.GetResourceGroup(string.Empty);
        m_CheckResourcesComplete = true;
        m_NeedUpdateResources = updateCount > 0 && (assetGroup.TotalCount - assetGroup.ReadyCount > 0);
        m_UpdateResourceCount = assetGroup.TotalCount - assetGroup.ReadyCount;
        m_UpdateResourceTotalCompressedLength = assetGroup.TotalLength - assetGroup.ReadyLength;
        Log.Info("Check resources complete, '{0}' resources need to update, compressed length is '{1}', uncompressed length is '{2}'.", updateCount.ToString(), updateTotalCompressedLength.ToString(), updateTotalLength.ToString());
    }
}