using System.Collections;
using System.Collections.Generic;
using DwarfFortress;

namespace DwarfCastles
{
    public static class ResourceMasterList
    {
        private static IDictionary<string, Entity> DefaultEntities;

        static ResourceMasterList()
        {
            DefaultEntities = new Dictionary<string, Entity>();
            LoadAllResources();
        }

        private static void LoadAllResources()
        {
            
        }

        public static Entity GetDefaultClone(string name)
        {
            return GetDefault(name).Clone();
        }

        public static Entity GetDefault(string name)
        {
            return DefaultEntities[name];
        }
    }
}