using System.IO;
using System.Diagnostics;

namespace KMobileProcessor.Controller
{
    public static class KmCommandPrompt
    {
        public static string[] GoToPath(string path)
        {
            string[] output;
            string cmString = "cd" + " " + path;
            return output = RunCommand(cmString, true);
        }

        public static void UploadScheme(string mergeShpPath)
        {
            string[] input = new string[3];
            //input[0] = "cd " + Properties.Settings.Default.DefaultPostgreSQL;
            //input[1] = "set PGPASSWORD=" + Properties.Settings.Default.PostGISPassword;
            input[2] = @"shp2pgsql -s 27700 C:/Test/Chambert.shp Chambert | psql -U postgres -h localhost -p 5432 -d postgres";
        }

        public static string[] UploadTest()
        {
            string[] input = new string[3];
            //input[0] = "cd C:/Program Files/PostgreSQL/10/bin"; 
            input[1] = "set PGPASSWORD=Carnell2018";
            input[2] = @"shp2pgsql -s 27700 C:/Test/Chambert.shp Chambert | psql -U postgres -h localhost -p 5432 -d postgres";
            string[] output;
            return output = KmCommandPrompt.RunCommands(input, true);
        }

        public static string[] UploadShapeFile(string fullPath, string password, string shpPath, string filename)
        {
            string[] input = new string[3];
            input[0] = "cd " + fullPath;
            input[1] = "set PGPASSWORD=" + password;
            //input[2] = "shp2pgsql -s 27700 C:/Test/Chamber.shp Chamber | psql -U postgres -h localhost -p 5432 -d postgres";
            input[2] = @"shp2pgsql -s 27700 " + shpPath + " " + filename + " " + @"| psql -U postgres -h localhost -p 5432 -d postgres";
            string[] output;
            return output = KmCommandPrompt.RunCommands(input, true);
        }

        public static string[] ListCommand(string[] command, bool output)
        {
            //Note that string [0] is error message and string[1] is output message
            string[] message = new string[2];
            return message;
        }

        public static string[] RunCommand(string command, bool output)
        {
            //Note that string [0] is error message and string[1] is output message
            string[] message = new string[2];

            //Start the command prompt
            ProcessStartInfo info = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                CreateNoWindow = true
            };

            Process proc = new Process
            {
                StartInfo = info
            };

            proc.Start();


            //The stream writer is replacing the keyboard as the input
            using (StreamWriter writer = proc.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    writer.WriteLine(command);
                }
                writer.Close();
            }

            message[0] = proc.StandardError.ReadToEnd();

            if (output)
            {
                message[0] = proc.StandardOutput.ReadToEnd();
            }

            proc.Close();

            return message;
        }

        public static string[] RunCommands(string[] command, bool output)
        {
            string[] message = new string[2];

            ProcessStartInfo info = new ProcessStartInfo
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                CreateNoWindow = true
            };

            Process proc = new Process
            {
                StartInfo = info
            };
            proc.Start();

            using (StreamWriter writer = proc.StandardInput)
            {
                if (writer.BaseStream.CanWrite)
                {
                    foreach (string q in command)
                    {
                        writer.WriteLine(q);
                    }
                }
            }

            message[0] = proc.StandardError.ReadToEnd();

            if (output)
            {
                message[0] = proc.StandardOutput.ReadToEnd();
            }

            return message;
        }
    }
}
