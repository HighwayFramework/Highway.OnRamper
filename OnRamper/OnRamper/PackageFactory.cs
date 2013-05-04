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
        private List<string> xmlExtensions;

        public void Build(Config config)
        {
            xmlExtensions = new List<string> { ".xml", ".config" };
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
                MoveContentInPlace(selectedContent, pconfig, config);

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


        private void MoveContentInPlace(List<string> selectedContent, PackageConfig pconfig, Config config)
        {
            foreach (string item in selectedContent)
            {
                var pathFragment = item.Replace(config.SourceDirectory, "").TrimStart('/', '\\');
                var destFile = Path.Combine(pconfig.ContentDirectory, pathFragment);
                string destDirectory = Path.GetDirectoryName(destFile);

                if (Directory.Exists(destDirectory) == false)
                    Directory.CreateDirectory(destDirectory);

                using (var rdr = new StreamReader(item))
                {
                    string itemExtension = Path.GetExtension(item);
                    if (xmlExtensions.Contains(itemExtension.ToLower()))
                        WriteXmlTransform(destFile, rdr, pconfig, config);
                    else
                        WritePartialFile(destFile, rdr);
                }
            }
        }

        private void WriteXmlTransform(string destFile, StreamReader rdr, PackageConfig pconfig, Config config)
        {
            destFile += ".transform";
            var content = rdr.ReadToEnd();
            var cExaminer = new ConfigExaminer();
            var xDoc = cExaminer.Populate(pconfig.PackageName, content);
            xDoc.Save(destFile);
        }

        private static void WritePartialFile(string destFile, StreamReader rdr)
        {
            destFile += ".pp";
            var content = rdr.ReadToEnd();
            content = content.Replace("Templates", "$rootnamespace$");
            using (var wrt = new StreamWriter(destFile))
            {
                wrt.Write(content);
                wrt.Flush();
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
                    var allContent = rdr.ReadToEnd();
                    if ((firstLine != null && ContainsPackage(firstLine, packageName)) ||
                        (xmlExtensions.Contains(Path.GetExtension(file)) && ContainsPackage(allContent, packageName)))
                    {
                        selectedContent.Add(file);
                    }
                }
            }
        }

        private bool ContainsPackage(string content, string packageName)
        {
            return content.Contains(String.Format("[[{0}]]", packageName));
        }
    }
}
