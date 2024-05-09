namespace WebApplication2.Models {
    public class EdgeMod {
        public double X { get; set; }
        public double Y { get; set; }
    }
    public class DateMod {
        public string Start { get; set; }
        public string End { get; set; }
    }
    public class FavoriteSlots {
        public List<SlotTree> SlotTrees { get; set; } = new List<SlotTree>();
        public void AddSlotTree(SlotTree slotTree) {
            this.SlotTrees.Add(slotTree);
        }
    }
        public class SlotModel {
        public HashSet<EdgeMod> Edge { get; set; } = new HashSet<EdgeMod>();
        public HashSet<DateMod> Durations { get; set; } = new HashSet<DateMod>();
        public string? Name { get; set; }
        public int? SlotId { get; set; }
        public int? ParentSlotId { get; set; }
        public bool? IsReservable { get; set; }
        public string? ReserverName { get; set; }
        public string? InvitationCode { get; set; }
        public string? Note { get; set; }
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
        public List<int> SecondLayerId { get; set; } = new List<int>();
        public List<int> ThirdLayerId { get; set; } = new List<int>();
        public SlotModel? RootSlotModel { get; set; }
        public int? RootId { get; set; }
        public List<SlotModel> SecondLayerChildren { get; set; } = new List<SlotModel>();
        public List<SlotModel> ThirdLayerChildren { get; set; } = new List<SlotModel>();
        public void AddSecondLayer(int SlotId) {
            this.SecondLayerId.Add(SlotId);
        }
        public void AddThirdLayer(int SlotId) {
            this.ThirdLayerId.Add(SlotId);
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
    public class LoginSuccessModel {
        public bool Valid { get; set; }
        public LoginModel? loginModel { get; set; }
    }

    public class SlotParent {
        public int parent_id { get; set; }
    }
    //public class SlotTree {
    //    public List<SlotModel> slots { get; set; } 
    //}
}