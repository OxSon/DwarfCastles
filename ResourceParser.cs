using System;
using System.Collections.Generic;
using System.IO;
using Json;

namespace DwarfCastles
{
    public class ResourceParser
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
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string fileContent = reader.ReadToEnd();
                        IDictionary<string, object> json = JsonParser.FromJson(fileContent);
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

        public static Entity BuildFromJSON(IDictionary<string, object> json)
        {
            Entity e;

            IDictionary<string, object> Attributes = (IDictionary<string, object>) json["attributes"];

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
                List<object> InheritanceArray = (List<object>) inheritanceOut;
                
                Logger.Log($"Entering inheritance with {InheritanceArray.Count} elements");
                foreach (object o in InheritanceArray)
                {
                    string s = (string) o;
                    Entity defaultE = ResourceMasterList.GetDefault(s);
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

            ConsoleColor c;
            ConsoleColor.TryParse((string) Attributes["backgroundcolor"], true, out c);
            e.BackgroundColor = c;
            ConsoleColor.TryParse((string) Attributes["foregroundcolor"], true, out c);
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

        public static Tag ParseTag(KeyValuePair<string, object> json)
        {
            Tag tag = new Tag(json.Key);
            Logger.Log($"Tag {json.Key} has a value type of {json.Value.GetType()} and value of {json.Value}");
            Type type = json.Value.GetType();
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
                List<object> array = (List<object>) json.Value;

                foreach (var o in array)
                {
                    if (o.GetType() == typeof(Dictionary<string, object>))
                    {
                        Logger.Log("Hit Dictionary inside list");
                        Tag temp = ParseTag(new KeyValuePair<string, object>("", o));
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
                tag.Value.setValue(json.Value);
            }

            return tag;
        }
    }
}