using System.Data;
using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class UserEntityData {
        public List<SlotModel> SlotTree = new List<SlotModel>();
        public FavoriteSlots favoriteSlots = new FavoriteSlots();
        private List<Host> HostedTreeSlots = new List<Host>();
        private SlotModel SlotInfoQuery(string ConnectionQuery, string Query, int slot_id, int? root_id, int? user_id) {
            SlotModel Model = new SlotModel();
            using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                using (SqlCommand command = new SqlCommand(Query, conn)) {
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = slot_id;
                    command.Parameters.Add("@user_id", System.Data.SqlDbType.Int, 50).Value = user_id;
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader()){
                        while (reader.Read()) {
                            double x = reader.IsDBNull(0) ? 0 : reader.GetSqlDouble(0).Value;
                            double y = reader.IsDBNull(1) ? 0 : reader.GetSqlDouble(1).Value;
                            Model.AddEdge((x, y));
                            DateTime startDate = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2);
                            DateTime endDate = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3);
                            Model.AddDuration((startDate, endDate));           
                            Model.Name = reader.GetString(4);
                            Model.SlotId = reader.GetInt32(5);
                            Model.IsReservable = !reader.IsDBNull(6) ? reader.GetByte(6) != 0 : false;
                            Model.ReserverName = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
                            Model.InvitationCode = !reader.IsDBNull(8) ? reader.GetString(8) : string.Empty;
                            Model.Note = !reader.IsDBNull(9) ? reader.GetString(9) : string.Empty;
                            Model.ParentSlotId = root_id;
                            Model.HostName = !reader.IsDBNull(10) ? reader.GetString(10) : string.Empty;
                        }
                    }

                }
            }
            return Model;
        }
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
                    inv.code AS slot_invitation_code,
                    ft.note AS slot_note,
                    h.name
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
                LEFT JOIN 
                    favorites_tagging ft ON s.id = ft.slot_id AND ft.user_id = @user_id
                WHERE 
                    s.id = @slot_id;
            ";
            FavoriteSlots favoriteSlotsContainer = new FavoriteSlots();
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command_1 = new SqlCommand(slot_user_favorites, conn)) {
                        command_1.Parameters.Add("@user_id", System.Data.SqlDbType.Int, 50).Value = UserId;
                        conn.Open();
                        using (SqlDataReader reader_1 = command_1.ExecuteReader()){
                            while (reader_1.Read()) {
                                Console.WriteLine("umm" + reader_1.GetInt32(0));
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
                                                    Tree.RootSlotModel = SlotInfoQuery(ConnectionQuery, slot_info_query, root_slot_id.Value, null, UserId);
                                                    Tree.RootId = root_slot_id;
                                                } 
                                                if (!Tree.SecondLayerExists(second_layer_slot_id.Value) && second_layer_slot_id != null) {
                                                    Tree.AddSecondLayerChildren(SlotInfoQuery(ConnectionQuery, slot_info_query, second_layer_slot_id.Value, Tree.RootId, UserId));
                                                    Tree.AddSecondLayer(second_layer_slot_id.Value);
                                                } 
                                                if (third_layer_slot_id != null && !Tree.ThirdLayerExists(third_layer_slot_id.Value)) {
                                                    Tree.AddThirdLayerChildren(SlotInfoQuery(ConnectionQuery, slot_info_query, third_layer_slot_id.Value, second_layer_slot_id.Value, UserId));
                                                    Tree.AddThirdLayer(third_layer_slot_id.Value);
                                                    
                                                } 
                                            }
                                            favoriteSlotsContainer.AddSlotTree(Tree);
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
            favoriteSlots = favoriteSlotsContainer;
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