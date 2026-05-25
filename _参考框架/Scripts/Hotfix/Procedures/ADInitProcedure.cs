using System;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

public class ADInitProcedure : GameBaseProcedure
{
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        EntityInst.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
        EntityInst.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

        // var authURL = "https://platform.ironsrc.com/partners/publisher/auth";
        // EntityInst.WebRequest.AddWebRequest(authURL, this);

        // var mgr = new GameObject("AdsMgr");
        // mgr.AddComponent<AdsPlayerAndRewards>();
        // mgr.AddComponent<AdsVideoPlayer>();
        // GameObject.DontDestroyOnLoad(mgr);
        
        ChangeState<LoginRegistProcedure>(procedureOwner);
    }

    private void OnWebRequestSuccess(object sender, GameEventArgs e)
    {
        WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }
    }

    private void OnWebRequestFailure(object sender, GameEventArgs e)
    {
        WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }

        Log.Warning("Check version failure, error message is '{0}'.", ne.ErrorMessage);
    }
    
    // class AdsPlayerAndRewards : MonoBehaviour
    // {
    //     private void LateUpdate()
    //     {
    //         var fsm = EntityInst.Fsm.GetFsm<IFsmManager>();
    //         if (fsm.HasData("AdsPlay"))
    //         {
    //             var adsPlay = fsm.GetData<VarString>("AdsPlay");
    //             fsm.RemoveData("AdsPlay");
    //             switch (adsPlay)
    //             {
    //                 case "video":
    //                     PlayVideoAds();
    //                     break;
    //             }
    //         }
    //     
    //         if (fsm.HasData("AdsReward"))
    //         {
    //             var adsReward = fsm.GetData<VarString>("AdsReward");
    //             fsm.RemoveData("AdsReward");
    //     
    //             var placement = IronSource.Agent.getPlacementInfo(adsReward);
    //             if (placement != null)
    //             {
    //                 var rewardName = placement.getRewardName();
    //                 var rewardAmount = placement.getRewardAmount();
    //                 Debug.Log($"发放奖励：{rewardName},{rewardAmount}");
    //             }
    //         }
    //     }
    //     
    //     void PlayVideoAds()
    //     {
    //         if (IronSource.Agent.isRewardedVideoAvailable())
    //         {
    //             IronSource.Agent.showRewardedVideo();
    //         }
    //         else
    //         {
    //             Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
    //         }
    //     }
    //
    //     void OnApplicationPause(bool isPaused) { 	 
    //         IronSource.Agent.onApplicationPause(isPaused);	 
    //     }
    // }
}

// public class AdsVideoPlayer : MonoBehaviour
// {
//     private string appKey = null;
//     public void SetAppKey(string key)
//     {
//         appKey = key;
//     }
//
//     private void Awake()
//     {
//         if (appKey == null)
//         {
//             appKey = "20b5905ad";
//         }
//         
//         IronSource.Agent.setMetaData("is_test_suite","enable");
//         IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
//     }
//     private void SdkInitializationCompletedEvent()
//     {
//         Debug.Log("unity-script: IronSource.Agent.validateIntegration");
//         IronSource.Agent.launchTestSuite();
//     }
//
//     private void Start()
//     {
//         Debug.Log("unity-script: IronSource.Agent.validateIntegration");
//         //IronSource.Agent.validateIntegration();
//
//         Debug.Log("unity-script: unity version" + IronSource.unityVersion());
//
//         // SDK init
//         Debug.Log($"unity-script: LevelPlay SDK initialization:{appKey}");
//         LevelPlay.Init(appKey,adFormats:new []{LevelPlayAdFormat.REWARDED});
//         
//         LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
//         LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
//     }
//
//     void EnableAds()
//     {
//         //Add AdInfo Rewarded Video Events
//         IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
//         IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
//         IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
//         IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
//         IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
//         IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
//         IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
//     }
//     
//     void SdkInitializationCompletedEvent(LevelPlayConfiguration config)
//     {
//         Debug.Log("unity-script: I got SdkInitializationCompletedEvent with config: "+ config);
//         
//         
//         //IronSource.Agent.setManualLoadRewardedVideo(true);  //手动
//         
//         EnableAds();
//     }
//     
//     void SdkInitializationFailedEvent(LevelPlayInitError error)
//     {
//         Debug.Log("unity-script: I got SdkInitializationFailedEvent with error: "+ error);
//     }
//     
//     #region AdInfo Rewarded Video
//     void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdOpenedEvent With AdInfo " + adInfo);
//     }
//
//     void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdClosedEvent With AdInfo " + adInfo);
//     }
//
//     void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdAvailable With AdInfo " + adInfo);
//         
//     }
//
//     void RewardedVideoOnAdUnavailable()
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdUnavailable");
//     }
//
//     void RewardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdShowFailedEvent With Error" + ironSourceError + "And AdInfo " + adInfo);
//     }
//
//     void RewardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
//     }
//
//     void RewardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
//     {
//         Debug.Log("unity-script: I got RewardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement + "And AdInfo " + adInfo);
//     }
//
//     #endregion
// }
