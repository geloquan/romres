namespace WebApplication2.Models {
    public class SlotModel {
        public int? PrimarySlotId { get; set; }
        public string? PrimarySlotName { get; set; }
        public int? ChildRootId { get; set; }
        public List<SlotModel>? Children { get; set; }
        public int? ReserverId { get; set; }
        public int? HostId { get; set; }
        public int? EdgeId { get; set; }
        public System.Data.SqlTypes.SqlDouble? EdgeX { get; set; }
        public System.Data.SqlTypes.SqlDouble? EdgeY { get; set; }
        public int? InvitationId { get; set; }
        public string? Code { get; set; }
        public bool? IsRervable { get; set; }
        public bool? IsOneTimeUsage { get; set; }
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
    public class SlotTree {
        public List<SlotModel> slots { get; set; } 
    }
}