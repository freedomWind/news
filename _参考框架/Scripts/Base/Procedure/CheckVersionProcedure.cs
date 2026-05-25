using System.IO;
using System.Text;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Fsm;
using GameFramework.Procedure;

public class CheckVersionProcedure : GameBaseProcedure
{
    private bool m_CheckVersionComplete = false;
    private bool m_NeedUpdateVersion = false;
    private VersionInfo lastestVersionInfo = null;

    private string versionUrl = "http://120.79.143.37/Server_#/app4/appVersion/version.bat";

    private string resourceUrl = null;

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        m_CheckVersionComplete = false;
        m_NeedUpdateVersion = false;
        lastestVersionInfo = null;
        
        EntityInst.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
        EntityInst.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

        var builtInData = EntityInst.BuiltinData.BuildInfo;
        if (!string.IsNullOrEmpty(builtInData.CheckVersionUrl))
        {
            versionUrl = builtInData.CheckVersionUrl;
        }
        
        versionUrl = versionUrl.Replace("#",GetPlatformPath());
        Log.Debug("CheckVersionProcedure: versionUrl = {0}", versionUrl);
        //var url = versionUrl.Replace("#","Android");
        // 向服务器请求版本信息
        EntityInst.WebRequest.AddWebRequest(Utility.Text.Format(versionUrl, GetPlatformPath(),builtInData.Channel), this);
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        EntityInst.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
        EntityInst.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);

        base.OnLeave(procedureOwner, isShutdown);
    }

    protected override void OnNewUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
    {
        base.OnNewUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

        if (!m_CheckVersionComplete)
        {
            return;
        }

        if (m_NeedUpdateVersion)
        {
            procedureOwner.SetData<VarInt32>("VersionListLength", lastestVersionInfo.VersionListLength);
            procedureOwner.SetData<VarInt32>("VersionListHashCode", lastestVersionInfo.VersionListHashCode);
            procedureOwner.SetData<VarInt32>("VersionListCompressedLength", lastestVersionInfo.VersionListCompressedLength);
            procedureOwner.SetData<VarInt32>("VersionListCompressedHashCode", lastestVersionInfo.VersionListCompressedHashCode);
            ChangeState<ProcedureUpdateVersion>(procedureOwner);
        }
        else
        {
            ChangeState<CheckResourcesProcedure>(procedureOwner);
        }
    }

    private void GotoUpdateApp(object userData)
    {
        string url = null;

        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    private void OnWebRequestSuccess(object sender, GameEventArgs e)
    {
        WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }
        ParseVersionInfo(ne.GetWebResponseBytes());
        if(string.IsNullOrEmpty(resourceUrl))
            throw new System.Exception("Resource Update Url is null.");
        EntityInst.Resource.UpdatePrefixUri = resourceUrl;
    }
    
    private void ParseVersionInfo(byte[] versionInfoBytes)
    {
        // 解析版本信息
        string versionInfoString = Utility.Converter.GetString(versionInfoBytes,Encoding.UTF8);
        lastestVersionInfo = Utility.Json.ToObject<VersionInfo>(versionInfoString);
        
        if (lastestVersionInfo == null)
        {
            Log.Error("Parse VersionInfo failure.");
            return;
        }

        Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.", lastestVersionInfo.LatestGameVersion, lastestVersionInfo.InternalGameVersion.ToString(), Version.GameVersion, Version.InternalGameVersion.ToString());

        if (lastestVersionInfo.ForceGameUpdate)
        {
            // 需要强制更新游戏应用
            GotoUpdateApp(null);
            return;
        }

        m_CheckVersionComplete = true;
        resourceUrl = $"{lastestVersionInfo.ResourceUpdateUrl}/Server_{GetPlatformPath()}/app4/appResources";
        m_NeedUpdateVersion = EntityInst.Resource.CheckVersionList(lastestVersionInfo.InternalResourceVersion) == CheckVersionListResult.NeedUpdate;
    }

    private void OnWebRequestFailure(object sender, GameEventArgs e)
    {
        WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
        if (ne.UserData != this)
        {
            return;
        }
        
        var builtInData = EntityInst.BuiltinData.BuildInfo;
        versionUrl = versionUrl.Replace("#",GetPlatformPath());
        EntityInst.WebRequest.AddWebRequest(Utility.Text.Format(versionUrl, GetPlatformPath(),builtInData.Channel), this);

        Log.Warning("Check version failure, error message is '{0}'.", ne.ErrorMessage);
    }

    private string GetPlatformPath()
    {
        return "Android";
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return "Windows64";

            case RuntimePlatform.IPhonePlayer:
                return "IOS";

            case RuntimePlatform.Android:
                return "Android";

            default:
                throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.", Application.platform.ToString()));
        }
    }
}