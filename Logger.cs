using System;
using System.Collections.Concurrent;
using System.IO;

namespace DwarfCastles
{
    public static class Logger
    {
        public const int ERROR = 1;
        public const int WARNING = 2;
        public const int DEBUG = 3;
        public const int VERBOSE = 4;
        
        private const int Verbosity = 4;

        private static ConcurrentQueue<string> OutputQueue;
        
        static Logger()
        {
            OutputQueue = new ConcurrentQueue<string>();
            File.Delete("Log.txt");
        }

        public static void Log(string s, int VerboseLevel = 1)
        {
            if (VerboseLevel > Verbosity)
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