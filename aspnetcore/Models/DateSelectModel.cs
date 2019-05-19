using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TeslaCamBrowser.Common;

namespace TeslaCamBrowser.Models
{
    public class DateSelectModel
    {
        public List<DirectoryEntry> Dates { get; set; }
        public List<FileEntry> Files { get; set; }

        public int TotalFiles { get; set; }
        public string SelectedDate { get; set; }

        public int ItemsPerPage { get; set; }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public string FilesToDelete { get; set; }
    }
}
