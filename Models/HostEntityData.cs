using System.Data;
using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class HostEntityData {
        public List<SlotModelV2> RootSlots = new List<SlotModelV2>();
        public void InitRootSlots(int HostId) {
            string query = @"SELECT
                                sf.slot_id AS primary_slot_id,
                                s.name AS primary_slot_name,
                                parent.id AS parent_slot_id,
                                parent.name AS parent_slot_name,
                                child.id AS child_slot_id,
                                child.name AS child_slot_name,
                                sf.reserver_id,
                                s.host_id,
                                e.id AS edge_id,
                                e.x,
                                e.y,
                                inv.id AS invitation_id,
                                inv.code,
                                inv.is_one_time_usage
                            FROM
                                slot_fnl sf
                            JOIN
                                slot s ON sf.slot_id = s.id
                            LEFT JOIN
                                slot_network sn ON s.id = sn.primary_slot_id
                            LEFT JOIN
                                slot parent ON sn.parent_slot_id = parent.id
                            LEFT JOIN
                                slot child ON sn.child_slot_id = child.id
                            LEFT JOIN
                                edge e ON s.id = e.slot_id
                            LEFT JOIN
                                invitation inv ON s.id = inv.slot_id
                            WHERE 
                                parent_slot_id is null
                            AND
                                s.host_id = @host_id;";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command = new SqlCommand(query, conn)) {
                        command.Parameters.Add("@host_id", System.Data.SqlDbType.Int, 100).Value = HostId;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader()){
                            while (reader.Read()) {
                                SlotModelV2 slotModel = new SlotModelV2();
                                slotModel.PrimarySlotId = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                                slotModel.PrimarySlotName = reader.IsDBNull(1) ? null : reader.GetString(1);
                                slotModel.ParentRootId = reader.IsDBNull(2) ? null : reader.GetInt32(2);
                                slotModel.ParentRootName = reader.IsDBNull(3) ? null : reader.GetString(3);
                                slotModel.ChildRootId = reader.IsDBNull(4) ? null : reader.GetInt32(4);
                                slotModel.ChildRootName = reader.IsDBNull(5) ? null : reader.GetString(5);
                                slotModel.ReserverId = reader.IsDBNull(6) ? null : reader.GetInt32(6);
                                slotModel.HostId = reader.IsDBNull(7) ? null : reader.GetInt32(7);
                                slotModel.EdgeId = reader.IsDBNull(8) ? null : reader.GetInt32(8);
                                slotModel.EdgeX = reader.GetSqlDouble(9);
                                slotModel.EdgeY = reader.GetSqlDouble(10);
                                slotModel.InvitationId = reader.IsDBNull(11) ? null : reader.GetInt32(11);
                                slotModel.Code = reader.IsDBNull(12) ? null : reader.GetString(12);
                                slotModel.IsOneTimeUsage = reader.IsDBNull(13) ? null : (reader.GetByte(13) != 0);
                                RootSlots.Add(slotModel);
                            }
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("HostEntityData/InitRootSlots : " + e.Message);
            }
        }
    }
    public class HostEntityLogin {
        public LoginSuccessModel Verify(LoginModel loginModel, string EntityType) {
            LoginSuccessModel loginSuccessModel = new LoginSuccessModel();
            loginSuccessModel.Valid = false;
            loginSuccessModel.loginModel = loginModel;
            string query = "SELECT * FROM " + EntityType + " WHERE name = @name AND password = @password;";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command = new SqlCommand(query, conn)) {
                        command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = loginModel.UserName;
                        command.Parameters.Add("@password", System.Data.SqlDbType.VarChar, 50).Value = loginModel.UserPassword;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if (reader.HasRows) {
                                loginSuccessModel.Valid = true;
                                return loginSuccessModel;
                            } else {
                                return loginSuccessModel;
                            }
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("LoginSuccessModel Verify : " + e.Message);
                return loginSuccessModel;
            }
        }
    }
}