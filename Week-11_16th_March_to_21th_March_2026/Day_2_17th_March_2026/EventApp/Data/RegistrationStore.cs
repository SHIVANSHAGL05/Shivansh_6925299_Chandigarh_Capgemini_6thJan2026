using EventApp.Models;

namespace EventApp.Data
{
    /// <summary>
    /// Simple static in-memory store — replace with DbContext / EF Core in production.
    /// Shared across all PageModels via static state (fine for this demo scope).
    /// </summary>
    public static class RegistrationStore
    {
        private static int _nextId = 1;

        public static List<EventRegistration> Registrations { get; } = new()
        {
            new EventRegistration { Id = _nextId++, ParticipantName = "Alice Johnson", Email = "alice@example.com",  EventName = "Tech Summit 2025",    RegisteredAt = DateTime.Now.AddDays(-3) },
            new EventRegistration { Id = _nextId++, ParticipantName = "Bob Sharma",    Email = "bob@example.com",    EventName = "AI Workshop",         RegisteredAt = DateTime.Now.AddDays(-2) },
            new EventRegistration { Id = _nextId++, ParticipantName = "Carol White",   Email = "carol@example.com",  EventName = "Tech Summit 2025",    RegisteredAt = DateTime.Now.AddDays(-1) },
        };

        public static void Add(EventRegistration reg)
        {
            reg.Id = _nextId++;
            Registrations.Add(reg);
        }

        public static bool Delete(int id)
        {
            var item = Registrations.FirstOrDefault(r => r.Id == id);
            if (item is null) return false;
            Registrations.Remove(item);
            return true;
        }
    }
}
