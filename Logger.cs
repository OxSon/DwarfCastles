using System;
using System.IO;

namespace DwarfCastles
{
    public static class Logger
    {
        public const bool DoLogging = false;
        static Logger()
        {
            try
            {
                using (var LogWriter = new StreamWriter("Log.txt"))
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
            if (!DoLogging)
            {
                return;
            }
            try
            {
                using (var LogWriter = new StreamWriter("Log.txt", true))
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