using System;
using System.Configuration;

namespace KMobileProcessor.Data
{
    public static class Global
    {
        /// <summary>
        /// Source Folder from K-mobile where data is synced
        /// </summary>
        public static string SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];

        /// <summary>
        /// Backup Folder for processing Kmobile data
        /// </summary>
        public static string BackupFolder = ConfigurationManager.AppSettings["BackupFolder"];

        /// <summary>
        /// Sorted Folder for processing Kmobile data
        /// </summary>
        public static string SortedFolder = ConfigurationManager.AppSettings["SortedFolder"];

        /// <summary>
        /// Area database for the highway network
        /// </summary>
        public static string AreaDatabase = ConfigurationManager.AppSettings["HighwayNetwork"];

        public static string DBconnection = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
        public static string SqlTable = ConfigurationManager.AppSettings["table"];


        /// <summary>
        /// Configuration of email server 
        /// </summary>
        public static string SmtpServer = ConfigurationManager.AppSettings["Smtp_server"];
        public static string SmtpPort = ConfigurationManager.AppSettings["Smtp_port"];
        public static string SmtpFromTo = ConfigurationManager.AppSettings["Smtp_fromto"];

        /// <summary>
        /// Configuraton of email list for relevant staffs
        /// </summary>
        public static string Hao = ConfigurationManager.AppSettings["Hao"];

    }
}
