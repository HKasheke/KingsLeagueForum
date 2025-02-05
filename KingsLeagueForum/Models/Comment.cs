using System.ComponentModel.DataAnnotations;

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
    }
}
