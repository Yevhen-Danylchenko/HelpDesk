using Microsoft.EntityFrameworkCore;
using HelpDesk.Models;

namespace HelpDesk.Data
{ 
    public class HelpDeskDbContext: DbContext
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets => Set<Ticket>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 1,
                    Title = "Не працює принтер",
                    Description = "Принтер на 3 поверсі не друкує",
                    Status = "Новий",
                    AuthorId = "demo",
                    AuthorName = "ivan",
                    CreatedAt = new DateTime(2026, 03, 15, 10, 0, 0)
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Потрібен доступ до VPN",
                    Description = "Не можу підключитися до корпоративного VPN з дому",
                    Status = "В роботі",
                    AuthorId = "demo",
                    AuthorName = "ivan",
                    CreatedAt = new DateTime(2026, 03, 15, 11, 0, 0)
                },
                new Ticket
                {
                    Id = 3,
                    Title = "Оновити ліцензію Office",
                    Description = "Ліцензія Microsoft Office закінчилась на моєму ПК",
                    Status = "Вирішено",
                    AuthorId = "demo",
                    AuthorName = "ivan",
                    CreatedAt = new DateTime(2026, 03, 15, 12, 30, 0)
                }
            );
        }
    }
}
