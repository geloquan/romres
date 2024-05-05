namespace WebApplication2.Models {
    public class FavoriteSlots {
        public List<SlotTree> SlotTrees { get; set; } = new List<SlotTree>();
        public void AddSlotTree(SlotTree slotTree) {
            this.SlotTrees.Add(slotTree);
        }
    }
    public class SlotModel {
        public List<(
        System.Data.SqlTypes.SqlDouble, System.Data.SqlTypes.SqlDouble
        )> Edge { get; set; } = 
        new List<(System.Data.SqlTypes.SqlDouble, System.Data.SqlTypes.SqlDouble)>();
        public List<(DateTime, DateTime)> Durations { get; set; } = new List<(DateTime, DateTime)>();
        public string? Name { get; set; }
        public int? Id { get; set; }
        public bool? IsRervable { get; set; }
        public string? ReserverName { get; set; }
        public string? InvitationCode { get; set; }
        //public int? HostId { get; set; }
        //public int? EdgeId { get; set; }
        //public System.Data.SqlTypes.SqlDouble? EdgeX { get; set; }
        //public System.Data.SqlTypes.SqlDouble? EdgeY { get; set; }
        //public int? InvitationId { get; set; }
        //public bool? IsOneTimeUsage { get; set; }
        public void AddEdge((System.Data.SqlTypes.SqlDouble, System.Data.SqlTypes.SqlDouble) edge) {

        }
        public void AddDuration((DateTime, DateTime) duration) {
            
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