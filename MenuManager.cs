using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DwarfCastles.Jobs;

namespace DwarfCastles
{
    public class MenuManager
    {
        // -1 is a return state that will reset the menu after a key is pressed
        // 0 is a selection context menu
        // 1 is a position selection menu
        // 2 is a rectangle selection menu
        public int State;

        private string Info;

        private IDictionary<char, string> CurrentMenuContext;
        private Action<string> CurrentMenuActionHandler;

        // Used in Rectangle Selection
        public Point FirstPoint;
        public Action<Point> SetPointAction;
        private string StoredValue;

        public Map Map;
        public GameManager Manager;

        public MenuManager()
        {
            ResetMenu();
        }

        public void SetManager(GameManager manager)
        {
            Manager = manager;
            Map = manager.Map;
        }

        public void HandleMenuCommand(char key)
        {
            if (State == -1)
            {
                ResetMenu();
                return;
            }

            if (CurrentMenuContext.TryGetValue(key, out string value))
                CurrentMenuActionHandler(value);
        }

        public void ResetMenu()
        {
            IDictionary<char, string> Actions = new Dictionary<char, string>();
            Actions.Add('b', "Build");
            Actions.Add('c', "Craft");
            Actions.Add('h', "Harvest");
            Actions.Add('i', "Info");
            Info = "Please Select an Action to do:";
            CurrentMenuContext = Actions;
            CurrentMenuActionHandler = HandleMainAction;
            State = 0;
            StoredValue = "";
            FirstPoint = Point.Empty;
        }

        public void HandleMainAction(string Selection)
        {
            switch (Selection)
            {
                case "Build":
                    Info = "Please select what building you would like to place:";
                    CurrentMenuContext = GetAllBuildableEntities();
                    CurrentMenuActionHandler = HandleBuildAction;
                    break;
                case "Craft":
                    Info = "Please select what crafting station you want to craft at:";
                    CurrentMenuContext = GetAllCraftingStations();
                    CurrentMenuActionHandler = HandleCraftAction;
                    break;
                case "Harvest":
                    Info = "Please select where you would like to Harvest (First point)";
                    State = 1;
                    CurrentMenuContext = new Dictionary<char, string>();
                    SetPointAction = HandleStartHarvestAction;
                    break;
                case "Info":
                    Info = "Please select where you would like information about";
                    State = 1;
                    CurrentMenuContext = new Dictionary<char, string>();
                    SetPointAction = HandleFinishInfoAction;
                    break;
            }
        }

        public void HandleBuildAction(string selectedObject)
        {
            Info = $"Please select where you want to build {selectedObject}";
            var buildable = ResourceMasterList.GetDefault(selectedObject).GetTag("buildable");
            if (buildable.GetTag("resources") != null)
            {
                var resources = buildable.GetTag("resources");
                foreach (var r in resources.SubTags)
                {
                    Info += r.ToString() + '\n';
                }
            }

            CurrentMenuContext = new Dictionary<char, string>();
            State = 1;
            StoredValue = selectedObject;
            SetPointAction = FinishBuildAction;
        }

        public void FinishBuildAction(Point p)
        {
            Manager.AddTask(new Build(p, StoredValue));
            ResetMenu();
        }

        public void HandleCraftAction(string stationName)
        {
            Info = "Please select what item you want to craft at the station:";
            CurrentMenuContext = GetItemsCraftableAt(stationName);
            CurrentMenuActionHandler = HandleStationSelection;
        }

        public void HandleStationSelection(string itemName)
        {
            ResetMenu();
        }

        public void HandleStartHarvestAction(Point p)
        {
            Info = "Please select where the other point of the rectangle goes";
            FirstPoint = p;
            State = 2;
            SetPointAction = HandleFinishHarvestAction;
        }

        public void HandleFinishHarvestAction(Point p)
        {
            var r = FixedRectangle(FirstPoint, p);
            Logger.Log($"Harvesting everything between {FirstPoint.X}, {FirstPoint.Y} and {p.X}, {p.Y}");
            foreach (var e in Map.Entities)
            {
                if (Map.Within(e.Pos, r))
                {
                    if (e.GetTag("harvestable") != null)
                    {
                        Manager.AddTask(new Harvest(e));
                    }
                }
            }

            ResetMenu();
        }

        public void HandleFinishInfoAction(Point p)
        {
            var ents = Map.GetEntitiesByLocation(p);
            Info = string.Join("; ", ents.Select(e => $"{e.Ascii}: {e.Name}"));
            State = -1;
            
            Logger.Log($"Found {Info}\n");
        }

        public static Rectangle FixedRectangle(Point p1, Point p2)
        {
            var minX = Math.Min(p1.X, p2.X);
            var minY = Math.Min(p1.Y, p2.Y);
            var Width = Math.Max(p1.X, p2.X) - minX;
            var Height = Math.Max(p1.Y, p2.Y) - minY;
            return new Rectangle(minX, minY, Width, Height);
        }


        public string GetMenuDisplay()
        {
            string s = $"{Info}\n{string.Join("\n", CurrentMenuContext.Select(x => $"{x.Key} {x.Value}"))}";
            Logger.Log(s);
            return s;
        }

        private IDictionary<char, string> GetAllBuildableEntities()
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

        private IDictionary<char, string> GetAllCraftingStations()
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

        private IDictionary<char, string> GetItemsCraftableAt(string stationName)
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