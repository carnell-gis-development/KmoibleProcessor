using System;
using System.Threading;
using Topshelf;
using KMobileProcessor.Services;
using KMobileProcessor.Data;

namespace KMobileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {

            var rc = HostFactory.Run(x =>
            {
                x.Service<KmobileDirector>(sc =>
                {
                    sc.ConstructUsing(hostSettings => new KmobileDirector());
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.Stop());

                });

                x.RunAsLocalSystem();
                x.SetDescription("Kmobile Automation: Carnell internal windows service for Smartscan data processing");
                x.SetDisplayName("Kmobile Automation");
                x.SetServiceName("Kmobile Automation");
                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                    r.OnCrashOnly();
                });
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;

            Applogger.LogInformation("Carnell service windows service: Topshelf installation is configured");
            new System.Threading.AutoResetEvent(false).WaitOne();
        }
    
    }
}
