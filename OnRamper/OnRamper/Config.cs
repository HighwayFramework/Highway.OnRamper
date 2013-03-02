using NDesk.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnRamper
{
    public class Config
    {
        public string SourceDirectory { get; set; }
        public string DestinationDirectory { get; set; }
        public string ConfigDirectory { get; set; }
        public bool ShowHelp { get; set; }

        public bool IsValid()
        {
            if (Directory.Exists(SourceDirectory) == false)
            {
                Console.WriteLine("Source directory does not exist");
                return false;
            }
            if (Directory.Exists(DestinationDirectory) == false)
            {
                Console.WriteLine("Destination directory does not exist");
                return false;
            }
            if (Directory.Exists(ConfigDirectory) == false)
            {
                Console.WriteLine("Config directory does not exist");
                return false;
            }
            return true;
        }
    }
}
