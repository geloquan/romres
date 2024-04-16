namespace WebApplication2.Models {
    public class ErrorViewModel {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    }
    
    public class Host {
        public int Id { get; set; }
        public string HostName { get; set; }

        public Scope Scope { get; set; }
    }

    public class Scope {
        public int Id { get; set; }

        public List<Level> Levels { get; set; }
    }

    public class Level {
        public int Id { get; set; }
        public int IndexLevel { get; set; }
        public int Reserver { get; set; }

        public List<Slot> Slots { get; set; }
    }

    public class Slot {
        public int Id { get; set; }
        public int Name { get; set; }
        public List<Edge> Edges { get; set; }
        public string Reserver { get; set; }
        public List<DateTime> DateTimes { get; set; }
    }
    public class Edge {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class DateTime {
        public int Date { get; set; }
        public int Reserver { get; set; }
    }
}