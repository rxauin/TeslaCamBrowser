using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

using TeslaCamBrowser.Common;
using TeslaCamBrowser.Models;
using Microsoft.AspNetCore.Authorization;

namespace TeslaCamBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly int YEAR = DateTime.Now.Year;
        private readonly int MONTH = DateTime.Now.Month;
        private readonly int DAY = DateTime.Now.Day;

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IOptions<TeslaCamBrowserConfiguration> _config;
        private readonly IHttpContextAccessor _accessor;

        public HomeController(IOptions<TeslaCamBrowserConfiguration> config, IHostingEnvironment hostingEnvironment, IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config;
            _accessor = accessor;
        }

        [Authorize]
        public IActionResult Index(DateSelectModel model)
        {
            populateModel(model);

            if (Request.Method == "POST" && Request.Form.ContainsKey("cmdDelete") && !string.IsNullOrEmpty(model.FilesToDelete))
            {
                deleteFiles(model);
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Single(string id)
        {
            string date = id.Substring(0, 10);

            FileEntry result = getFiles(date).FirstOrDefault(i => i.FileId == id);

            return View(result);
        }

        private void populateModel(DateSelectModel model)
        {
            model.Dates = getDirectories();

            if (model.SelectedDate != null)
            {
                model.Files = getFiles(model.SelectedDate);
            }
        }

        private List<DirectoryEntry> getDirectories()
        {
            List<DirectoryEntry> results = new List<DirectoryEntry>(0);
            foreach (string directory in Directory.EnumerateDirectories(_config.Value.DashCamPath))
            {
                string datePart = directory.Substring(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1, 10);
                DateTime date = DateTime.ParseExact(datePart, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var foundItem = results.FirstOrDefault(i => i.Date == date);
                if (foundItem == null)
                {
                    foundItem = new DirectoryEntry() { Date = date };
                    results.Add(foundItem);
                }

                foundItem.Paths.Add(directory);
            }
            return results;
        }

        private FileEntry parseFileEntry(string previewFileName)
        {
            FileEntry results = new FileEntry();

            string timepart = previewFileName.Substring(previewFileName.LastIndexOf(Path.DirectorySeparatorChar) + 12, 5);
            DateTime t = new DateTime(YEAR, MONTH, DAY, int.Parse(timepart.Substring(0, 2)), int.Parse(timepart.Substring(3)), 0);


            results.Time = t.ToShortTimeString();
            results.PreviewFile = previewFileName;
            results.CombinedFile = previewFileName.Substring(0, previewFileName.Length - 12) + "-combined.mp4";
            results.TimelapseFile = previewFileName.Substring(0, previewFileName.Length - 12) + "-timelapse.mp4";

            results.MappedPreview = @"/Tesla/" + results.PreviewFile.Substring(_config.Value.DashCamPath.Length).Replace(@"\", "/");
            results.MappedCombined = @"/Tesla/" + results.CombinedFile.Substring(_config.Value.DashCamPath.Length).Replace(@"\", "/");
            results.MappedTimelapse = @"/Tesla/" + results.TimelapseFile.Substring(_config.Value.DashCamPath.Length).Replace(@"\", "/");

            results.FileId = previewFileName.Substring(previewFileName.LastIndexOf(Path.DirectorySeparatorChar) +1, 16);

            return results;
        }

        private List<FileEntry> getFiles(string selectedDate)
        {
            List<FileEntry> results = new List<FileEntry>(1);

            DateTime date = DateTime.Parse(selectedDate);
            string partial = date.ToString("yyyy-MM-dd") + "_*";

            foreach (string directory in Directory.EnumerateDirectories(_config.Value.DashCamPath, partial))
            {
                foreach (string file in Directory.EnumerateFiles(directory, "*-preview.mp4"))
                {
                    results.Add(parseFileEntry(file));    
                }
            }
            return results;
        }

        private void deleteFiles(DateSelectModel model)
        {
            List<string> idsToDelete = model.FilesToDelete.Split(':').ToList();

            idsToDelete.RemoveAll(i => string.IsNullOrEmpty(i));

            var filesToDelete = 
                model.Files
                    .Join(idsToDelete, i => i.FileId, j => j, (i, j) => new { i.CombinedFile, i.PreviewFile, i.TimelapseFile });

            filesToDelete
                .ToList()
                .ForEach(i =>
                {
                    System.IO.File.Delete(i.PreviewFile);
                    System.IO.File.Delete(i.CombinedFile);
                    System.IO.File.Delete(i.TimelapseFile);
                });

            filesToDelete
                .Select(i => i.PreviewFile.Substring(0, i.PreviewFile.LastIndexOf(Path.DirectorySeparatorChar)))
                .Distinct()
                .ToList()
                .ForEach(i =>
                {
                    if (!Directory.EnumerateFiles(i).Any())
                    {
                        Directory.Delete(i);
                    }
                });

            model.Files.RemoveAll(i => idsToDelete.Contains(i.FileId));
            model.FilesToDelete = string.Empty;
        }
   }
}

