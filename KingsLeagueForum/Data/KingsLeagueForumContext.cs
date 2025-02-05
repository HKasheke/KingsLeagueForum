using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KingsLeagueForum.Models;

namespace KingsLeagueForum.Data
{
    public class KingsLeagueForumContext : DbContext
    {
        public KingsLeagueForumContext (DbContextOptions<KingsLeagueForumContext> options)
            : base(options)
        {
        }

        public DbSet<KingsLeagueForum.Models.Discussion> Discussion { get; set; } = default!;
        public DbSet<KingsLeagueForum.Models.Comment> Comment { get; set; } = default!;
    }
}
