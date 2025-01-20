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
                var mediaList = JsonSerializer.Deserialize<List<MediaItem>>(jsonData) ?? new List<MediaItem>();

                // Ensure unique IDs for items with duplicate or zero IDs
                int idCounter = 1;
                foreach (var item in mediaList)
                {
                    item.Id = idCounter++;
                }

                return mediaList;
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

        // Add New Media Item
        [HttpPost]
        public IActionResult AddNew(string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            if (Class != "Show")
            {
                Season = null; // Set Season to NULL if Class is not "Show"
            }

            // Assign a unique Id to the new MediaItem
            var newId = MediaList.Count > 0 ? MediaList.Max(m => m.Id) + 1 : 1;

            MediaList.Add(new MediaItem
            {
                Id = newId, // Ensure unique ID for each item
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

        // Edit Media Item: Display the Edit Form (GET)
        public IActionResult Edit(int id)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            if (mediaItem == null)
            {
                return NotFound(); // Handle case where the item doesn't exist
            }
            return View(mediaItem); // Pass the media item to the Edit view
        }

        // Edit Media Item: Update the Media Item after form submission (POST)
        [HttpPost]
        public IActionResult Edit(int id, string Class, string InputTitle, int YearFinished, int Rating, string Review, int? Season)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            if (mediaItem == null)
            {
                return NotFound();
            }

            // Update the media item with the new data
            mediaItem.Class = Class;
            mediaItem.Title = InputTitle;
            mediaItem.YearFinished = YearFinished;
            mediaItem.Rating = Rating;
            mediaItem.Review = Review;
            mediaItem.Season = Class == "Show" ? Season : null; // Only update Season for "Show" items

            SaveMediaList(); // Save the updated list to the file

            TempData["Message"] = "Item updated successfully!";
            return RedirectToAction("ViewList"); // Redirect to the ViewList page after update
        }

        // Delete Media Item: Remove a Media Item
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var mediaItem = MediaList.FirstOrDefault(m => m.Id == id);
            if (mediaItem == null)
            {
                return NotFound();
            }

            MediaList.Remove(mediaItem); // Remove the item from the list
            SaveMediaList(); // Save the updated list to the file

            TempData["Message"] = "Item deleted successfully!";
            return RedirectToAction("ViewList"); // Redirect to the ViewList page
        }

        // Sort Media List (AJAX-based)
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
