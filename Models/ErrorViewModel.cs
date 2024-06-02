using Newtonsoft.Json;
namespace WebApplication2.Models {
    public class EdgeMod {
        [JsonProperty("x")]

        public double X { get; set; }
        [JsonProperty("y")]

        public double Y { get; set; }
    }
    public class DateMod {
        [JsonProperty("start")]
        public string Start { get; set; }
        [JsonProperty("end")]
        public string End { get; set; }
    }
    public class HostedSlots {
        [JsonProperty("slotTrees")]
        public List<SlotTree> SlotTrees { get; set; } = new List<SlotTree>();
        public void AddSlotTree(SlotTree slotTree) {
            this.SlotTrees.Add(slotTree);
        }
    }
    public class FavoriteSlots {
        [JsonProperty("slotTrees")]
        public List<SlotTree> SlotTrees { get; set; } = new List<SlotTree>();
        public void AddSlotTree(SlotTree slotTree) {
            this.SlotTrees.Add(slotTree);
        }
    }
    public class SlotModel {
        [JsonProperty("edge")]
        public HashSet<EdgeMod> Edge { get; set; } = new HashSet<EdgeMod>();

        [JsonProperty("durations")]
        public HashSet<DateMod> Durations { get; set; } = new HashSet<DateMod>();

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("slotId")]
        public int? SlotId { get; set; }

        [JsonProperty("rootSlotId")]
        public int? RootSlotId { get; set; }

        [JsonProperty("rootSlotName")]
        public string? RootSlotName { get; set; }

        [JsonProperty("parentSlotId")]
        public int? ParentSlotId { get; set; }

        [JsonProperty("parentSlotName")]
        public string? ParentSlotName { get; set; }

        [JsonProperty("isReservable")]
        public bool? IsReservable { get; set; }

        [JsonProperty("reserverName")]
        public string? ReserverName { get; set; }

        [JsonProperty("invitationCode")]
        public string? InvitationCode { get; set; }

        [JsonProperty("note")]
        public string? Note { get; set; }

        [JsonProperty("hostName")]
        public string? HostName { get; set; }
        //public int? HostId { get; set; }
        //public int? EdgeId { get; set; }
        //public System.Data.SqlTypes.SqlDouble? EdgeX { get; set; }
        //public System.Data.SqlTypes.SqlDouble? EdgeY { get; set; }
        //public int? InvitationId { get; set; }
        //public bool? IsOneTimeUsage { get; set; }
        public void AddEdge((double, double) edge) {
            this.Edge.Add(new EdgeMod{
                X = edge.Item1,
                Y = edge.Item2
            });
        }
        public void AddDuration((DateTime, DateTime) duration) {
            this.Durations.Add(new DateMod{
                Start = duration.Item1.ToString("MM-dd-yyyy HH:mm:ss"),
                End = duration.Item2.ToString("MM-dd-yyyy HH:mm:ss")
            });
        }
    }
    public class SlotTree {
        [JsonProperty("rootId")]
        public int? RootId { get; set; }

        [JsonProperty("invitationCode")]
        public string? InvitationCode { get; set; }

        [JsonProperty("secondLayerId")]
        public List<int> SecondLayerId { get; set; } = new List<int>();

        [JsonProperty("thirdLayerId")]
        public List<int> ThirdLayerId { get; set; } = new List<int>();

        [JsonProperty("secondLayerName")]
        public List<string> SecondLayerName { get; set; } = new List<string>();

        [JsonProperty("thirdLayerName")]
        public List<string> ThirdLayerName { get; set; } = new List<string>();

        [JsonProperty("rootSlotModel")]
        public SlotModel? RootSlotModel { get; set; }

        [JsonProperty("secondLayerChildren")]
        public List<SlotModel> SecondLayerChildren { get; set; } = new List<SlotModel>();

        [JsonProperty("thirdLayerChildren")]
        public List<SlotModel> ThirdLayerChildren { get; set; } = new List<SlotModel>();
        public void AddSecondLayer(int SlotId) {
            this.SecondLayerId.Add(SlotId);
        }
        public void AddThirdLayer(int SlotId) {
            this.ThirdLayerId.Add(SlotId);
        }public void AddSecondLayerName(string SlotName) {
            this.SecondLayerName.Add(SlotName);
        }
        public void AddThirdLayerName(string SlotName) {
            this.ThirdLayerName.Add(SlotName);
        }
        public bool SecondLayerExists(int SlotId) {
            if (this.SecondLayerId.Contains(SlotId)) {
                return true;
            } else {
                return false;
            }
        } 
        public bool ThirdLayerExists(int SlotId) {
            if (this.ThirdLayerId.Contains(SlotId)) {
                return true;
            } else {
                return false;
            }
        } 
        public void AddSecondLayerChildren(SlotModel slotModel) {
            this.SecondLayerChildren.Add(slotModel);
        }
        public void AddThirdLayerChildren(SlotModel slotModel) {
            this.ThirdLayerChildren.Add(slotModel);
        }
        public void SetRootSlot(SlotModel slotModel) {
            this.RootSlotModel = slotModel;
        }


    }
    public class LoginModel {
        public int? Id { get; set; }
        public string? UserName { get; set; }
        public string? UserPassword { get; set; }
    }
    public class CalendarDataModel {
        [JsonProperty("calendarId")]
        public int? id {get; set; }

        [JsonProperty("slotId")]
        public int? slot_id {get; set; }

        [JsonProperty("rowLabel")]
        public string? row_label {get; set; }

        [JsonProperty("columnLabel")]
        public string? column_label {get; set; }

        [JsonProperty("typeLabel")]
        public string? type_label {get; set; }

        [JsonProperty("isModified")]
        public bool is_modified {get; set; } = false;
    }
    public class CalendarModel {
        [JsonProperty("calendarDataModel")]
        public List<CalendarDataModel> CalendarDataModel { get; set; } = new List<CalendarDataModel>();
    }
}