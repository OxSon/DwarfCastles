using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DwarfCastles
{
    public static class ResourceMasterList
    {
        private static IDictionary<string, Entity> DefaultEntities;

        private static IList<string> MissingDependancesStaging;

        static ResourceMasterList()
        {
            MissingDependancesStaging = new List<string>();
            DefaultEntities = new Dictionary<string, Entity>();
        }

        public static void LoadAllResources()
        {
            Logger.Log("Entering LoadAllResources");
            string filepath = "Resources/";
            
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

            bool ChangeFound = true;
            while (ChangeFound)
            {
                IList<string> previousUnfoundFiles = MissingDependancesStaging;
                MissingDependancesStaging = new List<string>();
                ChangeFound = false;
                int count = DefaultEntities.Count;
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

        public static void LoadResourceFromDirectory(string path)
        {
            string[] subFolders = Directory.GetDirectories(path);
            foreach (var subPath in subFolders)
            {
                LoadResourceFromDirectory(subPath);
            }

            foreach (string s in Directory.GetFiles(path))
            {
                LoadResourceFromFile(s);
            }
        }

        public static void LoadResourceFromFile(string path)
        {
            try
            {
                Entity e = ResourceParser.ParseFile(path);
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
    }
}