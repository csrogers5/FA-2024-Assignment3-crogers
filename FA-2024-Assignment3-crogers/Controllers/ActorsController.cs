using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FA_2024_Assignment3_crogers.Data;
using FA_2024_Assignment3_crogers.Models;
using Humanizer;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using System.Text.Json;
using System.Web;
using VaderSharp2;

namespace FA_2024_Assignment3_crogers.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            List<string> textToExamine = await SearchRedditAsync(actor.Name);

            // Prepare a list to hold Reddit posts and their sentiment scores
            var redditResultsWithSentiment = new List<(string Post, string Sentiment, double Score)>();
            double totalScore = 0;
            int count = 0;

            foreach (var text in textToExamine)
            {
                var (compoundScore, sentiment) = await GetSentimentAsync(text);
                redditResultsWithSentiment.Add((text, sentiment, compoundScore));

                // Only count non-zero scores for average calculation
                if (compoundScore != 0)
                {
                    totalScore += compoundScore;
                    count++;
                }
            }

            double overallSentimentScore = count > 0 ? totalScore / count : 0;

            ViewBag.SearchResultsWithSentiment = redditResultsWithSentiment;
            ViewBag.OverallSentimentScore = overallSentimentScore; // Store the overall score

            return View(actor);
        }

        public static Task<(double, string)> GetSentimentAsync(string text)
        {
            // Instantiate the SentimentIntensityAnalyzer from VaderSharp2
            SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();

            // Get the sentiment scores for the input text
            var results = analyzer.PolarityScores(text);

            // Extract the compound score and classify the sentiment
            double compoundScore = results.Compound;
            string sentimentClassification = ClassifySentiment(compoundScore);

            // Return both the compound score and sentiment classification
            return Task.FromResult((compoundScore, sentimentClassification));
        }

        public static readonly HttpClient client = new HttpClient();

        [HttpGet]
        [HttpPost]
        //Get the text from Reddit related to the actor name
        //List<string> textToExamine = await SearchRedditAsync(actor.Name);
        //Do the sentiment stuff here (watch class video)
        //HINT: DO NOT count values where the compound score is ZERO. Likewise, do not count the item in the total number of items if it is zero.

        public static async Task<List<string>> SearchRedditAsync(string searchQuery)
        {
            var returnList = new List<string>();
            var json = "";
            using (HttpClient client = new HttpClient())
            {
                //fake like you are a "real" web browser
                client.DefaultRequestHeaders.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                json = await client.GetStringAsync("https://www.reddit.com/search.json?limit=100&q=" + HttpUtility.UrlEncode(searchQuery));
            }
            var textToExamine = new List<string>();
            JsonDocument doc = JsonDocument.Parse(json);
            // Navigate to the "data" object
            JsonElement dataElement = doc.RootElement.GetProperty("data");
            // Navigate to the "children" array
            JsonElement childrenElement = dataElement.GetProperty("children");
            foreach (JsonElement child in childrenElement.EnumerateArray())
            {
                if (child.TryGetProperty("data", out JsonElement data))
                {
                    if (data.TryGetProperty("selftext", out JsonElement selftext))
                    {
                        string selftextValue = selftext.GetString();
                        if (!string.IsNullOrEmpty(selftextValue)) { returnList.Add(selftextValue); }
                        else if (data.TryGetProperty("title", out JsonElement title)) //use title if text is empty
                        {
                            string titleValue = title.GetString();
                            if (!string.IsNullOrEmpty(titleValue)) { returnList.Add(titleValue); }
                        }
                    }
                }
            }
            return returnList;
        }

        private static string ClassifySentiment(double score)
        {
            if (score >= 0.75)
            {
                return "very positive";
            }
            else if (0.25 <= score && score < 0.75)
            {
                return "positive";
            }
            else if (0.0 < score && score < 0.25)
            {
                return "slightly positive";
            }
            else if (-0.25 < score && score <= 0.0)
            {
                return "slightly negative";
            }
            else if (-0.75 < score && score <= -0.25)
            {
                return "negative";
            }
            else
            {
                return "very negative";
            }
        }


            // GET: Actors/Create
            public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> GetActorPhoto(int id)
        {
            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            var imageData = actor.ActorImage;

            return File(imageData, "image/jpg");
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,Link,ActorImage")] Actor actor, IFormFile ActorImage)
        {
            if (ModelState.IsValid)
            {
                if (ActorImage != null && ActorImage.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await ActorImage.CopyToAsync(memoryStream);
                    actor.ActorImage = memoryStream.ToArray();
                }
                else
                {
                    actor.ActorImage = new byte[0];
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,Link,ActorImage")] Actor actor, IFormFile ActorImage)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(actor.ActorImage));

            Actor existingActor = _context.Actor.AsNoTracking().FirstOrDefault(m => m.Id == id);

            if (ActorImage != null && ActorImage.Length > 0)
            {
                var memoryStream = new MemoryStream();
                await ActorImage.CopyToAsync(memoryStream);
                actor.ActorImage = memoryStream.ToArray();
            }
            else if (existingActor != null)
            {
                actor.ActorImage = existingActor.ActorImage;
            }
            else
            {
                actor.ActorImage = new byte[0];
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actor.Any(e => e.Id == id);
        }
    }
}
