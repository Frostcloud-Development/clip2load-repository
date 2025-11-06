namespace clip2load
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize Sentry SDK for logging and analytics
            SentrySdk.Init(o =>
            {
                o.Dsn = "https://566cd2389f8b8dbaebea75cf6ef12b25@o1113761.ingest.us.sentry.io/4510319876505600"; // future me, please remember to add this
                o.TracesSampleRate = 1.0;
                o.IsGlobalModeEnabled = true;
                o.AutoSessionTracking = true;
                o.Release = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            });

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new clip2load());
        }
    }
}