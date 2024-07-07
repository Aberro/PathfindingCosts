using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Colossal.IO.AssetDatabase;
using Unity.Collections;
using Unity.Entities;

namespace PathfindingCosts;

public class Mod : IMod
{
    public static ILog Log = LogManager.GetLogger($"{nameof(PathfindingCosts)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
    private static Setting? _Setting;
    public static Setting Setting => _Setting!;
    public void OnLoad(UpdateSystem updateSystem)
    {
        Log.Info(nameof(OnLoad));

        if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            Log.Info($"Current mod asset at {asset.path}");

        _Setting = new Setting(this);
        _Setting.RegisterInOptionsUI();
        GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(_Setting));

        //Setting.ReloadSettings();
        AssetDatabase.global.LoadSettings(nameof(PathfindingCosts), _Setting, new Setting(this));

        updateSystem.UpdateAfter<LoadDefaultDataSystem>(SystemUpdatePhase.LoadSimulation);
        updateSystem.UpdateAt<UpdateDataSystem>(SystemUpdatePhase.MainLoop);
    }

    public void OnDispose()
    {
        Log.Info(nameof(OnDispose));
        if (_Setting != null)
        {
            _Setting.UnregisterInOptionsUI();
            _Setting = null;
        }
    }
}

public partial class UpdateDataSystem : GameSystemBase
{
    private bool _needUpdate;
    public void UpdateCosts()
    {
        this._needUpdate = true;
    }
    protected override void OnUpdate()
    {
        if (!this._needUpdate)
            return;
        this._needUpdate = false;
        var prefabSystem = World.GetExistingSystemManaged<PrefabSystem>();
        UpdateData<Game.Prefabs.PathfindCarData, PathfindCarData>(prefabSystem, Mod.Setting.CarData);
        UpdateData<Game.Prefabs.PathfindPedestrianData, PathfindPedestrianData>(prefabSystem, Mod.Setting.PedestrianData);
        UpdateData<Game.Prefabs.PathfindTrackData, PathfindTrackData>(prefabSystem, Mod.Setting.TrackData);
    }

    /// <summary>
    /// Loads default data for the specified component type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="prefabSystem"></param>
    /// <param name="data"></param>
    private void UpdateData<T, TMod>(PrefabSystem prefabSystem, Dictionary<string, TMod> data) where T : unmanaged, IComponentData where TMod : struct, ILoadFromGame<T>
    {
        using var entities = QueryPrefabs<T>();
        foreach (var entity in entities)
        {
            var prefabName = GetPrefabName(prefabSystem, entity);
            if (prefabName is null)
                continue;
            if(data.TryGetValue(prefabName, out var moddedData))
            {
                var componentData = EntityManager.GetComponentData<T>(entity);
                moddedData.Set(componentData);
                EntityManager.SetComponentData(entity, componentData);
            }
        }
    }

    /// <summary>
    /// Returns the prefab name of the specified entity.
    /// </summary>
    /// <param name="prefabSystem"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    private string? GetPrefabName(PrefabSystem prefabSystem, Entity entity)
    {
        if (EntityManager.HasComponent<PrefabData>(entity))
        {
            var prefabData = EntityManager.GetComponentData<PrefabData>(entity);
            if (prefabSystem.TryGetPrefab(prefabData, out PrefabBase prefab))
                return prefab.name;
        }
        return null;
    }

    /// <summary>
    /// Gets entities with the specified component type, and the PrefabData component type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private NativeArray<Entity> QueryPrefabs<T>() => EntityManager.CreateEntityQuery(ComponentType.ReadWrite<T>(), ComponentType.ReadOnly<PrefabData>()).ToEntityArray(Allocator.Persistent);
}

