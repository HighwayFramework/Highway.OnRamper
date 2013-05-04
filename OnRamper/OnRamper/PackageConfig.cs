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
    public class PackageConfig
    {
        public PackageConfig(Config config, string packageName)
        {
            PackageName = packageName;
            this.config = config;
            PackageDirectory = Path.Combine(config.DestinationDirectory, packageName);
            ContentDirectory = Path.Combine(PackageDirectory, "content");
            LibDirectory = Path.Combine(PackageDirectory, "lib");
            ToolsDirectory = Path.Combine(PackageDirectory, "tools");
        }

        private readonly Config config;

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

            var scriptsPath = Path.Combine(config.ConfigDirectory, PackageName + ".Scripts");
            if (Directory.Exists(scriptsPath))
            {
                foreach (string file in Directory.GetFiles(scriptsPath))
                {
                    File.Copy(file, Path.Combine(ToolsDirectory, Path.GetFileName(file)));
                }
            }
        }
    }
}
