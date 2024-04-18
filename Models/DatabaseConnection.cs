using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class DatabaseConnect {
        public List<SlotModel> SlotList = new List<SlotModel>();

        public void OnGete() {
            SlotModel slotModele = new SlotModel();
            slotModele.FnlId = 1;
            slotModele.Name = "reader.GetString(1)";
            slotModele.EdgeX = 1;
            slotModele.EdgeY = 1;
            slotModele.ReserverId = 1;
            slotModele.ReservationStart = "reader.GetDateTime(5).ToString()";
            slotModele.ReservationEnd = "reader.GetDateTime(6).ToString()";
            SlotList.Add(slotModele);
            string query_1 = @"SELECT
    sf.id AS slot_fnl_id,
    s.name AS slot_name,
    e.x AS edge_x,
    e.y AS edge_y,
    r.user_id AS reserver_user_id,
    r.duration_start AS reservation_start,
    r.duration_end AS reservation_end
FROM
    slot_fnl sf
JOIN
    slot s ON sf.slot_id = s.id
JOIN
    edge e ON e.slot_id = s.id
JOIN
    reserver r ON sf.reserver_id = r.id;";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(query_1, conn)) {
                        using (SqlDataReader reader = command.ExecuteReader()){
                            while (reader.Read()) {
                                SlotModel slotModel = new SlotModel();
                                slotModel.FnlId = reader.GetInt32(0);
                                slotModel.Name = reader.GetString(1);
                                slotModel.EdgeX = reader.GetSqlDouble(2);
                                slotModel.EdgeY = reader.GetSqlDouble(3);
                                slotModel.ReserverId = reader.GetInt32(4);
                                slotModel.ReservationStart = reader.GetDateTime(5).ToString();
                                slotModel.ReservationEnd = reader.GetDateTime(6).ToString();
                                SlotList.Add(slotModel);
                            }
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("AWWWWWWWWWWWW" + e.Message);
            }
        }
    }
}