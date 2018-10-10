using Serilog;


namespace KMobileProcessor.Data
{
    public static class Applogger
    {
        public static void ConfigApp()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Kmobile.log")
                .CreateLogger();
        }

        public static void LogInformation(string logmessage)
        {
            Log.Information(logmessage);
        }

        public static void LogWarning(string logmessage)
        {
            Log.Warning(logmessage);
        }
    }
}
