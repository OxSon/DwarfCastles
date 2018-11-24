using System;
using System.IO;
using System.Linq;

namespace DwarfCastles
{
    public class ResourceParser
    {
        public static string ParseFile(string fileName)
        {
            string tempScanStorage = "../../Resources/";
            var file = Directory.GetFiles(tempScanStorage, fileName, SearchOption.AllDirectories)
                .FirstOrDefault();
            if (file == null)
            {
                // The file variable has the *first* occurrence of that filename
                Logger.Log("Didn't find file!");
            }
            else
            {
                Logger.Log("Found file!");
                try
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        string fileContent = reader.ReadToEnd();
                        Console.WriteLine(file);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception caught!");
                    Console.WriteLine(e.Message);
                }
            }

            return "";
        }
    }
}