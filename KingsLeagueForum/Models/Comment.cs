using KingsLeagueForum.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingsLeagueForum.Models
{
    public class Comment
    {
        // Primary Key
        public int CommentId { get; set; }

        public string Content { get; set; } = string.Empty;

        [Display(Name = "Created")]
        public DateTime CreateDate { get; set; }

        public int DiscussionId { get; set; } //foreign key

        // Navigation property to Discussion
        [ForeignKey("DiscussionId")]
        public Discussion? Discussion { get; set; }

        // Foreign key property for user
        public string? ApplicationUserId { get; set; }

        // Navigation property to user
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? User{ get; set; }
    }
}
