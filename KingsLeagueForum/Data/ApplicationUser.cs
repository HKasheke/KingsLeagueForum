using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace KingsLeagueForum.Data
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData] // property is included in download of personal data.
        public string Name { get; set; } = string.Empty;

        [PersonalData]
        public string? Location { get; set; } = string.Empty;

        [PersonalData]
        public string? ImageFilename { get; set; } = string.Empty;

        [PersonalData]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
