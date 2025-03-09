using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingsLeagueForum.Data;
using KingsLeagueForum.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace KingsLeagueForum.Controllers
{
    [Authorize] // Restrict access to authenticated users only
    public class DiscussionsController : Controller
    {
        private readonly KingsLeagueForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DiscussionsController(
            KingsLeagueForumContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Discussions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Discussion
                .Include(d => d.User)
                .ToListAsync());
        }

        // GET: Discussions/Details/5
        [AllowAnonymous] // Allow unauthenticated users to view discussion details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion
                .Include(d => d.User)
                .Include(d => d.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // GET: Discussions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Discussions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscussionId,Title,Content,ImageFile")] Discussion discussion)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            discussion.ApplicationUserId = user.Id;

            // init date time
            discussion.CreateDate = DateTime.Now;

            //rename the uploaded file
            discussion.ImageFilename = Guid.NewGuid().ToString() + Path.GetExtension(discussion.ImageFile?.FileName);

            if (ModelState.IsValid)
            {
                _context.Add(discussion);
                await _context.SaveChangesAsync();
                
                if (discussion.ImageFile != null) 
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", discussion.ImageFilename);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(discussion);
        }

        // GET: Discussions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion.Include("Comments")
                .Include(d => d.Comments)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the discussion
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || discussion.ApplicationUserId != currentUser.Id)
            {
                // Not the owner - deny access
                return Forbid(); // Returns a 403 Forbidden response
            }

            return View(discussion);
        }

        // POST: Discussions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscussionId,Title,ImageFilename,Content,CreateDate")] Discussion discussion)
        {
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            // Get the original discussion to check ownership
            var originalDiscussion = await _context.Discussion.FindAsync(id);
            if (originalDiscussion == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || originalDiscussion.ApplicationUserId != currentUser.Id)
            {
                return Forbid(); // Not the owner - deny access
            }

            // Preserve the original user ID
            discussion.ApplicationUserId = originalDiscussion.ApplicationUserId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionExists(discussion.DiscussionId))
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
            return View(discussion);
        }

        // GET: Discussions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context.Discussion
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);
            if (discussion == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || discussion.ApplicationUserId != currentUser.Id)
            {
                return Forbid(); // Not the owner - deny access
            }

            return View(discussion);
        }

        // POST: Discussions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussion = await _context.Discussion.FindAsync(id);
            if (discussion != null)
            {
                // Check if the current user is the owner
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null || discussion.ApplicationUserId != currentUser.Id)
                {
                    return Forbid(); // Not the owner - deny access
                }

                // Delete the image file if it exists
                if (!string.IsNullOrEmpty(discussion.ImageFilename))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(),
                                                 "wwwroot", "photos", discussion.ImageFilename);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Discussion.Remove(discussion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussion.Any(e => e.DiscussionId == id);
        }
    }
}
