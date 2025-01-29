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

        // Load media list from file
        private static List<MediaItem> LoadMediaList()
        {
            if (System.IO.File.Exists(DataFilePath))
            {
                var jsonData = System.IO.File.ReadAllText(DataFilePath);
                var mediaList = JsonSerializer.Deserialize<List<MediaItem>>(jsonData) ?? new List<MediaItem>();

                // Assign unique IDs if missing or duplicate
                int idCounter = 1;
                foreach (var item in mediaList)
                {
                    item.Id = idCounter++;
                }

                return mediaList;
            }
            return new List<MediaItem>();
        }

        // Save media list to file
        private static void SaveMediaList()
        {
            var jsonData = JsonSerializer.Serialize(MediaList);
            System.IO.File.WriteAllText(DataFilePath, jsonData);
        }

        // Routes
        [Route("TRACKXD/AboutUs")]
        public IActionResult AboutUs() => View("AboutUs");

        [Route("TRACKXD/Home")]
        public IActionResult Home() => View("Index");

        [Route("TRACKXD/ViewList")]
        public IActionResult ViewList(string sortBy)
        {
            List<MediaItem> sortedList = sortBy switch
            {
                "Year" => MediaList.OrderBy(m => m.YearFinished).ToList(),
                "Rating" => MediaList.OrderByDescending(m => m.Rating).ToList(),
                "Title" => MediaList.OrderBy(m => m.Title).ToList(),
                "Class" => MediaList.OrderBy(m => m.Class).ToList(),
                _ => MediaList
            };

            ViewData["CurrentSort"] = sortBy;
            return View(sortedList);
        }

        // Add new media item
        [HttpPost]
        [Route("TRACKXD/Home/AddNew")]
        public IActionResult AddNew(string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            if (Class != "Show") Season = null; // Nullify season unless it's a show

            var newId = MediaList.Count > 0 ? MediaList.Max(m => m.Id) + 1 : 1;

            MediaList.Add(new MediaItem
            {
                Id = newId,
                Class = Class,
                Title = InputTitle,
                YearFinished = YearFinished,
                Rating = Rating,
                Review = Review,
                Season = Season
            });

            SaveMediaList();
            TempData["Message"] = "New item added successfully!";
            return RedirectToAction("Home");
        }

        // Edit media item (GET)
        [Route("TRACKXD/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            return mediaItem == null ? NotFound() : View(mediaItem);
        }

        // Edit media item (POST)
        [HttpPost]
        [Route("TRACKXD/Edit/{id}")]
        public IActionResult Edit(int id, string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            if (mediaItem == null) return NotFound();

            // Update item details
            mediaItem.Class = Class;
            mediaItem.Title = InputTitle;
            mediaItem.YearFinished = YearFinished;
            mediaItem.Rating = Rating;
            mediaItem.Review = Review;
            mediaItem.Season = Class == "Show" ? Season : null;

            SaveMediaList();
            TempData["Message"] = "Item updated successfully!";
            return RedirectToAction("ViewList");
        }

        // Delete media item
        [HttpPost, ActionName("Delete")]
        [Route("TRACKXD/Delete/{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            if (mediaItem == null) return NotFound();

            MediaList.Remove(mediaItem);
            SaveMediaList();

            TempData["Message"] = "Item deleted successfully!";
            return RedirectToAction("ViewList");
        }

        // Sort media list (AJAX)
        [HttpGet]
        [Route("TRACKXD/SortList")]
        public IActionResult SortList(string sortBy)
        {
            if (string.IsNullOrEmpty(sortBy)) return BadRequest("Sort parameter is missing.");

            var sortedList = sortBy switch
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
