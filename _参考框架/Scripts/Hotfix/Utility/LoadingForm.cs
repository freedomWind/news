using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class LoadingForm : UIFormLogic, ILoadingForm
{
    [SerializeField]
    private Slider loadingSlider;

    private IFsm<IProcedureManager> _procedureInst;

    public float LoadingProgress
    {
        get { return loadingSlider.value; }
        set { loadingSlider.value = value; }
    }

    public void OpenLoadingForm()
    {
        throw new NotImplementedException();
    }

    public void FinishLoading()
    {
        throw new NotImplementedException();
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        
        _procedureInst = EntityInst.Fsm.GetFsm<IProcedureManager>();
        LoadingProgress = 0f;
    }
    
    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        if (_procedureInst.HasData("BattleLoading"))
        {
            var progress = _procedureInst.GetData<VarInt32>("BattleLoading").Value;
            LoadingProgress = Mathf.MoveTowards(LoadingProgress, progress * 0.01f, Time.deltaTime);

            if (progress == 100)
            {
                EntityInst.UI.CloseUIForm(this.UIForm);
            }
        }
    }
}
