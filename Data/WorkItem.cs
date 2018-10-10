using System;
using System.IO;
using System.Configuration;

namespace KMobileProcessor.Data
{
    public class WorkItem
    {
        #region Public Properties

        public string JobNumber { get; private set; }
        public string JobFolder { get; private set; }
        public string Status { get; private set; }
        public string EstWork { get; private set; }
        public string EstCost { get; private set; }
        public string InContract { get; private set; }
        public string Area { get; private set; }
        public string DateAdded { get; private set; }
        public string Assigned { get; private set; }
        public string AssignedTo { get; private set; }
        public string DataStarted { get; private set; }
        public string ImageUploaded { get; private set; }
        public string ReportUploaded { get; private set; }
        public string CommitedToMaster { get; private set; }
        public string ClientEmailed { get; private set; }
        public string DateCompleted { get; private set; }
        public string DateAssigned { get; private set; }
        public string DataSurveyed { get; private set; }
        public int Smartscan { get; private set; }
        public string Road { get; private set; }

        public KmScheme Kscheme;

        #endregion

        #region Constructor

        public WorkItem(KmScheme scheme, string area)
        {
            Kscheme = scheme;
            GetParameters(area);
        }

        #endregion

        /// <summary>
        /// Retrieve file parameters from directory
        /// </summary>
        /// <param name="file"></param>
        private void GetParameters(string area)
        {
            if (Kscheme != null)
            {
                JobNumber = Kscheme.SchemeName;
                Road = Kscheme.SchemeGpsKit;
            }

            //Default parameters for work items
            JobFolder = Global.SortedFolder;
            Status = "Unassinged";
            EstWork = "null";
            EstCost = "null";
            InContract = "Yes";
            DateAdded = DateTime.Now.Date.ToShortDateString();
            Assigned = "No";
            AssignedTo = "null";
            DateAssigned = "null";
            DataSurveyed = "null";
            Smartscan = 0;
            DataStarted = DateTime.Now.Date.ToShortDateString();
            ImageUploaded = "No";
            ReportUploaded = "No";
            CommitedToMaster = "null";
            DateCompleted = "null";
            ClientEmailed = "null";

            Area = area;

        }
    }
}
