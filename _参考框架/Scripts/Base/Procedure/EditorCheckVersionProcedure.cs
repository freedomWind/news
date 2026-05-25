using System.IO;
using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;

public class EditorCheckVersionProcedure : GameBaseProcedure
{
    private bool m_CheckVersionComplete = false;
    private bool m_NeedUpdateVersion = false;
    private VersionInfo lastestVersionInfo = null;
    
    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);

        m_CheckVersionComplete = false;
        m_NeedUpdateVersion = false;
        lastestVersionInfo = null;


        var versionPath = "D://testServer/appVersion/Android/version.bat";
        if (!File.Exists(versionPath))
            return;
        var txt = File.ReadAllText(versionPath);
        ParseVersionInfo(txt);
        // 设置资源更新下载地址
        EntityInst.Resource.UpdatePrefixUri = Utility.Path.GetRegularPath("D://testServer/appResources/Android");
        
        // 向服务器请求版本信息
        //var builtInData = EntityInst.BuiltinData.BuildInfo;
        //EntityInst.WebRequest.AddWebRequest(Utility.Text.Format(builtInData.CheckVersionUrl, GetPlatformPath(),builtInData.Channel), this);
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


    private void ParseVersionInfo(string txt)
    {
        // 解析版本信息
        lastestVersionInfo = JsonUtility.FromJson<VersionInfo>(txt);
        if (lastestVersionInfo == null)
        {
            Log.Error("Parse VersionInfo failure.");
            return;
        }

        Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.", lastestVersionInfo.LatestGameVersion, lastestVersionInfo.InternalGameVersion.ToString(), Version.GameVersion, Version.InternalGameVersion.ToString());

        Log.Info($"vvvvv:{lastestVersionInfo.VersionListLength}");
        if (lastestVersionInfo.ForceGameUpdate)
        {
            // 需要强制更新游戏应用

            return;
        }

        m_CheckVersionComplete = true;
        m_NeedUpdateVersion = EntityInst.Resource.CheckVersionList(lastestVersionInfo.InternalResourceVersion) == CheckVersionListResult.NeedUpdate;
    }
}