using System;
using System.IO;
using KMobileProcessor.Controller;
using KMobileProcessor.Data;
using OSGeo.OGR;
using OSGeo.OSR;
using System.Net;
using System.Data.SqlClient;

namespace KMobileProcessor.Services
{
    class KmobileBuilder
    {
        #region All Fields

        // Constant
        private const string geoRef = "27700";

        // Folder structure
        public string KmSourceFolder { get; set; }
        public string KmBackUpFolder { get; set; }
        public string KmSchemePath { get; set; }
        public string CopyToSchemePath { get; set; }
        public string MergeShpPath { get; set; }
        public string BackupDirectory { get; set; }

        // Scheme Information
        public string SchemeArea { get; set; }
        public string SyncSchemePath { get; set; }
        public KmScheme CurrentScheme { get; set; }

        #endregion

        public KmobileBuilder(string schemePath)
        {            
            Ogr.RegisterAll();

            // Build scheme information from folder structure
            KmSchemePath = schemePath;
            CurrentScheme = new KmScheme(schemePath);
        }

        #region Public Methods

        public void BuildScheme()
        {
            // Move scheme folder from K-mobile source folder to backup folder
            MoveScheme();

            // Merge all point assets in one shapefile
            MergeAsset();

            // Append any image information to shapefile
            AppendImage();

            // Match the job to correct area division in Highway England
            AreaMatch();

            // Sort the data to the highway network area
            SortToArea();

            // Alter the fields to avoid the error 
            AlterField();

            // Upload Scheme data to SQL server
            UploadScheme();

            // Update the database in task manager list SQL server
            //UpdateDatabase();

            // Send email notification to relevent staffs
            NotifyResult();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Move scheme folder from K-mobile source folder to backup folder
        /// </summary>
        private void MoveScheme()
        {
            // Check if scheme is an existing scheme in Backup Folder
            var Schemes = DirectoryHelper.GetDirectories(Global.BackupFolder);

            // Overwrite the data if the file exists, otherwise copy the data
            bool inSchemeList = DirectoryHelper.IsInDirectoryList(CurrentScheme.SchemeName, Schemes);

            // Copy data if required
            if (!inSchemeList)
            {
                CopyToSchemePath = Path.Combine(Global.BackupFolder, CurrentScheme.SchemeName);
                //CopyToSchemePath = Global.BackupFolder + "\\" + CurrentScheme.SchemeName;
            }
            else
            {
                CopyToSchemePath = Path.Combine(Global.BackupFolder, CurrentScheme.SchemeName) + " Latest";
                //CopyToSchemePath = AppController.KmBackupFolder + "\\" + this.CurrentScheme.SchemeName + " Latest";
            }

            // Delete existing folder in backup folder 
            if (Directory.Exists(CopyToSchemePath))
            {
                DirectoryHelper.DeleteDirectory(CopyToSchemePath);
            }

            // Createa a new folder for data storage
            Directory.CreateDirectory(CopyToSchemePath);
            DirectoryHelper.CopyFiles(CurrentScheme.SchemePath, CopyToSchemePath);
            DirectoryHelper.DeleteDirectory(CurrentScheme.SchemePath);
        }

        /// <summary>
        /// Merge all point assets in one shapefile
        /// </summary>
        private void MergeAsset()
        {
            // Step1: Create an empty shapefile datasource and a layer as merged file
            string outputMergefn = "PointMerge.shp";
            MergeShpPath = Path.Combine(CopyToSchemePath, outputMergefn);

            // Make sure to use ESRI standard to keep compatibility
            Driver driver = Ogr.GetDriverByName("ESRI Shapefile");

            if (File.Exists(this.MergeShpPath))
            {
                File.Delete(this.MergeShpPath);
            }

            // Configure geospatial parameters for new shapefile, e.g. coordinate system
            DataSource mergeds = driver.CreateDataSource(CopyToSchemePath, null);
            SpatialReference srs = new SpatialReference(geoRef);

            // If the merge file exists, delete it
            if (File.Exists(MergeShpPath))
                File.Delete(MergeShpPath);

            Layer mergeLy = mergeds.CreateLayer("PointMerge", srs, wkbGeometryType.wkbPoint, null);
            DataSource dataSource;

            // Step2: Find all relevant shapefiles
            foreach (var shapefile in CurrentScheme.SchemeShapeCollection)
            {
                if (shapefile.Name.Contains("Chamber") || shapefile.Name.Contains("Ghost_Node"))
                {
                    // Get datasource from the shapefile
                    dataSource = driver.Open(Path.Combine(CopyToSchemePath, shapefile.Name), 0);
                    Layer layer = dataSource.GetLayerByIndex(0);

                    // Get shapefile definition
                    FeatureDefn lyDf = layer.GetLayerDefn();
                    for (int i = 0; i < lyDf.GetFieldCount(); i++)
                    {
                        FieldDefn fDf = lyDf.GetFieldDefn(i);
                        mergeLy.CreateField(fDf, 1);
                    }

                    // Get Features
                    for (int i = 0; i < layer.GetFeatureCount(1); i++)
                    {
                        Feature outFeature = new Feature(layer.GetLayerDefn());
                        Feature inFeature = layer.GetNextFeature();
                        outFeature.SetGeometry(inFeature.GetGeometryRef().Clone());

                        for (int j = 0; j < layer.GetLayerDefn().GetFieldCount(); j++)
                        {
                            outFeature.SetField(inFeature.GetFieldDefnRef(j).GetNameRef(), inFeature.GetFieldAsString(j));
                        }

                        mergeLy.CreateFeature(outFeature);
                        mergeLy.SyncToDisk();
                    }
                }
            }
    }

        /// <summary>
        ///  Append any image information to shapefile
        /// </summary>
        private void AppendImage()
        {
            Driver driver2 = Ogr.GetDriverByName("ESRI Shapefile");
            DataSource dataSource = driver2.Open(MergeShpPath, 1);
            Layer ly = dataSource.GetLayerByIndex(0);
            FeatureDefn layerDefn = ly.GetLayerDefn();

            foreach (var file in CurrentScheme.SchemeImageCollection)
            {
                string filetitle = file.Name.Replace("Chamber-", "").Replace("-Sketch.JPG", "");

                //Read feature attributes from layer
                for (int i = 0; i < ly.GetFeatureCount(1) + 1; i++) // must start with 1
                {
                    Feature inFeature = ly.GetNextFeature();
                    if (inFeature != null)
                    {
                        string fdValue = inFeature.GetFieldAsString("GPS_Ref");
                        if (fdValue == filetitle)
                        {
                            inFeature.SetField("Sketch", file.FullName);
                            ly.SetFeature(inFeature);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Match the scheme to correct area
        /// </summary>
        private void AreaMatch()
        {
            if (MergeShpPath != null)
            {
                SchemeArea = GdalAreaMatcher.AreaMactch(MergeShpPath, Global.AreaDatabase);
            }
            else
            {
                throw new Exception("The merge file is not produced and cannot process area");
            }
        }

        /// <summary>
        /// Sort the scheme to correct area folder
        /// </summary>
        private void SortToArea()
        {
            DirectoryInfo[] dictInfo = DirectoryHelper.GetDirectories(Global.SortedFolder);
            foreach (var dict in dictInfo)
            {
                if (dict.Name == SchemeArea)
                {
                    SyncSchemePath = Global.SortedFolder + "\\" + dict.Name + "\\" + CurrentScheme.SchemeName;
                    if (Directory.Exists(SyncSchemePath))
                    {
                        DirectoryHelper.DeleteDirectory(SyncSchemePath);
                    }
                    Directory.CreateDirectory(SyncSchemePath);
                    DirectoryHelper.CopyFiles(CopyToSchemePath, SyncSchemePath);
                    break;
                }
            }
        }

        /// <summary>
        /// Alter the fields to avoid the error 
        /// </summary>
        private void AlterField()
        {
            string[] input = new string[3];
            string oldField = "\"CONSTRAINT\"";
            string newField = "\"Cstre\"";
            input[0] = @"cd C:\Program Files\PostgreSQL\10\bin";
            string sql = $"\"ALTER TABLE PointMerge RENAME COLUMN {oldField} TO {newField}\"";
            input[2] = $"ogrinfo {MergeShpPath} -sql {sql}";
            //input[2] = $"ogrinfo {MergeShpPath} - sql \"ALTER TABLE PointMerge RENAME COLUMN \"CONSTRAINT\" TO \"Cstre\"";
            string[] output;
            output = KmCommandPrompt.RunCommands(input, true);
            Console.WriteLine("Test");
        }

        /// <summary>
        /// Upload Scheme data to SQL server
        /// </summary>
        private void UploadScheme()
        {
            string[] input = new string[3];
            input[0] = string.Join(" ", "cd", "C:/Program Files/PostgreSQL/10/bin");
            input[1] = string.Join(" ", "set", "MSSQLSPATIAL_USE_GEOMETRY_COLUMNS=NO");
            input[2] = "ogr2ogr -f \"MSSQLSpatial\" \"MSSQL: server=carnell99\\arc_sqlexpress;database=raw_gps;uid=HaoYe;pwd=Carnell2018\" " + "\"" + MergeShpPath + "\"";
            string[] output;
            output = KmCommandPrompt.RunCommands(input, true);
            Console.WriteLine("Test");
        }

        /// <summary>
        /// Update the database in task manager list SQL server
        /// </summary>
        private void UpdateDatabase()
        {
            WorkItem workitem = new WorkItem(CurrentScheme, SchemeArea);
            string table = Global.SqlTable;
            string sqlQuery =
                $"INSERT INTO {table} (" +
                     "[job number]," +
                     "[job folder]," +
                     "[status]," +
                     "[Est work (days)]," +
                     "[est cost]," +
                     "[in contract]," +
                     "[area]," +
                     "[date added]," +
                     "[assigned]," +
                     "[assigned to]," +
                     "[date started]," +
                     "[images uploaded]," +
                     "[reports uploaded]," +
                     "[commited to master]," +
                     "[client emailed]," +
                     "[date completed]," +
                     "[date assigned]," +
                     "[date surveyed]," +
                     "[smartscan]," +
                     "[road])" +
                "VALUES(" +
                    $"'{workitem.JobNumber}'" + "," +
                    $"'{workitem.JobFolder}'" + "," +
                    $"'{workitem.Status}'" + "," +
                    $"{workitem.EstWork}" + "," +
                    $"{workitem.EstCost}" + "," +
                    $"'{workitem.InContract}'" + "," +
                    $"'{workitem.Area}'" + "," +
                    $"'{workitem.DateAdded}'" + "," +
                    $"'{workitem.Assigned}'" + "," +
                    $"'{workitem.AssignedTo}'" + "," +
                    $"'{workitem.DataStarted}'" + "," +
                    $"'{workitem.ImageUploaded}'" + "," +
                    $"'{workitem.ReportUploaded}'" + "," +
                    $"'{workitem.CommitedToMaster}'" + "," +
                    $"'{workitem.ClientEmailed}'" + "," +
                    $"{workitem.DateCompleted}" + "," +
                    $"{workitem.DateAssigned}" + "," +
                    $"{workitem.DataSurveyed}" + "," +
                    $"'{workitem.Smartscan}'" + "," +
                    $"'{workitem.Road}'" + ");";

            Console.WriteLine(sqlQuery); ;

            string connectString;
            connectString = Global.DBconnection;

            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sqlQuery;
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Send email notification to relevent staffs
        /// </summary>
        private void NotifyResult()
        {
            MailSender.SendNotification(CurrentScheme.SchemeName);
        }

        #endregion

    }
}
