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
                
                // Performance monitoring
                o.TracesSampleRate = 1.0;
                
                // Session tracking
                o.IsGlobalModeEnabled = true;
                o.AutoSessionTracking = true;
                
                // Version tracking
                o.Release = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString();
                
                // Environment configuration
                #if DEBUG
                o.Environment = "development";
                o.Debug = true;
                o.DiagnosticLevel = Sentry.SentryLevel.Debug;
                #else
                o.Environment = "production";
                o.Debug = false;
                #endif
                
                // Breadcrumbs configuration
                o.MaxBreadcrumbs = 100;
                
                // Event filtering and enrichment
                o.SetBeforeSend((sentryEvent, hint) =>
                {
                    // Add custom context to all events
                    sentryEvent.SetTag("os", Environment.OSVersion.ToString());
                    sentryEvent.SetTag("machine-name", Environment.MachineName);
                    return sentryEvent;
                });
                
                // Attach stack traces to all messages
                o.AttachStacktrace = true;
                
                // Send default PII (Personally Identifiable Information)
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