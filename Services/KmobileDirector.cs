using System;
using System.IO;
using Topshelf;
using KMobileProcessor.Data;
using System.Threading;

namespace KMobileProcessor.Services
{
    public class KmobileDirector : ServiceControl
    {
        #region Private Fields

        private KmobileWatcher watcher;
        private KmobileBuilder builder;
        private Thread excuteThread;

        #endregion

        #region Constructor

        public KmobileDirector()
        {
            Applogger.ConfigApp();
            watcher = new KmobileWatcher(Global.SourceFolder);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Public Method

        public void Start()
        {
            try
            {
                excuteThread = new Thread(new ThreadStart(WorkExecute));
                excuteThread.Start();
            }
            catch
            {
                throw;
            }
        }

        public void WorkExecute()
        {
            watcher.SourceFolderCreated += new FileSystemEventHandler(KmobileCreated);
            Console.WriteLine("Watcher is started");
        }

        public void Stop()
        {
            watcher.SourceFolderCreated -= new FileSystemEventHandler(KmobileCreated);
            excuteThread.Abort();
        }

        public bool Start(HostControl hostControl)
        {
            throw new NotImplementedException();
        }

        public bool Stop(HostControl hostControl)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region Events

        void KmobileCreated(object sender, FileSystemEventArgs e)
        {
            Applogger.LogInformation($"Kmobile project:{e.Name} is detected");

            if (e == null)
            {
                Applogger.LogInformation($"Kmobile project is a null project cannot be processed");
            }
            else
            {
                builder = new KmobileBuilder(e.FullPath);
                builder.BuildScheme();
            }
        }

        #endregion

    }
}
