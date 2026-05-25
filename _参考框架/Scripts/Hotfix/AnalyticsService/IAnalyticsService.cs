namespace Hotfix.AnalyticsService
{
using System;
using System.Collections.Generic;
using UnityEngine;

// 1. 定义统一的数据上报接口
public interface IAnalyticsService
{
    void Initialize();
    void Track(string eventName, Dictionary<string, object> properties = null);
    void Identify(string userId, Dictionary<string, object> traits = null);
    void SetUserProperty(string key, object value);
    void SetGlobalProperties(Dictionary<string, object> properties);
    void Flush();
    void SetEnabled(bool enabled);
    bool GetEnabled();
}

// 2. 自定义上报实现
public class SelfHostedAnalytics : IAnalyticsService
{
    private string endpoint;
    private bool enabled = true;
    private Dictionary<string, object> globalProperties = new Dictionary<string, object>();
    private string userId = null;
    private List<AnalyticsEvent> eventQueue = new List<AnalyticsEvent>();

    public SelfHostedAnalytics(string endpoint)
    {
        this.endpoint = endpoint;
    }

    public void Initialize()
    {
        Debug.Log("Self-hosted analytics initialized");
    }

    public void Track(string eventName, Dictionary<string, object> properties = null)
    {
        if (!enabled) return;

        var finalProperties = new Dictionary<string, object>(globalProperties);
        if (properties != null)
        {
            foreach (var kvp in properties)
            {
                finalProperties[kvp.Key] = kvp.Value;
            }
        }
        finalProperties["event_time"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        finalProperties["user_id"] = userId;

        var analyticsEvent = new AnalyticsEvent
        {
            eventName = eventName,
            properties = finalProperties,
            timestamp = DateTime.UtcNow
        };

        eventQueue.Add(analyticsEvent);

        // 尝试立即发送（实际应用中可能需要队列管理和批量发送）
        Flush();
    }

    public void Identify(string userId, Dictionary<string, object> traits = null)
    {
        if (!enabled) return;
        this.userId = userId;
        if (traits != null)
        {
            SetGlobalProperties(traits);
        }
    }

    public void SetUserProperty(string key, object value)
    {
        if (!enabled) return;
        SetGlobalProperties(new Dictionary<string, object> { { key, value } });
    }

    public void SetGlobalProperties(Dictionary<string, object> properties)
    {
        if (!enabled) return;
        foreach (var kvp in properties)
        {
            globalProperties[kvp.Key] = kvp.Value;
        }
    }

    public void Flush()
    {
        if (!enabled || eventQueue.Count == 0) return;

        var eventsToSend = new List<AnalyticsEvent>(eventQueue);
        eventQueue.Clear();

        // 模拟发送到服务器
        Debug.Log($"Sending {eventsToSend.Count} events to self-hosted server:");
        foreach (var evt in eventsToSend)
        {
            Debug.Log($"  Event: {evt.eventName}, Properties: {JsonUtility.ToJson(evt.properties)}");
        }

        // 实际实现中应该是网络请求
        // SendToServer(eventsToSend);
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public bool GetEnabled()
    {
        return enabled;
    }
}

// 3. 第三方工具适配器示例 (模拟实现)
public class ThirdPartyAnalytics : IAnalyticsService
{
    private bool enabled = true;
    private Dictionary<string, object> globalProperties = new Dictionary<string, object>();
    private string userId = null;

    public void Initialize()
    {
        Debug.Log("Third-party analytics service initialized");
    }

    public void Track(string eventName, Dictionary<string, object> properties = null)
    {
        if (!enabled) return;

        var finalProperties = new Dictionary<string, object>(globalProperties);
        if (properties != null)
        {
            foreach (var kvp in properties)
            {
                finalProperties[kvp.Key] = kvp.Value;
            }
        }
        finalProperties["user_id"] = userId;

        Debug.Log($"[ThirdParty] Tracking event: {eventName}, Properties: {string.Join(", ", finalProperties)}");
    }

    public void Identify(string userId, Dictionary<string, object> traits = null)
    {
        if (!enabled) return;
        this.userId = userId;
       // Debug.Log($"[ThirdParty] Identifying user: {userId}, Traits: {string.Join(", ", traits?.Values ?? new List<object>())}");
    }

    public void SetUserProperty(string key, object value)
    {
        if (!enabled) return;
        Debug.Log($"[ThirdParty] Setting user property: {key} = {value}");
    }

    public void SetGlobalProperties(Dictionary<string, object> properties)
    {
        if (!enabled) return;
        foreach (var kvp in properties)
        {
            globalProperties[kvp.Key] = kvp.Value;
        }
    }

    public void Flush()
    {
        if (!enabled) return;
        Debug.Log("[ThirdParty] Flushing events");
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
    }

    public bool GetEnabled()
    {
        return enabled;
    }
}

// 4. 分析事件结构
[System.Serializable]
public class AnalyticsEvent
{
    public string eventName;
    public Dictionary<string, object> properties;
    public DateTime timestamp;
}

// 5. 上报管理器 - 使用策略模式
public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    [Header("Analytics Configuration")]
    public AnalyticsProvider currentProvider = AnalyticsProvider.SelfHosted;
    public string selfHostedEndpoint = "/api/analytics";
    public bool isEnabled = true;

    private IAnalyticsService currentService;
    private Dictionary<AnalyticsProvider, IAnalyticsService> services = new Dictionary<AnalyticsProvider, IAnalyticsService>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeServices();
            InitializeCurrentService();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeServices()
    {
        services[AnalyticsProvider.SelfHosted] = new SelfHostedAnalytics(selfHostedEndpoint);
        services[AnalyticsProvider.ThirdParty] = new ThirdPartyAnalytics();
    }

    private void InitializeCurrentService()
    {
        if (services.ContainsKey(currentProvider))
        {
            currentService = services[currentProvider];
            currentService.SetEnabled(isEnabled);
            currentService.Initialize();
        }
    }

    public void SwitchService(AnalyticsProvider provider)
    {
        if (services.ContainsKey(provider))
        {
            currentProvider = provider;
            currentService = services[provider];
            currentService.SetEnabled(isEnabled);
            currentService.Initialize();
            Debug.Log($"Switched to {provider} analytics service");
        }
    }

    public void Track(string eventName, Dictionary<string, object> properties = null)
    {
        currentService?.Track(eventName, properties);
    }

    public void Identify(string userId, Dictionary<string, object> traits = null)
    {
        currentService?.Identify(userId, traits);
    }

    public void SetUserProperty(string key, object value)
    {
        currentService?.SetUserProperty(key, value);
    }

    public void SetGlobalProperties(Dictionary<string, object> properties)
    {
        currentService?.SetGlobalProperties(properties);
    }

    public void Flush()
    {
        currentService?.Flush();
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (currentService != null)
        {
            currentService.SetEnabled(enabled);
        }
    }

    public bool GetEnabled()
    {
        return isEnabled && currentService?.GetEnabled() == true;
    }
}

// 6. 分析提供商枚举
public enum AnalyticsProvider
{
    SelfHosted,
    ThirdParty
}

// 7. 编辑器配置窗口

// 8. 测试脚本 - 演示如何在游戏代码中使用
public class AnalyticsTest : MonoBehaviour
{
    void Start()
    {
        // 初始化示例
        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.Identify("player123", new Dictionary<string, object>
            {
                { "level", 5 },
                { "class", "warrior" }
            });

            AnalyticsManager.Instance.SetGlobalProperties(new Dictionary<string, object>
            {
                { "game_version", "1.0.0" },
                { "platform", Application.platform.ToString() }
            });

            AnalyticsManager.Instance.Track("game_start", new Dictionary<string, object>
            {
                { "scene", "main_menu" },
                { "time_of_day", DateTime.Now.Hour }
            });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.Track("player_action", new Dictionary<string, object>
                {
                    { "action_type", "jump" },
                    { "position_x", transform.position.x },
                    { "position_y", transform.position.y }
                });
            }
        }
    }
}       
}       