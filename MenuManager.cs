using System.Collections.Generic;

namespace DwarfCastles
{
    public class MenuManager
    {
        private IDictionary<string, string> GetActionList()
        {
            IDictionary<string, string> Actions = new Dictionary<string, string>();
            Actions.Add("b", "Build");
            Actions.Add("c", "Craft");
            Actions.Add("h", "Harvest");
            Actions.Add("i", "info");
            
            return Actions;
        }

        private IEnumerable<string> GetAllCraftingStations()
        {
            IList<string> stations = new List<string>{"none"};
            foreach (var e in ResourceMasterList.GetAllEntities())
            {
                var station = e.GetTag("Craftable.station");
                if (station != null)
                {
                    if (stations.Contains(station.Value.GetString()))
                        continue;
                    stations.Add(station.Value.GetString());
                }
            }

            return stations;
        }

        private IEnumerable<string> GetItemsCraftableAt(string stationName)
        {
            IList<string> items = new List<string>();
            foreach (var e in ResourceMasterList.GetAllEntities())
            {
                var station = e.GetTag("craftable.station");
                if (station != null)
                {
                    if (station.Value.GetString() == stationName)
                    {
                        items.Add(e.Name);
                    }
                }
            }

            return items;
        }
    }
}