# 资源加载系统 - 简洁版本

## 📋 设计理念

**工具化、无状态、不存储数据**：
- ✅ 只负责加载和解析资源
- ✅ 数据通过回调直接传递给业务层
- ✅ 业务层自己决定如何存储和管理数据
- ✅ 不保留数据，避免数据"缠着"系统

## 🚀 快速开始

### 基本使用

```csharp
using Hotfix.ResourceLoader;

// 在进入 Procedure 前配置
GameLoadProcedureV3.Configure()
    .Add("Scene", "BattleScene", (data) =>
    {
        Debug.Log("场景加载完成");
    })
    .Add<byte[]>("Binary", "AnimationDatabase.bytes", (bytes) =>
    {
        // 业务层自己处理数据
        AnimationDatabaseLoader.LoadDatabase("AnimationDatabase.bytes");
    })
    .Add<ConfigData>("Json<ConfigData>", "Config.json", (config) =>
    {
        // 业务层自己存储
        ConfigManager.Instance.SetConfig(config);
    });

// 切换到加载流程
ChangeState<GameLoadProcedureV3>(procedureOwner);
```

### 错误处理

```csharp
GameLoadProcedureV3.Configure()
    .Add<byte[]>("Binary", "Data.bytes",
        (data) => { /* 成功处理 */ },
        (error) => { /* 失败处理 */ }
    );
```

### 批量加载

```csharp
GameLoadProcedureV3.Configure()
    .AddBatch(
        "Scene:BattleScene#Binary:Data.bytes#Json<Config>:Config.json",
        (name, data) => { /* 处理每个资源 */ },
        (name, error) => { /* 处理错误 */ }
    );
```

## 📦 核心组件

### 1. ResourceLoadConfig（配置类）

```csharp
var config = new ResourceLoadConfig();
config.Add("Binary", "Data.bytes", (data) => { /* 处理 */ });
config.Add<ConfigData>("Json<ConfigData>", "Config.json", (config) => { /* 处理 */ });
```

### 2. SimpleResourceLoader（简化加载器）

只负责加载和解析，不存储数据。

### 3. GameLoadProcedureV3（Procedure）

提供静态配置函数 `Configure()`。

## 🔧 内置加载器

- `SceneResourceLoader` - 场景加载
- `BinaryResourceLoader` - 二进制数据加载
- `JsonResourceLoader` - JSON 字符串加载
- `JsonResourceLoader<T>` - JSON 自动解析为类型
- `ScriptableObjectResourceLoader` - ScriptableObject 加载
- `ScriptableObjectResourceLoader<T>` - 泛型 ScriptableObject 加载

## 💡 自定义加载器

```csharp
public class CustomResourceLoader : IResourceLoader
{
    public string ResourceType => "Custom";
    
    public void Load(string resourceName, Action<object, object> onSuccess, Action<string> onFailure, object userData = null)
    {
        // 加载逻辑
    }
    
    public object Parse(object rawAsset, string resourceName)
    {
        // 解析逻辑
        return rawAsset;
    }
    
    public void Store(object parsedData, string resourceName)
    {
        // V3版本不存储数据，留空
    }
    
    public object GetStored(string resourceName)
    {
        // V3版本不存储数据，返回null
        return null;
    }
    
    public void Unload(string resourceName)
    {
        // 卸载逻辑
    }
}

// 注册自定义加载器
SimpleResourceLoader.Instance.RegisterLoader(new CustomResourceLoader());
```

## 📚 完整示例

查看 `GameLoadProcedureV3使用示例.md` 获取更多示例。

## 🔑 关键特性

1. **不存储数据**：数据通过回调直接传递给业务层
2. **类型安全**：使用泛型方法，自动类型转换
3. **静态配置**：在任何地方配置，在 Procedure 执行时使用
4. **兼容旧格式**：支持原有的 Procedure 数据格式

## 📝 注意事项

- 加载器不存储数据，业务层需要自己管理数据
- Store 和 GetStored 方法保留为空实现以兼容接口
- 数据通过回调直接传递给业务层处理

