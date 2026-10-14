using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace lesson2.Models
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsDone).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.CategoryId).HasColumnName("CategoryId");
                entity.Property(e => e.DueDate).HasColumnType("datetime");
                
                // Задача 11: поведение при удалении категории — SET NULL (не удалять задачи!)
                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Tasks)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull); // ✅ При удалении категории у задач CategoryId станет null
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            // Задача 3: стартовые категории через HasData (seed data)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Работа" },
                new Category { Id = 2, Name = "Учёба" },
                new Category { Id = 3, Name = "Личное" }
            );
        }
    }
}
