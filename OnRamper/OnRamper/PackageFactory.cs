using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnRamper
{
    public class PackageFactory
    {
        public void Build(Config config)
        {
            var configFiles = Directory.GetFiles(config.ConfigDirectory, "*.nuspec");
            foreach (var file in configFiles)
            {
                var packageName = Path.GetFileNameWithoutExtension(file);
                var pconfig = new PackageConfig(config, packageName);

                // Create all the package directories we'll need
                pconfig.Ensure();

                // Move in the nuspec file
                string nuspecFile = Path.Combine(pconfig.PackageDirectory, Path.GetFileName(file));
                File.Copy(file, nuspecFile, true);

                // Recurse the directories, looking for files which opted into the package 
                var selectedContent = new List<string>();
                FilterForFiles(config.SourceDirectory, selectedContent, packageName);

                // Announce the files
                AnnounceFiles(pconfig, selectedContent);

                // Move files from source to destination content
                MoveContentInPlace(selectedContent, pconfig.ContentDirectory, config.SourceDirectory);

                // Optionally execute NuGet.exe
                if (config.ShouldExecute)
                {
                    string args = string.Format(" pack \"{0}\" -o \"{1}\"", Path.GetFullPath(nuspecFile), Path.GetFullPath(config.DestinationDirectory).TrimEnd('\\'));
                    Console.WriteLine(Path.GetFullPath(config.ExecutePath) + " " + args);
                    var p = Process.Start(Path.GetFullPath(config.ExecutePath), args);
                    p.OutputDataReceived += p_OutputDataReceived;
                    p.Start();
                    p.WaitForExit();
                }
            }
        }

        void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private static void AnnounceFiles(PackageConfig pconfig, List<string> selectedContent)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(pconfig.PackageName);
            Console.WriteLine("-------------------------------------------------------------");
            Console.ResetColor();
            foreach (var file in selectedContent)
            {
                Console.WriteLine(file);
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        private void MoveContentInPlace(List<string> selectedContent, string contentDirectory, string sourceDirectory)
        {
            foreach (string item in selectedContent)
            {
                var pathFragment = item.Replace(sourceDirectory, "").TrimStart('/', '\\');
                var destFile = Path.Combine(contentDirectory, pathFragment);
                string destDirectory = Path.GetDirectoryName(destFile);

                if (Directory.Exists(destDirectory) == false)
                    Directory.CreateDirectory(destDirectory);

                using (var rdr = new StreamReader(item))
                {
                    var content = rdr.ReadToEnd();
                    content = content.Replace("Template.", "$rootnamespace$.");
                    using (var wrt = new StreamWriter(destFile))
                    {
                        wrt.Write(content);
                        wrt.Flush();
                    }
                }
            }
        }


        private void FilterForFiles(string directory, List<string> selectedContent, string packageName)
        {
            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                FilterForFiles(subDirectory, selectedContent, packageName);
            }

            foreach (string file in Directory.GetFiles(directory))
            {
                using (var rdr = new StreamReader(file))
                {
                    var firstLine = rdr.ReadLine();
                    if (firstLine != null && firstLine.Contains(String.Format("[[{0}]]", packageName)))
                    {
                        selectedContent.Add(file);
                    }
                }
            }
        }
        public class PackageConfig
        {
            public PackageConfig(Config config, string packageName)
            {
                PackageName = packageName;
                PackageDirectory = Path.Combine(config.DestinationDirectory, packageName);
                ContentDirectory = Path.Combine(PackageDirectory, "content");
                LibDirectory = Path.Combine(PackageDirectory, "lib");
                ToolsDirectory = Path.Combine(PackageDirectory, "tools");
            }
            public string PackageName { get; set; }
            public string ContentDirectory { get; set; }
            public string LibDirectory { get; set; }
            public string ToolsDirectory { get; set; }
            public string PackageDirectory { get; set; }

            public void Ensure()
            {
                if (Directory.Exists(PackageDirectory) == false)
                    Directory.CreateDirectory(PackageDirectory);
                if (Directory.Exists(ContentDirectory) == false)
                    Directory.CreateDirectory(ContentDirectory);
                if (Directory.Exists(ToolsDirectory) == false)
                    Directory.CreateDirectory(ToolsDirectory);
                if (Directory.Exists(LibDirectory) == false)
                    Directory.CreateDirectory(LibDirectory);
            }
        }
    }
}
