using System;
using System.Collections.Concurrent;
using System.IO;

namespace DwarfCastles
{
    public static class Logger
    {
        public const bool DoLogging = true;

        private static ConcurrentQueue<string> NextOutput;
        
        static Logger()
        {
            NextOutput = new ConcurrentQueue<string>();
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
            NextOutput.Enqueue(s);
            
            try
            {
                using (var LogWriter = new StreamWriter("Log.txt", true))
                {
                    while (!NextOutput.IsEmpty)
                    {
                        LogWriter.WriteLine(NextOutput.TryDequeue(out var temp) ? temp: "");
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}