using Colossal.IO.AssetDatabase;
using Game.Modding;
using System.Collections.Generic;
using Colossal;
using Game.Settings;
using Colossal.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using static Colossal.IO.AssetDatabase.SettingAsset;
using Unity.Entities;
using System.Linq;

namespace PathfindingCosts;

[FileLocation(nameof(PathfindingCosts))]
public class Setting : ModSetting
{
    [SettingsUIHidden]
    public string Warning1 { get; set; } = "";
    [SettingsUIHidden]
    public string Warning2 { get; set; } = "";
    [SettingsUIHidden]
    public string ReadMe1 { get; set; } = "";
    [SettingsUIHidden]
    public string ReadMe2 { get; set; } = "";
    [SettingsUIHidden]
    public string ReadMe3 { get; set; } = "";
    // These dictionaries store pathfinding costs for different presets, by preset name.
    [SettingsUIHidden]
    public Dictionary<string, PathfindCarData> DefaultCarData { get; set; } = new(); // Cars
    [SettingsUIHidden]
    public Dictionary<string, PathfindPedestrianData> DefaultPedestrianData { get; set; } = new(); // Pedestrians
    [SettingsUIHidden]
    public Dictionary<string, PathfindTrackData> DefaultTrackData { get; set; } = new(); // trains?
    [SettingsUIHidden]
    public Dictionary<string, PathfindCarData> CarData { get; set; } = new(); // Cars
    [SettingsUIHidden]
    public Dictionary<string, PathfindPedestrianData> PedestrianData { get; set; } = new(); // Pedestrians
    [SettingsUIHidden]
    public Dictionary<string, PathfindTrackData> TrackData { get; set; } = new(); // trains?

    [Exclude]
    [SettingsUIHidden]
    private string _loadFrom = "";
    [SettingsUIDirectoryPicker]
    [Exclude]
    public string LoadFrom
    {
        get => this._loadFrom;
        set
        {
            this._loadFrom = value;
            LoadSettingsFrom(this._loadFrom);
        }
    }

    [Exclude]
    [SettingsUIHidden]
    private string _saveTo = "";
    [SettingsUIDirectoryPicker]
    [Exclude]
    public string SaveTo
    {
        get => this._saveTo;
        set
        {
            this._saveTo = value;
            SaveSettingsTo(this._saveTo);
        }
    }

    [SettingsUIButton]
    [Exclude]
    public bool Edit
    {
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        set => EditSettings();
    }

    //[SettingsUISection("Reload settings")]
    [SettingsUIButton]
    [Exclude]
    public bool Reload
    {
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        set => ReloadSettings();
    }

    [SettingsUIConfirmation]
    [SettingsUIButton]
    [Exclude]
    public bool Reset
    {
        [SuppressMessage("ReSharper", "ValueParameterNotUsed")]
        set => SetDefaults();
    }

    [SettingsUIHidden]
    [Exclude]
    private readonly IMod _mod;
    public Setting(IMod mod) : base(mod)
    {
        this._mod = mod;
    }

    public override void Apply()
    {
        Warning1 = "Default data values are ignored, overriden with each run and exists here only for your convenience,";
        Warning2 = "so you would be able to see all possible pathfinding costs and their default values.";
        ReadMe1 = "To modify pathfinding costs, define values with names 'CarData', 'PedestrianData' and 'TrackData' in this config,";
        ReadMe2 = "with the same structure as default values. You don't have to copy entire default values dictionary, just the values you want to change.";
        ReadMe3 = "Don't forget to set all costs inside each prefab to either null or custom values, otherwise file will not load.";
        base.Apply();
        SaveToFile(null);
        foreach(var world in World.All)
            world.GetExistingSystemManaged<UpdateDataSystem>()?.UpdateCosts();
    }

    public override void SetDefaults()
    {
        CarData = new();
        PedestrianData = new();
        TrackData = new();
        Apply();
    }

    public void ReloadSettings()
    {
        var userDatabase = AssetDatabase.global.databases.OfType<AssetDatabase<User>>().First(x => x.name == "User");
        var dataSource = (FileSystemDataSource)userDatabase.dataSource;
        LoadFromFile(Path.Combine(dataSource.rootPath, nameof(PathfindingCosts) + ".coc"));
        Apply();
    }

    private void LoadSettingsFrom(string path)
    {
        path = Path.Combine(path, nameof(PathfindingCosts) + ".json");
        if(!File.Exists(path))
        {
            log.Error($"File {path} does not exist.");
            return;
        }
        LoadFromFile(path);
        Apply();
    }

    private void EditSettings()
    {
        
    }

    private void SaveSettingsTo(string path)
    {
        SaveToFile(Path.Combine(path, nameof(PathfindingCosts) + ".json"));
    }
    private async void SaveToFile(string? path)
    {
        await TaskManager.instance.EnqueueTask(nameof(SaveToFile), async () =>
        {
            SaveSettingsHelper helper = new SaveSettingsHelper(path);
            try
            {
                foreach (var asset in AssetDatabase.global.GetAssets<SettingAsset>())
                    foreach (var fragment in asset)
                        if (fragment.source is Setting)
                            await SaveAsset(asset, true, false, helper);
            }
            catch(Exception e)
            {
                log.Error(e.ToString());
            }
            finally
            {
                helper.Dispose();
            }
        });
    }

