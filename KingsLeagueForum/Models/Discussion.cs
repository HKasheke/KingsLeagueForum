using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingsLeagueForum.Models
{
    public class Discussion
    {
        public int DiscussionId { get; set; }
       
        public string Title { get; set; } = string.Empty;
        
        public string Content { get; set; } = string.Empty;     
        
        public string ImageFilename { get; set; } = string.Empty;

        [NotMapped]
        [Display(Name = "Photograph")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Created")]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
        public  List<Comment>? Comments { get; set; } 
    }
}
