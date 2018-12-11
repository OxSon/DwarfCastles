using System;
using System.Collections.Generic;
using System.IO;
using Json;

namespace DwarfCastles
{
    public static class ResourceParser
    {
        public static Entity ParseFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                Logger.Log($"Couldn't find file {fileName}!");
            }
            else
            {
                Logger.Log($"Found file {fileName}, loading contents.");
                try
                {
                    using (var reader = new StreamReader(fileName))
                    {
                        var fileContent = reader.ReadToEnd();
                        var json = JsonParser.FromJson(fileContent);
                        return BuildFromJSON(json);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log($"Exception while reading in {fileName}\n{e.Message}");
                }
            }

            return null;
        }

        private static Entity BuildFromJSON(IDictionary<string, object> json)
        {
            Entity e;

            var Attributes = (IDictionary<string, object>) json["attributes"];
            Logger.Log($"Found base Object in file with subobjects of {string.Join(",", json.Keys)}");
            Logger.Log("Found Attributes in file: " + string.Join(", ", Attributes.Keys));

            switch (Attributes.TryGetValue("class", out var output) ? output : "")
            {
                case "entity":
                    Logger.Log("Found class to be Entity");
                    e = new Entity();
                    break;
                case "actor":
                    Logger.Log("Found class to be Actor");
                    e = new Actor();
                    break;
                default:
                    Logger.Log("Could not find class definition in attributes, using entity as default");
                    e = new Entity();
                    break;
            }

            if (json.TryGetValue("inheritance", out var inheritanceOut))
            {
                Logger.Log(inheritanceOut.GetType().ToString());
                var InheritanceArray = (List<object>) inheritanceOut;
                
                Logger.Log($"Entering inheritance with {InheritanceArray.Count} elements");
                foreach (var o in InheritanceArray)
                {
                    var s = (string) o;
                    
                    var defaultE = ResourceMasterList.GetDefault(s);
                    if (defaultE == null)
                    {
                        return new Entity {Name = "Inheritance Missing"};
                    }
                    
                    e.Inherit(defaultE);
                }
            }


            if (Attributes["name"] != null)
            {
                e.Name = (string) Attributes["name"];
            }

            if (Attributes["ascii"] != null)
            {
                e.Ascii = ((string) Attributes["Ascii"])[0];
            }

            if (Attributes["display"] != null)
            {
                e.Display = (string) Attributes["display"];
            }

            Enum.TryParse((string) Attributes["backgroundcolor"], true, out ConsoleColor c);
            e.BackgroundColor = c;
            
            Enum.TryParse((string) Attributes["foregroundcolor"], true, out c);
            e.ForegroundColor = c;

            // Set up all tags, regardless of their use in fields
            foreach (var o in json)
            {
                Logger.Log($"Trying ParseTag on tag {o.Key}");
                e.AddTag(ParseTag(o));
            }

            Logger.Log(e.ToString());
            return e;
        }

        private static Tag ParseTag(KeyValuePair<string, object> json)
        {
            var tag = new Tag(json.Key);
            Logger.Log($"Tag {json.Key} has a value type of {json.Value.GetType()} and value of {json.Value}");
            var type = json.Value.GetType();
            if (type == typeof(Dictionary<string, object>))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>) json.Value;
                foreach (var kvp in dict)
                {
                    tag.AddTag(ParseTag(kvp));
                }
            }
            else if (type == typeof(List<object>))
            {
                var array = (List<object>) json.Value;

                foreach (var o in array)
                {
                    if (o.GetType() == typeof(Dictionary<string, object>))
                    {
                        Logger.Log("Hit Dictionary inside list");
                        var temp = ParseTag(new KeyValuePair<string, object>("", o));
                        tag.AddTag(temp);
                        Logger.Log($"End Tag created from dictionary was {temp}");
                    }
                    else
                    {
                        tag.AddArrayValue(new Value(o));
                    }
                }
            }
            else
            {
                tag.Value.SetValue(json.Value);
            }

            return tag;
        }
    }
}