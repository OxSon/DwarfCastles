// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using Json;
//
// namespace DwarfCastles
// {
//     public class ResourceParser
//     {
//         public static Entity ParseFile(string fileName)
//         {
//             string tempScanString = "../../Resources/"; // TODO fix for release
//             var file = Directory.GetFiles(tempScanString, fileName, SearchOption.AllDirectories).FirstOrDefault();
//             if (file == null)
//             {
//                 Logger.Log($"Couldn't find file {fileName}!");
//             }
//             else
//             {
//                 Logger.Log($"Found file {fileName}, loading contents.");
//                 try
//                 {
//                     using (StreamReader reader = new StreamReader(file))
//                     {
//                         string fileContent = reader.ReadToEnd();
//                         IDictionary<string, object> json = JsonParser.FromJson(fileContent);
//                         return BuildFromJSON(json);
//                     }
//                 }
//                 catch (Exception e)
//                 {
//                     Logger.Log($"Exception while reading in {fileName}\n{e.Message}");
//                 }
//             }
//
//             return null;
//         }
//
//         public static Entity BuildFromJSON(IDictionary<string, object> json)
//         {
//             Entity e;
//
//             IDictionary<string, object> Attributes = (IDictionary<string, object>) json["attributes"];
//
//             Logger.Log("Found Attributes in file: " + string.Join(", ", Attributes.Keys));
//
//             switch (Attributes["class"])
//             {
//                 case "entity":
//                     e = new Entity();
//                     break;
//                 case "actor":
//                     e = new Actor();
//                     break;
//                 default:
//                     e = new Entity();
//                     break;
//             }
//
//             if (json.TryGetValue("inheritance", out var inheritanceArray))
//             {
//                 foreach (string s in (IEnumerable<string>) inheritanceArray)
//                 {
//                     Entity defaultE = ResourceMasterList.GetDefault(s);
//                     if (defaultE == null)
//                     {
//                         return new Entity();
//                     }
//
//                     e.Inherit(defaultE);
//                 }
//             }
//
//
//             if (Attributes["name"] != null)
//             {
//                 e.Name = (string) Attributes["name"];
//             }
//
//             if (Attributes["ascii"] != null)
//             {
//                 e.Ascii = ((string) Attributes["Ascii"])[0];
//             }
//
//             if (Attributes["display"] != null)
//             {
//                 e.Display = (string) Attributes["display"];
//             }
//
//             ConsoleColor c;
//             ConsoleColor.TryParse((string) Attributes["backgroundcolor"], true, out c);
//             e.BackgroundColor = c;
//             ConsoleColor.TryParse((string) Attributes["foregroundcolor"], true, out c);
//             e.ForegroundColor = c;
//
//             // Set up all tags, regardless of their use in fields
//             foreach (var o in json)
//             {
//                 Logger.Log($"Trying ParseTag on tag {o.Key}");
//                 e.Tags.Add(ParseTag(o));
//             }
//
//
//             //Logger.Log(string.Join(",", json.Keys));
//             Logger.Log(e.ToString());
//             return e;
//         }
//
//         public static Tag ParseTag(KeyValuePair<string, object> json)
//         {
//             Tag tag = new Tag(json.Key);
//             Logger.Log($"Tag {json.Key} has a type of {json.Value.GetType()}");
//             Type type = json.Value.GetType();
//             if (type == typeof(Dictionary<string, object>))
//             {
//                 Dictionary<string, object> dict = (Dictionary<string, object>) json.Value;
//                 foreach (var kvp in dict)
//                 {
//                     tag.AddTag(ParseTag(kvp));
//                 }
//             }
//             else if (type == typeof(List<object>))
//             {
//                 List<object> array = (List<object>)json.Value;
//                 foreach (var o in array)
//                 {
//                     tag.AddArrayValue(new Value(o));
//                 }
//             }
//             else
//             {
//                 tag.Value.setValue(json.Value);
//             }
//
//             return tag;
//         }
//     }
// }