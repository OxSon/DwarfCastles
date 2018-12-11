using System.Collections.Generic;

namespace DwarfCastles
{
    public static class DefaultQueries
    {
        public static IDictionary<char, string> GetAllBuildableEntities()
        {
            IDictionary<char, string> buildables = new Dictionary<char, string>();

            char c = 'a';
            foreach (var e in ResourceMasterList.GetAllEntities())
            {
                var buildable = e.GetTag("buildable");
                if (buildable != null)
                {
                    buildables.Add(c++, e.Name);
                }
            }

            return buildables;
        }

        public static IDictionary<char, string> GetAllCraftingStations()
        {
            IDictionary<char, string> stations = new Dictionary<char, string> {{'a', "none"}};

            char c = 'b';
            foreach (var e in ResourceMasterList.GetAllEntities())
            {
                Logger.Log($"Attempting to check if {e.Name} is craftable with a station");
                var station = e.GetTag("craftable.station");
                if (station != null)
                {
                    Logger.Log($"{e.Name} found to be craftable at {station.Value.GetString()}");
                    if (stations.Values.Contains(station.Value.GetString()))
                        continue;
                    stations.Add(c++, station.Value.GetString());
                }
            }

            return stations;
        }

        public static IDictionary<char, string> GetItemsCraftableAt(string stationName)
        {
            IDictionary<char, string> items = new Dictionary<char, string>();

            char c = 'a';
            foreach (var e in ResourceMasterList.GetAllEntities())
            {
                var station = e.GetTag("craftable.station");
                if (station != null)
                {
                    if (station.Value.GetString() == stationName)
                    {
                        items.Add(c++, e.Name);
                    }
                }
            }

            return items;
        }
    }
}