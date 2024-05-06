using System.Data;
using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class UserEntityData {
        //public List<SlotModelV2> RootSlots = new List<SlotModelV2>();
        public List<SlotModel> SlotTree = new List<SlotModel>();
        public FavoriteSlots favoriteSlots = new FavoriteSlots();
        private List<Host> HostedTreeSlots = new List<Host>();
        
        public void FavoriteSlots(int UserId) {
            string slot_user_favorites = @"
            SELECT slot_id FROM user_favorites WHERE user_id = @user_id;
            ";
            string slot_tree_query = @"SELECT 
                sn1.primary_slot_id as root_slot_id,
                sn1.child_slot_id AS second_layer_slot_id,
                sn2.child_slot_id AS third_layer_slot_id
            FROM 
                slot_network sn1
            JOIN 
                slot_network sn2 ON sn1.child_slot_id = sn2.primary_slot_id
            WHERE 
                sn1.primary_slot_id = @slot_id;";
            string slot_info_query = @"
                SELECT 
                    e.x AS edge_X,
                    e.y AS edge_Y,
                    r.duration_start AS reserve_duration_start,
                    r.duration_end AS reserve_duration_end,
                    s.name AS slot_name,
                    s.id AS slot_id,
                    s.is_reservable AS slot_is_reservable,
                    u.name AS reserver_user_name,
                    inv.code AS slot_invitation_code
                FROM 
                    slot s 
                LEFT JOIN 
                    slot_fnl sf ON sf.slot_id = s.id
                LEFT JOIN 
                    reserver r ON sf.reserver_id = r.id
                LEFT JOIN 
                    host h ON s.host_id = h.id
                LEFT JOIN 
                    [user] u ON r.user_id = u.id 
                LEFT JOIN 
                    edge e ON sf.slot_id = e.slot_id
                LEFT JOIN
                    invitation inv ON s.id = inv.slot_id                    
                WHERE 
                    s.id = @slot_id;
            ";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command_1 = new SqlCommand(slot_user_favorites, conn)) {
                        command_1.Parameters.Add("@user_id", System.Data.SqlDbType.Int, 50).Value = UserId;
                        conn.Open();
                        using (SqlDataReader reader_1 = command_1.ExecuteReader()){
                            while (reader_1.Read()) {
                                int slot_id = reader_1.GetInt32(0);
                                using (SqlConnection conn_2 = new SqlConnection(ConnectionQuery)) {
                                    using (SqlCommand command_2 = new SqlCommand(slot_tree_query, conn_2)) {
                                        command_2.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = slot_id;
                                        conn_2.Open();
                                        using (SqlDataReader reader_2 = command_2.ExecuteReader()){
                                            SlotTree Tree = new SlotTree();
                                            while (reader_2.Read()) {
                                                int? root_slot_id = reader_2.IsDBNull(0) ? (int?)null : reader_2.GetInt32(0);
                                                int? second_layer_slot_id = reader_2.IsDBNull(1) ? (int?)null : reader_2.GetInt32(1);
                                                int? third_layer_slot_id = reader_2.IsDBNull(2) ? (int?)null : reader_2.GetInt32(2);
                                                if (root_slot_id != Tree.RootId && root_slot_id != null) {
                                                    using (SqlConnection conn_3 = new SqlConnection(ConnectionQuery)) {
                                                        using (SqlCommand command_root_slot = new SqlCommand(slot_info_query, conn_3)) {
                                                            command_root_slot.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = root_slot_id;
                                                            conn_3.Open();
                                                            using (SqlDataReader reader_root_slot = command_root_slot.ExecuteReader()){
                                                                SlotModel Model = new SlotModel();
                                                                while (reader_root_slot.Read()) {
                                                                    System.Data.SqlTypes.SqlDouble x = reader_root_slot.IsDBNull(0) ? reader_root_slot.GetSqlDouble(0) : 0;
                                                                    System.Data.SqlTypes.SqlDouble y = reader_root_slot.IsDBNull(1) ? reader_root_slot.GetSqlDouble(1) : 0;
                                                                    Model.AddEdge((x, y));
                                                                    DateTime startDate = reader_root_slot.IsDBNull(2) ? DateTime.MinValue : reader_root_slot.GetDateTime(2);
                                                                    DateTime endDate = reader_root_slot.IsDBNull(3) ? DateTime.MinValue : reader_root_slot.GetDateTime(3);
                                                                    Model.AddDuration((startDate, endDate));                                                            
                                                                    Model.Name = reader_root_slot.GetString(4);
                                                                    Model.Id = reader_root_slot.GetInt32(5);
                                                                    Model.IsRervable = !reader_root_slot.IsDBNull(7) ? reader_root_slot.GetByte(6) != 0 : false;
                                                                    Model.ReserverName = !reader_root_slot.IsDBNull(7) ? reader_root_slot.GetString(7) : string.Empty;
                                                                    Model.InvitationCode = !reader_root_slot.IsDBNull(8) ? reader_root_slot.GetString(8) : string.Empty;
                                                                }
                                                                Tree.RootSlotModel = Model;
                                                                Tree.RootId = root_slot_id;
                                                            }
                                                        }
                                                    }
                                                } 
                                                if (!Tree.SecondLayerExists(second_layer_slot_id.Value) && second_layer_slot_id != null) {
                                                    using (SqlConnection conn_4 = new SqlConnection(ConnectionQuery)) {
                                                        using (SqlCommand command_second_layer_slot = new SqlCommand(slot_info_query, conn_4)) {
                                                            command_second_layer_slot.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = second_layer_slot_id;
                                                            conn_4.Open();
                                                            using (SqlDataReader reader_second_layer_slot = command_second_layer_slot.ExecuteReader()){
                                                                SlotModel Model = new SlotModel();
                                                                while (reader_second_layer_slot.Read()) {
                                                                    System.Data.SqlTypes.SqlDouble x = reader_second_layer_slot.IsDBNull(0) ? reader_second_layer_slot.GetSqlDouble(0) : 0;
                                                                    System.Data.SqlTypes.SqlDouble y = reader_second_layer_slot.IsDBNull(1) ? reader_second_layer_slot.GetSqlDouble(1) : 0;
                                                                    Model.AddEdge((x, y));
                                                                    DateTime startDate = reader_second_layer_slot.IsDBNull(2) ? DateTime.MinValue : reader_second_layer_slot.GetDateTime(2);
                                                                    DateTime endDate = reader_second_layer_slot.IsDBNull(3) ? DateTime.MinValue : reader_second_layer_slot.GetDateTime(3);
                                                                    Model.AddDuration((startDate, endDate));                                                            
                                                                    Model.Name = reader_second_layer_slot.GetString(4);
                                                                    Model.Id = reader_second_layer_slot.GetInt32(5);
                                                                    Model.IsRervable = !reader_second_layer_slot.IsDBNull(7) ? reader_second_layer_slot.GetByte(6) != 0 : false;
                                                                    Model.ReserverName = !reader_second_layer_slot.IsDBNull(7) ? reader_second_layer_slot.GetString(7) : string.Empty;
                                                                    Model.InvitationCode = !reader_second_layer_slot.IsDBNull(8) ? reader_second_layer_slot.GetString(8) : string.Empty;
                                                                }
                                                                Tree.AddSecondLayer(second_layer_slot_id.Value);
                                                                Tree.AddSecondLayerChildren(Model);
                                                            }
                                                        }
                                                    }
                                                } 
                                                if (third_layer_slot_id != null && !Tree.ThirdLayerExists(third_layer_slot_id.Value)) {
                                                    using (SqlConnection conn_5 = new SqlConnection(ConnectionQuery)) {
                                                        using (SqlCommand command_third_layer_slot = new SqlCommand(slot_info_query, conn_5)) {
                                                            command_third_layer_slot.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = third_layer_slot_id;
                                                            conn_5.Open();
                                                            using (SqlDataReader reader_third_layer_slot = command_third_layer_slot.ExecuteReader()){
                                                                SlotModel Model = new SlotModel();
                                                                while (reader_third_layer_slot.Read()) {
                                                                    System.Data.SqlTypes.SqlDouble x = reader_third_layer_slot.IsDBNull(0) ? reader_third_layer_slot.GetSqlDouble(0) : 0;
                                                                    System.Data.SqlTypes.SqlDouble y = reader_third_layer_slot.IsDBNull(1) ? reader_third_layer_slot.GetSqlDouble(1) : 0;
                                                                    Model.AddEdge((x, y));
                                                                    DateTime startDate = reader_third_layer_slot.IsDBNull(2) ? DateTime.MinValue : reader_third_layer_slot.GetDateTime(2);
                                                                    DateTime endDate = reader_third_layer_slot.IsDBNull(3) ? DateTime.MinValue : reader_third_layer_slot.GetDateTime(3);
                                                                    Model.AddDuration((startDate, endDate));                               
                                                                    Model.Name = reader_third_layer_slot.GetString(4);
                                                                    Model.Id = reader_third_layer_slot.GetInt32(5);
                                                                    Model.IsRervable = !reader_third_layer_slot.IsDBNull(7) ? reader_third_layer_slot.GetByte(6) != 0 : false;
                                                                    Model.ReserverName = !reader_third_layer_slot.IsDBNull(7) ? reader_third_layer_slot.GetString(7) : string.Empty;
                                                                    Model.InvitationCode = !reader_third_layer_slot.IsDBNull(8) ? reader_third_layer_slot.GetString(8) : string.Empty;
                                                                }
                                                                Tree.AddThirdLayer(third_layer_slot_id.Value);
                                                                Tree.AddThirdLayerChildren(Model);
                                                            }
                                                        }
                                                    }
                                                } 
                                            }
                                            favoriteSlots.AddSlotTree(Tree);
                                        }
                                    }
                                }
                            }

                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("AWWWWWWWWWWWW " + e.Message);
            }
        }
        public void Slots() {
            string slot_user_query = @"
            select * from host where user_id = @user_id;
            ";
            string slot_tree_query = @"SELECT 
                sn1.primary_slot_id as root_slot_id,
                sn1.child_slot_id AS second_layer_slot_id,
                sn2.child_slot_id AS third_layer_slot_id
            FROM 
                slot_network sn1
            JOIN 
                slot_network sn2 ON sn1.child_slot_id = sn2.primary_slot_id
            WHERE 
                sn1.primary_slot_id = @slot_id;";
        }
        private class Host {
            public int? Id { get; set; }
            public string? Name { get; set; }
            public int? UserId { get; set; }
        }
    }
}