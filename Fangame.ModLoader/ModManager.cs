using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

namespace Fangame.ModLoader;

public class ModManager
{
    public string ModsDirectory;
    Dictionary<string, ModAssembly> Assemblies;

    public ModManager(string modsDirectory)
    {
        ModsDirectory = modsDirectory;
        if (!Directory.Exists(modsDirectory))
        {
            Directory.CreateDirectory(modsDirectory);
        }

        Assemblies = [];
        foreach (var modDirectory in Directory.EnumerateDirectories(modsDirectory))
        {
            string modName = Path.GetFileName(modDirectory);
            Assemblies[modName] = new ModAssembly(modDirectory);
        }
    }

    public Mod? CreateInstance(string modName)
    {
        if (Assemblies.TryGetValue(modName, out var assembly))
        {
            return assembly.CreateInstance();
        }
        return null;
    }

    public void SaveConfig(string modName)
    {
        if (Assemblies.TryGetValue(modName, out var assembly))
        {
            assembly.SaveConfig();
        }
    }

    public ModConfig? GetConfig(string modName)
    {
        if (Assemblies.TryGetValue(modName, out var assembly))
        {
            return assembly.GetConfig();
        }
        return null;
    }

    public string[] GetModNames()
    {
        return Assemblies.Keys.ToArray();
    }

    class ModAssembly
    {
        string Name;
        string ConfigPath;
        AssemblyLoadContext LoadContext;
        Assembly Assembly;
        Type? ModType;
        Type? ConfigType;
        ModConfig? Config;
        static JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        public ModAssembly(string modDirectory)
        {
            Name = Path.GetFileName(modDirectory);
            LoadContext = new AssemblyLoadContext(modDirectory, true);
            LoadContext.Resolving += (loadContext, assemblyName) =>
            {
                return loadContext.LoadFromAssemblyPath(Path.Combine(modDirectory, $"{assemblyName.Name}.dll"));
            };
            Assembly = LoadContext.LoadFromAssemblyPath(Path.Combine(modDirectory, $"{Name}.dll"));
            foreach (var type in Assembly.GetTypes())
            {
                if (typeof(Mod).IsAssignableFrom(type))
                {
                    ModType = type;
                }
                if (typeof(ModConfig).IsAssignableFrom(type))
                {
                    ConfigType = type;
                }
            }
            ConfigPath = Path.Combine(modDirectory, "Config.json");
            if (ConfigType != null)
            {
                if (File.Exists(ConfigPath))
                {
                    Config = (ModConfig?)JsonSerializer.Deserialize(File.ReadAllText(ConfigPath), ConfigType);
                }
                else
                {
                    Config = (ModConfig?)Activator.CreateInstance(ConfigType);
                }
                SaveConfig();
            }
        }

        public Mod? CreateInstance()
        {
            if (ModType != null)
            {
                return (Mod?)Activator.CreateInstance(ModType);
            }
            return null;
        }

        public ModConfig? GetConfig()
        {
            return Config;
        }

        public void SaveConfig()
        {
            if (ConfigType != null && Config != null)
            {
                using var fs = File.Create(ConfigPath);
                JsonSerializer.Serialize(fs, Config, ConfigType, JsonSerializerOptions);
            }
        }
    }
}

