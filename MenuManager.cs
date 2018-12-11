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

        public MenuManager()
        {
            ResetMenu();
        }

        public void HandleMenuCommand(char key)
        {
            if (State == -1)
            {
                ResetMenu();
                return;
            }

            if (CurrentMenuContext.TryGetValue(key, out var value))
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

        private void HandleMainAction(string Selection)
        {
            switch (Selection)
            {
                case "Build":
                    Info = "Please select what building you would like to place:";
                    CurrentMenuContext = DefaultQueries.GetAllBuildableEntities();
                    CurrentMenuActionHandler = HandleBuildAction;
                    break;
                case "Craft":
                    Info = "Please select what crafting station you want to craft at:";
                    CurrentMenuContext = DefaultQueries.GetAllCraftingStations();
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
                default:
                    State = -1;
                    break;
            }
        }

        private void HandleBuildAction(string selectedObject)
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

        private void FinishBuildAction(Point p)
        {
            GameManager.ActiveMap.AddTask(new Build(p, StoredValue));
            ResetMenu();
        }

        private void HandleCraftAction(string stationName)
        {
            Info = "Please select what item you want to craft at the station:";
            CurrentMenuContext = DefaultQueries.GetItemsCraftableAt(stationName);
            CurrentMenuActionHandler = HandleStationSelection;
        }

        private void HandleStationSelection(string itemName)
        {
            GameManager.ActiveMap.AddTask(new Craft(itemName));
            ResetMenu();
        }

        private void HandleStartHarvestAction(Point p)
        {
            Info = "Please select where the other point of the rectangle goes";
            FirstPoint = p;
            State = 2;
            SetPointAction = HandleFinishHarvestAction;
        }

        private void HandleFinishHarvestAction(Point p)
        {
            var r = FixedRectangle(FirstPoint, p);
            Logger.Log($"Harvesting everything between {FirstPoint.X}, {FirstPoint.Y} and {p.X}, {p.Y}");
            foreach (var e in GameManager.ActiveMap.Entities)
            {
                if (Map.Within(e.Pos, r))
                {
                    if (e.GetTag("harvestable") != null)
                    {
                        GameManager.ActiveMap.AddTask(new Harvest(e));
                    }
                }
            }

            ResetMenu();
        }

        private void HandleFinishInfoAction(Point p)
        {
            var ents = GameManager.ActiveMap.GetEntitiesByLocation(p);
            Info = string.Join("; ", ents.Select(e => $"{e.Ascii}: {e.Name}"));
            State = -1;
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
            return $"{Info}\n{string.Join("\n", CurrentMenuContext.Select(x => $"{x.Key} {x.Value}"))}";
        }
    }
}