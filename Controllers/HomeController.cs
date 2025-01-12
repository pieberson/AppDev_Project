using AppDev.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace AppDev.Controllers
{
    public class HomeController : Controller
    {
        private static string DataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaList.json");
        private static List<MediaItem> MediaList = LoadMediaList();

        private static List<MediaItem> LoadMediaList()
        {
            if (System.IO.File.Exists(DataFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(DataFilePath);
                return JsonSerializer.Deserialize<List<MediaItem>>(jsonData) ?? new List<MediaItem>();
            }
            return new List<MediaItem>();
        }

        private static void SaveMediaList()
        {
            var jsonData = JsonSerializer.Serialize(MediaList);
            System.IO.File.WriteAllText(DataFilePath, jsonData);
        }

        public IActionResult Index()
        {
            return View(); // Ensure this view exists in Views/Home
        }

        public IActionResult ViewList()
        {
            return View(MediaList); // Pass the list to the View
        }

        [HttpPost]
        public IActionResult AddNew(string Class, string InputTitle, int YearFinished, int Rating, string Review)
        {
            MediaList.Add(new MediaItem
            {
                Class = Class,
                Title = InputTitle,
                YearFinished = YearFinished,
                Rating = Rating,
                Review = Review
            });

            SaveMediaList(); // Save the updated list to the file

            TempData["Message"] = "New item added successfully!";
            return RedirectToAction("Index");
        }
    }
}
