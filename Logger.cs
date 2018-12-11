using System;
using System.Collections.Concurrent;
using System.IO;

namespace DwarfCastles
{
    public static class Logger
    {
        private const int Verboseity = 1;

        private static ConcurrentQueue<string> OutputQueue;
        
        static Logger()
        {
            OutputQueue = new ConcurrentQueue<string>();
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

        public static void Log(string s, int VerboseLevel = 1)
        {
            if (VerboseLevel < Verboseity)
            {
                return;
            }
            OutputQueue.Enqueue(s);
            
            try
            {
                using (var LogWriter = new StreamWriter("Log.txt", true))
                {
                    while (!OutputQueue.IsEmpty)
                    {
                        LogWriter.WriteLine(OutputQueue.TryDequeue(out var temp) ? temp: "");
                    }
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}