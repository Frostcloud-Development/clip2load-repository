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
                o.Dsn = "https://566cd2389f8b8dbaebea75cf6ef12b25@o1113761.ingest.us.sentry.io/4510319876505600";
				o.Debug = true;
                o.TracesSampleRate = 1.0;
                o.IsGlobalModeEnabled = true;
				o.Release = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                o.Environment = "production";
                o.AutoSessionTracking = true;
                
                o.SetBeforeSend((sentryEvent, hint) =>
                {
                    // Add custom context to all events
                    sentryEvent.SetTag("os", Environment.OSVersion.ToString());
                    return sentryEvent;
                });
                
                o.AttachStacktrace = true;
                o.SendDefaultPii = false;
            });

            // Add global exception handler
            Application.ThreadException += (sender, e) =>
            {
                SentrySdk.CaptureException(e.Exception);
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new clip2load());
        }
    }
}