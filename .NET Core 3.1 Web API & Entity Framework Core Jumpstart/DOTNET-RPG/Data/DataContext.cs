using DOTNET_RPG.Models;
using Microsoft.EntityFrameworkCore;

namespace DOTNET_RPG.Data
{
    public class DataContext:DbContext
    {
       public DataContext(DbContextOptions<DataContext> options):base(options){} 

       public DbSet<Character> Characters {get;set;}
       public DbSet<User> Users { get; set; }
       public DbSet<Weapon> Weapons { get; set; }
       public DbSet<Skill> Skills { get; set; }
       public DbSet<CharacterSkill> CharacterSkills { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           modelBuilder.Entity<CharacterSkill>()
                       .HasKey(c=>new {c.CharacterId,c.SkillId});
            
            modelBuilder.Entity<User>()
                       .Property(user=>user.Role).HasDefaultValue("Player");
       }
    }
}