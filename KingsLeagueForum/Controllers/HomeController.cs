using System.Diagnostics;
using KingsLeagueForum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KingsLeagueForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly KingsLeagueForumContext _context;

        public HomeController(KingsLeagueForumContext context)
        {
            _context = context;
        }


        public async Task <IActionResult> Index()
        {
            var discussions = await _context.Discussion
                .Include(m => m.Comments)
                .Include(m => m.User) // Include the User data
                .OrderByDescending(d => d.CreateDate) // Order by creation date, newest first
                .ToListAsync();
            return View(discussions);
        }

        //ADDED/////////////////////////
        public async Task<IActionResult> Profile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            // Get the user by ID
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Get the user's discussions
            var userDiscussions = await _context.Discussion
                .Where(d => d.ApplicationUserId == id)
                .Include(d => d.Comments)
                .Include(d => d.User)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            // Pass data through ViewBag
            ViewBag.User = user;

            // Return the discussions as the model
            return View(userDiscussions);
        }

        public async Task<IActionResult> GetDiscussion(int id)
        {
            //get discussion by id
            var discussion = await _context.Discussion
                .Include(m => m.Comments)
                    .ThenInclude(c => c.User) // Include user data for each comment
                .Include(m => m.User) // Include the discussion owner's data
                .FirstOrDefaultAsync(p => p.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            // Order comments by CreateDate descending
            if(discussion.Comments != null)
            {
                discussion.Comments = discussion.Comments.OrderByDescending(c => c.CreateDate).ToList();
            }

            return View(discussion); // Pass the photo to the view
        }


        public IActionResult Privacy()
        {
            return View();
        }

   }
}
