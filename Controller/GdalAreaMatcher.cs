using System;
using OSGeo.OGR;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace KMobileProcessor.Controller
{
    public static class GdalAreaMatcher
    {

        public static string GetInspector(string projectDict)
        {
            string inspector = String.Empty;
            Ogr.RegisterAll();
            Driver driver = Ogr.GetDriverByName("ESRI Shapefile");

            string[] files = Directory.GetFiles(projectDict, "*shp");

            foreach (string file in files)
            {
                if (file.Contains("Chamber"))
                {
                    DataSource sourceData = driver.Open(file, 1);
                    Layer layer = sourceData.GetLayerByIndex(0);
                    FeatureDefn layerDefn = layer.GetLayerDefn();

                    for (int i = 0; i < layer.GetFeatureCount(1) + 1; i++)
                    {
                        Feature inFeature = layer.GetNextFeature();
                        if (inFeature != null)
                        {
                            inspector = inFeature.GetFieldAsString("Inspector");
                            return inspector;
                        }
                    }
                }
            }
            return inspector;
        }

        public static string AreaMactch(string sourceData, string areaDatabase)
        {
            //Register OGR and read file
            Ogr.RegisterAll();
            Driver driver = Ogr.GetDriverByName("ESRI Shapefile");

            //Define a multipoint
            Geometry multipoint = new Geometry(wkbGeometryType.wkbMultiPoint);

            DataSource dataSource = driver.Open(sourceData, 1);
            Layer ly = dataSource.GetLayerByIndex(0);
            FeatureDefn layerDefn = ly.GetLayerDefn();

            //Read feature attributes from layer
            for (int i = 0; i < ly.GetFeatureCount(1) + 1; i++) // must start with 1
            {
                Feature inFeature = ly.GetNextFeature();
                if (inFeature != null)
                {
                    multipoint.AddGeometry(inFeature.GetGeometryRef());
                }
            }

            Geometry point = new Geometry(wkbGeometryType.wkbPoint);
            point = multipoint.Centroid();
            double x = point.GetX(0);
            double y = point.GetY(0);

            //Define a multipoint
            Geometry polyline = new Geometry(wkbGeometryType.wkbMultiLineString);

            //Register OGR and read file
            Driver driver2 = Ogr.GetDriverByName("ESRI Shapefile");
            DataSource networkData = driver2.Open(areaDatabase, 1);
            Layer lyNetwork = networkData.GetLayerByIndex(0);
            FeatureDefn layerNetwork = lyNetwork.GetLayerDefn();

            Dictionary<string, double> disDict = new Dictionary<string, double>();

            //Read feature attributes from layer
            for (int i = 0; i < lyNetwork.GetFeatureCount(1) + 1; i++) // must start with 1
            {
                Feature inFeature = lyNetwork.GetNextFeature();
                if (inFeature != null)
                {
                    double distance = inFeature.GetGeometryRef().Distance(point);
                    string area = inFeature.GetFieldAsString("AREA_NAME");
                    disDict.Add(area, distance);
                }
            }

            var ordered = disDict.OrderBy(item => item.Value);
            string closeArea = ordered.First().Key;
            return closeArea;
        }

    }
}
