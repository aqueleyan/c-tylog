using System;

namespace Logger
{
    public class Program
    {
        public static void Main()
        {
            Logger.Configure(LogLevel.Warning, "Test", true);
            Logger.Instance.Debug("Debug message.");
            Logger.Instance.Info("Info message.");
            Logger.Instance.Warning("Warning message.");
            Logger.Instance.Error("Error message.");
            Logger.Instance.Fatal("Fatal message.");
            Logger.Instance.Dispose();
            Console.WriteLine("Done.");
        }
    }
}
