using System;
using System.IO;

namespace KMobileProcessor.Services
{
    public class KmobileWatcher
    {
        #region Public Fields

        private readonly FileSystemWatcher kmobileWatcher;
        public string WatchingDirectory { get; set; }

        #endregion

        public KmobileWatcher(string watchingPath)
        {
            WatchingDirectory = watchingPath;

            kmobileWatcher = new FileSystemWatcher()
            {
                IncludeSubdirectories = true,
                Path = watchingPath,
                NotifyFilter = NotifyFilters.DirectoryName
            };

            kmobileWatcher.EnableRaisingEvents = true;
        }

        #region Kmobile Watcher Events

        public event FileSystemEventHandler SourceFolderCreated
        {
            add { kmobileWatcher.Created += value; }
            remove { kmobileWatcher.Created -= value; }
        }

        public event FileSystemEventHandler SourceFolderDeleted
        {
            add { kmobileWatcher.Deleted += value; }
            remove { kmobileWatcher.Deleted -= value; }
        }

        public event FileSystemEventHandler SourceFolderChanged
        {
            add { kmobileWatcher.Changed += value; }
            remove { kmobileWatcher.Changed -= value; }
        }

        #endregion

    }
}
