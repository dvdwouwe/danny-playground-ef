// See https://aka.ms/new-console-template for more information


using Danny.Playground.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

await using var context = new MyDbContext();
// Console.WriteLine(context.Model.ToDebugString());
// Console.WriteLine(context.Database.GenerateCreateScript());
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
context.ChangeTracker.Clear();
await context
    .Set<EventWithIdentification>()
    .Where(e => e.Knowledge.To == null)
    .ToListAsync();

await context
    .Set<EventWithIdentificationAndPeriod>()
    .Where(e => e.Knowledge.To == null)
    .ToListAsync();

await context
    .Set<EventWithPartner>()
    .Where(e => e.Knowledge.To == null)
    .ToListAsync();

await context
    .Set<EventWithName>()
    .Where(e => e.Knowledge.To == null)
    .ToListAsync();

namespace Danny.Playground.EF
{
    public abstract class Event
    {
        protected Event(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        public Period Knowledge { get; set; }
    }

    public abstract class EventWithIdentification : Event
    {
        protected EventWithIdentification(int id) : base(id)
        {
        }

        public long ExtraId { get; set; }
    }

    public class EventWithName : EventWithIdentification
    {
        public EventWithName(int id) : base(id)
        {
        }

        public Name PersonName { get; set; }
    }

    public abstract class EventWithIdentificationAndPeriod : EventWithIdentification
    {
        protected EventWithIdentificationAndPeriod(int id) : base(id)
        {
        }

        public Period Execution { get; set; }
    }

    public abstract class EventWithPartner : EventWithIdentificationAndPeriod
    {
        protected EventWithPartner(int id) : base(id)
        {
        }

        public Name PartnerName { get; set; }
    }

    public class EventWithPartnerType1 : EventWithPartner
    {
        public EventWithPartnerType1(int id) : base(id)
        {
        }
    }

    public class EventWithPartnerType2 : EventWithPartner
    {
        public EventWithPartnerType2(int id) : base(id)
        {
        }
    }

    public record struct Period(DateTimeOffset From, DateTimeOffset? To);

    public record struct Name(string FirstName, string LastName);

    public class MyDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"data source=localhost;initial catalog=playground-ef;Trusted_Connection=True")
                .LogTo(Console.WriteLine, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>(builder =>
            {
                builder.ComplexProperty(e => e.Knowledge);
                builder.UseTpcMappingStrategy();
            });

            modelBuilder.Entity<EventWithIdentification>();
            modelBuilder.Entity<EventWithName>(builder => { builder.ComplexProperty(e => e.PersonName); });
            modelBuilder.Entity<EventWithIdentificationAndPeriod>(builder =>
            {
                builder.ComplexProperty(e => e.Execution);
            });
            modelBuilder.Entity<EventWithPartner>(builder => { builder.ComplexProperty(e => e.PartnerName); });
            modelBuilder.Entity<EventWithPartnerType1>();
            modelBuilder.Entity<EventWithPartnerType2>();
        }
    }
}
