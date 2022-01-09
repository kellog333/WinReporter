using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinReporter
{
    internal class ListItemsObjs
    {
        public class DriveItem
        {
            public string Drive { get; set; }
            public string Available { get; set; }
            public string Total { get; set; }
            public string Used { get; set; }
        }

        public class OpticalDriveItem
        {
            public string Drive { get; set; }
            public string Name { get; set; }
        }

        public class MemoryItem
        {
            public string Location { get; set; }
            public string Manufacturer { get; set; }
            public string PartNumber { get; set; }
            public string Capacity { get; set; }
        }
    }
}
