namespace WebApplication2.Models {
    public class ErrorViewModel {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    }
    
    public class Host {
        public int HostId { get; set; }
        public string HostName { get; set; }
    }

    public class Scope
    {
        public int ScopeId { get; set; }
        public int HostId { get; set; }
        public string ScopeName { get; set; }

        // Navigation property
        public Host Host { get; set; }
    }

    public class Level
    {
        public int LevelId { get; set; }
        public int ScopeId { get; set; }
        public string LevelName { get; set; }

        // Navigation property
        public Scope Scope { get; set; }
    }

    public class IndexLevel
    {
        public int IndexLevelId { get; set; }
        public int LevelId { get; set; }
        public string IndexLevelName { get; set; }

        // Navigation property
        public Level Level { get; set; }
    }

    public class Reservee
    {
        public int ReserveeId { get; set; }
        public string ReserveeName { get; set; }
    }

    public class Slot
    {
        public int SlotId { get; set; }
        public int IndexLevelId { get; set; }
        public DateTime SlotDateTime { get; set; }

        // Navigation property
        public IndexLevel IndexLevel { get; set; }
    }

    public class Edge
    {
        public int EdgeId { get; set; }
        public int SlotId { get; set; }
        public int EdgeX { get; set; }
        public int EdgeY { get; set; }

        // Navigation property
        public Slot Slot { get; set; }
    }

    public class SlotReservee
    {
        public int SlotId { get; set; }
        public int ReserveeId { get; set; }

        // Navigation properties
        public Slot Slot { get; set; }
        public Reservee Reservee { get; set; }
    }
}