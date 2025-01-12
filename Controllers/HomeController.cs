using AppDev.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AppDev.Controllers
{
    public class HomeController : Controller
    {
        private static string DataFilePath = "MediaList.json";
        private static List<MediaItem> MediaList = LoadMediaList();

        // Load MediaList from the file
        private static List<MediaItem> LoadMediaList()
        {
            if (System.IO.File.Exists(DataFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(DataFilePath);
                return JsonSerializer.Deserialize<List<MediaItem>>(jsonData) ?? new List<MediaItem>();
            }
            return new List<MediaItem>();
        }

        // Save MediaList to the file
        private static void SaveMediaList()
        {
            var jsonData = JsonSerializer.Serialize(MediaList);
            System.IO.File.WriteAllText(DataFilePath, jsonData);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewList()
        {
            return View(MediaList);
        }

        [HttpPost]
        public IActionResult AddNew(string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            if (Class != "Show")
            {
                Season = null; 
            }

            MediaList.Add(new MediaItem
            {
                Class = Class,
                Title = InputTitle,
                YearFinished = YearFinished,
                Rating = Rating,
                Review = Review,
                Season = Season
            });

            SaveMediaList(); // Save the updated list to the file

            TempData["Message"] = "New item added successfully!";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