    private void LoadFromFile(string path)
    {
        string source;
        using (StreamReader streamReader = new StreamReader(path))
        {
            source = streamReader.ReadToEnd();
        }
        var parser = new COCParser();
        Dictionary<string, COCParser.ObjectBlock> dictionary = parser.Parse(source, out var outReport);
        foreach (string message in outReport)
            log.Warn(message);
        foreach (KeyValuePair<string, COCParser.ObjectBlock> keyValuePair in dictionary)
        {
            var variant = JSON.Load(source.Substring(keyValuePair.Value.startIndex, keyValuePair.Value.length));
            Setting result = new(this._mod);
            JSON.WriteInto(variant, ref result);
            CarData = result.CarData;
            PedestrianData = result.PedestrianData;
            TrackData = result.TrackData;
        }
    }

    private async Task SaveAsset(SettingAsset @this, bool saveAll, bool cleanupSettings, SaveSettingsHelper helper)
    {
        if (helper == null)
            throw new NullReferenceException(nameof(helper));
        foreach (Fragment settingFragment in @this)
        {
            Fragment fragment = settingFragment;
            if (fragment.asset.database != AssetDatabase.game)
            {
                 string? jsonObject = fragment.source == null
                    ? fragment.variant?.ToJSON()
                    : (saveAll
                        ? JSON.Dump(fragment.source)
                        : DiffUtility.Diff(fragment.source, fragment.@default, cleanupSettings
                            ? null
                            : fragment.variant)?.ToJSON());
                if (!IsEmptyJson(jsonObject))
                {
                    await helper.WriteAsync(fragment, @this.name);
                    await helper.WriteAsync(fragment, jsonObject);
                }
                else
                {
                    await helper.WriteAsync(fragment, null);
                }
            }
        }
    }
    private bool IsEmptyJson(string? jsonString) => string.IsNullOrEmpty(jsonString) || TrimJson(jsonString!) == "{}";

    private static string TrimJson(string input)
    {
        char[] chArray = new char[input.Length];
        int length = 0;
        foreach (char ch in input)
            switch (ch)
            {
                case '\t':
                case '\n':
                case ' ':
                    continue;
                default:
                    chArray[length++] = ch;
                    continue;
            }
        return new string(chArray, 0, length);
    }
}

public class SaveSettingsHelper : IDisposable
{
    private readonly Dictionary<Fragment, TextWriter?> _writersMap = new();
    private string? _path;
    public SaveSettingsHelper(string? path)
    {
        this._path = path;
    }
    public void Write(Fragment fragment, string? line)
    {
        if (line != null)
        {
            TextWriter writeStream = GetWriteStream(fragment);
            writeStream.WriteLine(line);
            this._writersMap[fragment] = writeStream;
        }
        else
        {
            this._writersMap.Add(fragment, null);
        }
    }

    public async Task WriteAsync(Fragment fragment, string? line)
    {
        if (line != null)
            await GetWriteStream(fragment).WriteLineAsync(line);
        else
            this._writersMap.Add(fragment, null);
    }

    private TextWriter GetWriteStream(Fragment fragment)
    {
        if (!this._writersMap.TryGetValue(fragment, out TextWriter? writeStream) || writeStream == null)
        {
            if (this._path is null)
                writeStream = new StreamWriter(fragment.asset.database.GetWriteStream(fragment.guid));
            else
                writeStream = new StreamWriter(this._path);
            this._writersMap[fragment] = writeStream;
        }
        return writeStream;
    }

    public void Dispose()
    {
        foreach (KeyValuePair<Fragment, TextWriter?> writers in this._writersMap)
            if (writers.Value != null)
                writers.Value.Dispose();
    }
}

public class LocaleEN : IDictionarySource
{
    private readonly Setting _setting;
    public LocaleEN(Setting setting) => this._setting = setting;
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) =>
        new Dictionary<string, string>
        {
            { this._setting.GetSettingsLocaleID(), "Pathfinding costs" },
            { this._setting.GetOptionLabelLocaleID(nameof(Setting.Reload)), "Reload settings" },
            { this._setting.GetOptionDescLocaleID(nameof(Setting.Reload)), $"Reloads and applies settings from config file." },
            { this._setting.GetOptionLabelLocaleID(nameof(Setting.Reset)), "Reset settings" },
            { this._setting.GetOptionDescLocaleID(nameof(Setting.Reset)), $"Reset settings by removing all user defined pathfinding costs. Be careful, this will delete all your changes!" },
            { this._setting.GetOptionLabelLocaleID(nameof(Setting.LoadFrom)), "Load from..." },
            { this._setting.GetOptionDescLocaleID(nameof(Setting.LoadFrom)), $"Load settings from chosen file. Be careful, this will override all your changes!" },
            { this._setting.GetOptionLabelLocaleID(nameof(Setting.SaveTo)), "Save to..." },
            { this._setting.GetOptionDescLocaleID(nameof(Setting.SaveTo)), $"Save settings to chosen file." },
            { this._setting.GetOptionLabelLocaleID(nameof(Setting.Edit)), "Edit settings" },
            { this._setting.GetOptionDescLocaleID(nameof(Setting.Edit)), $"Opens settings file for editing." },
            { this._setting.GetOptionWarningLocaleID(nameof(Setting.Reset)), "Are you sure you want to reset mod settings? This will delete all your changes in settings file and cannot be reverted!" },
        };

    public void Unload()
    {

    }
}