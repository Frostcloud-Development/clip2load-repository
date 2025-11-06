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
                o.Dsn = ""; // future me, please remember to add this
                o.Debug = true;
                o.TracesSampleRate = 1.0;
                o.IsGlobalModeEnabled = true;
                o.Release = Properties.Resources.sentry_version;
                o.AutoSessionTracking = true;
                o.Environment = Properties.Resources.sentry_enviroment;
                o.StackTraceMode = Sentry.StackTraceMode.Enhanced;
            });

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new clip2load());
        }
    }
}