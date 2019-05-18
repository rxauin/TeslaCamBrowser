using System;
using System.Collections.Generic;

namespace TeslaCamBrowser.Common
{
    public class DirectoryEntry
    {
        public DateTime Date { get; set; }
        public List<string> Paths { get; set; } = new List<string>(1);

        public string SmallDate => Date.ToString("MM-dd-yyyy");

        public override string ToString()
        {
            return Date.ToString("MM-dd-yyyy");
        }
    }
}
