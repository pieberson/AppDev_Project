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

        // Default route: Redirect to About Us (Media Tracker Tab)
        public IActionResult Index()
        {
            return RedirectToAction("AboutUs");
        }

        // About Us Page (Media Tracker Tab)
        public IActionResult AboutUs()
        {
            return View("AboutUs"); // Renders the About Us page
        }

        // Home Page (Add New Media Tab)
        public IActionResult Home()
        {
            return View("Index"); // Displays the "Add New" form
        }

        // View List Page
        public IActionResult ViewList(string sortBy)
        {
            List<MediaItem> sortedList;

            switch (sortBy)
            {
                case "Year":
                    sortedList = MediaList.OrderBy(m => m.YearFinished).ToList();
                    break;
                case "Rating":
                    sortedList = MediaList.OrderByDescending(m => m.Rating).ToList();
                    break;
                case "Title":
                    sortedList = MediaList.OrderBy(m => m.Title).ToList();
                    break;
                case "Class":
                    sortedList = MediaList.OrderBy(m => m.Class).ToList();
                    break;
                default:
                    sortedList = MediaList; // Default order
                    break;
            }

            ViewData["CurrentSort"] = sortBy; // Pass the current sort value to the view
            return View(sortedList);
        }


        [HttpPost]
        public IActionResult AddNew(string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            if (Class != "Show")
            {
                Season = null; // Set Season to NULL if Class is not "Show"
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

            SaveMediaList();

            TempData["Message"] = "New item added successfully!";
            return RedirectToAction("Home"); // Redirect to the Home tab
        }

        [HttpGet]
        public IActionResult SortList(string sortBy)
        {
            if (string.IsNullOrEmpty(sortBy)) return BadRequest("Sort parameter is missing.");

            List<MediaItem> sortedList = sortBy switch
            {
                "Title" => MediaList.OrderBy(item => item.Title).ToList(),
                "YearFinished" => MediaList.OrderBy(item => item.YearFinished).ToList(),
                "Rating" => MediaList.OrderByDescending(item => item.Rating).ToList(),
                _ => MediaList
            };

            return Json(sortedList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
