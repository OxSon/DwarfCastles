using System;
using System.IO;

namespace DwarfCastles
{
    public class Logger
    {
        static Logger()
        {
            try
            {
                using (StreamWriter LogWriter = new StreamWriter("Log.txt"))
                {
                    //Clears the file and ensures a StreamWriter can be created for the file
                }
            }
            catch (Exception e)
            {
                Console.Write("Error creating Log File\n" + e.StackTrace);
            }
        }

        public static void Log(string s)
        {
            try
            {
                using (StreamWriter LogWriter = new StreamWriter("Log.txt", true))
                {
                    LogWriter.WriteLine(s);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to Log File\n" + e.StackTrace);
            }
        }
    }
}