[SuppressMessage("ReSharper", "PartialTypeWithSinglePart", Justification = "This is required for Roslyn Source Generator.")]
[UsedImplicitly]
public partial class LoadDefaultDataSystem : GameSystemBase
{
    protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
    {
        base.OnGameLoadingComplete(purpose, mode);
        Mod.Log.Info($"Loading {purpose}, where mode is {mode}");
        var prefabSystem = World.GetExistingSystemManaged<PrefabSystem>();
        Mod.Log.Info($"Accessing default car data...");
        LoadDefaultData<Game.Prefabs.PathfindCarData, PathfindCarData>(prefabSystem, Mod.Setting.DefaultCarData);
        Mod.Log.Info($"Accessing default pedestrian data...");
        LoadDefaultData<Game.Prefabs.PathfindPedestrianData, PathfindPedestrianData>(prefabSystem, Mod.Setting.DefaultPedestrianData);
        Mod.Log.Info($"Accessing default track data...");
        LoadDefaultData<Game.Prefabs.PathfindTrackData, PathfindTrackData>(prefabSystem, Mod.Setting.DefaultTrackData);
        Mod.Setting.ApplyAndSave();
        //AssetDatabase.global.SaveSettings();
        //Mod.Setting.Save();//.ApplyAndSave();
        //var m_UnsafeCrosswalkCost = new PathfindCosts(time: 0f, behaviour: 500f, money: 0f, comfort: 50f);
        //var entities = QueryPrefabs<PathfindPedestrianData>();

        //foreach (var entity in entities)
        //{
        //    var name = GetPrefabName(prefabSystem, entity);
        //    var componentData = EntityManager.GetComponentData<PathfindPedestrianData>(entity);
        //    //Mod.log.Info($"Setting pedestrian walking costs for prefab '{name}', changing from: {oldCosts.m_UnsafeCrosswalkCost.m_Value} to: {m_UnsafeCrosswalkCost.m_Value}");
        //    Mod.log.Info($"new Group(\"{name}\", \"{name}\", \"{name}\", " +
        //        $"new PathfindCosts({componentData.m_WalkingCost.m_Value.x}, {componentData.m_WalkingCost.m_Value.y}, {componentData.m_WalkingCost.m_Value.z}, {componentData.m_WalkingCost.m_Value.w}), " +
        //        $"new PathfindCosts({componentData.m_CrosswalkCost.m_Value.x}, {componentData.m_CrosswalkCost.m_Value.y}, {componentData.m_CrosswalkCost.m_Value.z}, {componentData.m_CrosswalkCost.m_Value.w}), " +
        //        $"new PathfindCosts({componentData.m_UnsafeCrosswalkCost.m_Value.x}, {componentData.m_UnsafeCrosswalkCost.m_Value.y}, {componentData.m_UnsafeCrosswalkCost.m_Value.z}, {componentData.m_UnsafeCrosswalkCost.m_Value.w}), " +
        //        $"new PathfindCosts({componentData.m_SpawnCost.m_Value.x}, {componentData.m_SpawnCost.m_Value.y}, {componentData.m_SpawnCost.m_Value.z}, {componentData.m_SpawnCost.m_Value.w})),");
        //    componentData.m_WalkingCost = componentData.m_WalkingCost;
        //    componentData.m_CrosswalkCost = componentData.m_CrosswalkCost;
        //    componentData.m_UnsafeCrosswalkCost = m_UnsafeCrosswalkCost;
        //    componentData.m_SpawnCost = componentData.m_SpawnCost;
        //    EntityManager.SetComponentData(entity, componentData);
        //}

        //entities.Dispose();
    }

    protected override void OnUpdate() { }

    /// <summary>
    /// Gets entities with the specified component type, and the PrefabData component type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private NativeArray<Entity> QueryPrefabs<T>() => EntityManager.CreateEntityQuery(ComponentType.ReadWrite<T>(), ComponentType.ReadOnly<PrefabData>()).ToEntityArray(Allocator.Persistent);

    /// <summary>
    /// Returns the prefab name of the specified entity.
    /// </summary>
    /// <param name="prefabSystem"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    private string? GetPrefabName(PrefabSystem prefabSystem, Entity entity)
    {
        if (EntityManager.HasComponent<PrefabData>(entity))
        {
            var prefabData = EntityManager.GetComponentData<PrefabData>(entity);
            if (prefabSystem.TryGetPrefab(prefabData, out PrefabBase prefab))
                return prefab.name;
        }
        return null;
    }

    /// <summary>
    /// Loads default data for the specified component type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TMod"></typeparam>
    /// <param name="prefabSystem"></param>
    /// <param name="data"></param>
    private void LoadDefaultData<T, TMod>(PrefabSystem prefabSystem, Dictionary<string, TMod> data) where T : unmanaged, IComponentData where TMod : struct, ILoadFromGame<T>
    {
        using var entities = QueryPrefabs<T>();
        foreach (var entity in entities)
        {
            var prefabName = GetPrefabName(prefabSystem, entity);
            if (prefabName is null)
            {
                Mod.Log.Error("Failed to get prefab name for entity.");
                continue;
            }
            var componentData = EntityManager.GetComponentData<T>(entity);
            var value = default(TMod);
            value.Load(componentData);
            data[prefabName] = value;
        }
    }
}
