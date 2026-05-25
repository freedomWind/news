/*
*FileName:	EntityInst
*Author:	油菜花
*CreateTime:2022/5/24 15:19:46
*Description:
*/

//using Unity.Entities;
using UnityEngine;
using UnityGameFramework.Runtime;

/// <summary>
/// 框架访问
/// </summary>
public sealed class EntityInst : MonoBehaviour
{
    public static BaseComponent Base { get; private set; }
    public static EventComponent Event { get; private set; }
    public static UIComponent UI { get; private set; }
    public static DataTableComponent Table { get; private set; }
    public static ConfigComponent Config { get; private set; }
    public static ProcedureComponent Procedure { get; private set; }
    public static ObjectPoolComponent ObjectPool { get; private set; }
    public static ReferencePoolComponent RefPool { get; private set; }
    public static ResourceComponent Resource { get; private set; }
    public static SceneComponent Scene { get; private set; }
    public static SoundComponent Sound { get; private set; }
    public static WebRequestComponent WebRequest { get; private set; }
    public static DownloadComponent Download { get; private set; }
    public static SettingComponent Setting { get; private set; }
    public static EntityComponent Entity { get; private set; }
    public static  FsmComponent Fsm { get; private set; }
    public static  LocalizationComponent Localizer { get; private set; }
    public static NetworkComponent Network { get; private set; }
    public static DataNodeComponent DataNode { get; private set; }
    public static BuiltinDataComponent BuiltinData { get; private set; }


    private void Start()
    {
        Base = GameEntry.GetComponent<BaseComponent>();
        Event = GameEntry.GetComponent<EventComponent>();
        UI = GameEntry.GetComponent<UIComponent>();
        Table = GameEntry.GetComponent<DataTableComponent>();
        DataNode = GameEntry.GetComponent<DataNodeComponent>();
        Config = GameEntry.GetComponent<ConfigComponent>();
        Procedure = GameEntry.GetComponent<ProcedureComponent>();
        ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();
        RefPool = GameEntry.GetComponent<ReferencePoolComponent>();
        Resource = GameEntry.GetComponent<ResourceComponent>();
        Scene = GameEntry.GetComponent<SceneComponent>();
        Sound = GameEntry.GetComponent<SoundComponent>();
        WebRequest = GameEntry.GetComponent<WebRequestComponent>();
        Download = GameEntry.GetComponent<DownloadComponent>();
        Setting = GameEntry.GetComponent<SettingComponent>();
        Entity = GameEntry.GetComponent<EntityComponent>();
        Fsm = GameEntry.GetComponent<FsmComponent>();
        Localizer = GameEntry.GetComponent<LocalizationComponent>();
        Network = GameEntry.GetComponent<NetworkComponent>();
        BuiltinData = GameEntry.GetComponent<BuiltinDataComponent>();

//jkjdkdd
        //Debug.Log($"防止被裁剪 {World.DefaultGameObjectInjectionWorld == null}"); ;
    }
}
