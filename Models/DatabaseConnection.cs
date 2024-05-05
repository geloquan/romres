using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class DatabaseConnect {
        public List<SlotModel> SlotList = new List<SlotModel>();
        public List<SlotModel> RootSlots = new List<SlotModel>();
        public List<SlotModel> favoriteSlots = new List<SlotModel>();
        public List<SlotTree> SlotTree = new List<SlotTree>();
        //public void OnGete() {
        //    string query_1 = @"SELECT
        //        sf.id AS slot_fnl_id,
        //        s.name AS slot_name,
        //        e.x AS edge_x,
        //        e.y AS edge_y,
        //        r.user_id AS reserver_user_id,
        //        r.duration_start AS reservation_start,
        //        r.duration_end AS reservation_end
        //    FROM
        //        slot_fnl sf
        //    JOIN
        //        slot s ON sf.slot_id = s.id
        //    JOIN
        //        edge e ON e.slot_id = s.id
        //    JOIN
        //        reserver r ON sf.reserver_id = r.id;";
        //    try {
        //        string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        //        using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
        //            conn.Open();
        //            using (SqlCommand command = new SqlCommand(query_1, conn)) {
        //                using (SqlDataReader reader = command.ExecuteReader()){
        //                    while (reader.Read()) {
        //                        SlotModel slotModel = new SlotModel();
        //                        slotModel.FnlId = reader.GetInt32(0);
        //                        slotModel.Name = reader.GetString(1);
        //                        slotModel.EdgeX = reader.GetSqlDouble(2);
        //                        slotModel.EdgeY = reader.GetSqlDouble(3);
        //                        slotModel.ReserverId = reader.GetInt32(4);
        //                        slotModel.ReservationStart = reader.GetDateTime(5).ToString();
        //                        slotModel.ReservationEnd = reader.GetDateTime(6).ToString();
        //                        SlotList.Add(slotModel);
        //                    }
        //                };
        //            };
        //        };
        //    } 
        //    catch (Exception e) {
        //        Console.WriteLine("AWWWWWWWWWWWW" + e.Message);
        //    }
        //}
        //public void InitRootSlots() {
        //    string query = @"SELECT
        //    sf.slot_id AS fnl_slot_id,
        //    sf.reserver_id,
        //    s.host_id,
        //    parent.id AS parent_root_id,
        //    parent.name AS parent_root_name,
        //    child.id AS child_root_id,
        //    child.name AS child_root_name,
        //    e.id AS edge_id,
        //    e.x,
        //    e.y,
        //    inv.id AS invitation_id,
        //    inv.code,
        //    inv.is_one_time_usage
        //FROM
        //    slot_fnl sf
        //JOIN
        //    slot s ON sf.slot_id = s.id
        //LEFT JOIN
        //    edge e ON s.id = e.slot_id
        //LEFT JOIN
        //    slot parent ON s.parent_root_id = parent.id
        //LEFT JOIN
        //    slot child ON s.child_root_id = child.id
        //LEFT JOIN
        //    invitation inv ON s.id = inv.slot_id
        //WHERE 
        //    parent.id IS NULL;";
        //}
    }
}