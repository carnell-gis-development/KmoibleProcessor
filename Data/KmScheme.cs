using System;
using System.IO;

namespace KMobileProcessor.Data
{
    public class KmScheme
    {
        #region Public Fields

        public string SchemeName { get; set; }
        public string SchemeType { get; set; }
        public string SchemePath { get; set; }
        public string SchemeGpsKit { get; set; }
        public string SchemeInspector { get; set; }
        public DateTime SchemeCreationDate { get; set; }
        public DirectoryInfo SchemeDirect { get; set; }
        public DirectoryInfo[] SubSchemeDirects { get; set; }
        public FileInfo[] SchemeImageCollection { get; set; }
        public FileInfo[] SchemeShapeCollection { get; set; }

        #endregion

        public KmScheme(string schemePath)
        {
            SchemePath = schemePath;
            SchemeDirect = new DirectoryInfo(schemePath);
            SchemeName = SchemeDirect.Name;
            SubSchemeDirects = SchemeDirect.GetDirectories();
            SchemeImageCollection = SchemeDirect.GetFiles("*JPG");
            SchemeShapeCollection = SchemeDirect.GetFiles("*SHP");
        }
    }
}
