using System;
using System.Collections.Generic;
using System.IO;

namespace DwarfCastles
{
    public static class ResourceMasterList
    {
        private static readonly IDictionary<string, Entity> DefaultEntities;
        
        // TODO Store the dependency names in a paired list, so we know when they have been resolved
        private static IList<string> MissingDependancesStaging; 

        static ResourceMasterList()
        {
            MissingDependancesStaging = new List<string>();
            DefaultEntities = new Dictionary<string, Entity>();
        }

        public static void LoadAllResources()
        {
            Logger.Log("Entering LoadAllResources");
            const string filepath = "Resources/";
            
            Logger.Log(Directory.GetCurrentDirectory());
            
            if (Directory.Exists(filepath)) // This will be used in releases
            {
                Logger.Log($"Found Resource Folder at {filepath}");
                LoadResourceFromDirectory(filepath);
            } else if (Directory.Exists("../../" + filepath)) // This will be used in debugging
            {
                Logger.Log($"Found Resource Folder at {"../../" + filepath}");
                LoadResourceFromDirectory("../../" + filepath);
            }

            var ChangeFound = true;
            while (ChangeFound)
            {
                var previousUnfoundFiles = MissingDependancesStaging;
                MissingDependancesStaging = new List<string>();
                ChangeFound = false;
                var count = DefaultEntities.Count;
                foreach (var s in previousUnfoundFiles)
                {
                    LoadResourceFromFile(s);
                }

                if (count != DefaultEntities.Count)
                {
                    ChangeFound = true;
                }
            }
            
            Logger.Log($"Exiting LoadAllResources with Final Count {DefaultEntities.Count}");
            Logger.Log($"{MissingDependancesStaging.Count} Files were not loaded properly due to dependency issues:");
            Logger.Log("\t" + string.Join("\n\t", MissingDependancesStaging));
        }

        private static void LoadResourceFromDirectory(string path)
        {
            var subFolders = Directory.GetDirectories(path);
            foreach (var subPath in subFolders)
            {
                LoadResourceFromDirectory(subPath);
            }

            foreach (var s in Directory.GetFiles(path))
            {
                LoadResourceFromFile(s);
            }
        }

        private static void LoadResourceFromFile(string path)
        {
            try
            {
                var e = ResourceParser.ParseFile(path);
                if (e.Name == "Inheritance Missing")
                {
                    MissingDependancesStaging.Add(path);
                }
                else
                {
                    DefaultEntities.Add(e.Name, e);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error Occured while parsing file {path}\n{e.Message}");
            }
        }

        public static Entity GetDefaultClone(string name)
        {
            return GetDefault(name).Clone();
        }

        public static Entity GetDefault(string name)
        {
            return DefaultEntities.TryGetValue(name, out var output) ? output : null;
        }

        public static IEnumerable<Entity> GetAllEntities()
        {
            return DefaultEntities.Values;
        }
    }
}