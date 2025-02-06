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
            var discussions = await _context.Discussion.ToListAsync();
            return View(discussions);
        }

        public async Task<IActionResult> DiscussionDetails(int id)
        {
            //get discussion by id
            var discussion = await _context.Discussion.Include(m => m.Comments).FirstOrDefaultAsync(p => p.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }
            return View(discussion); // Pass the photo to the view
        }


        public IActionResult Privacy()
        {
            return View();
        }

   }
}
