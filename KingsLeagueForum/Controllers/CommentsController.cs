using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KingsLeagueForum.Data;
using KingsLeagueForum.Models;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace KingsLeagueForum.Controllers
{
    [Authorize] // Require login to add comment
    public class CommentsController : Controller
    {
        private readonly KingsLeagueForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentsController(
            KingsLeagueForumContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments
        // GET: Comments/Details/5
        // GET: Comments/Edit/5
        // POST: Comments/Edit/5
        // POST: Comments/Delete/5

       
        // GET: Comments/Create/5
        // id = DiscussionId
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Verify the discussion exists
            var discussionExists = _context.Discussion.Any(d => d.DiscussionId == id);
            if (!discussionExists)
            {
                return NotFound();
            }

            // Set the Discussion Id for the comment's fk
            ViewData["DiscussionId"] = id;

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,Content,DiscussionId")] Comment comment)
        {
            // Get the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Set the user ID 
            comment.ApplicationUserId = user.Id;

            //init date time
            comment.CreateDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();

                return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
            }
            ViewData["DiscussionId"] = comment.DiscussionId;
            return View(comment);
        }

        
        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(x => x.DiscussionId)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            // Check if the current user is the owner of the comment
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || comment.ApplicationUserId != currentUser.Id)
            {
                return Forbid(); // Not the owner - deny access
            }

            ViewData["DiscussionId"] = comment.DiscussionId;

            return View(comment);
        }

        
        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.CommentId == id);
        }
    }
}
