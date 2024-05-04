using System.Data;
using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class UserEntityData {
        //public List<SlotModelV2> RootSlots = new List<SlotModelV2>();
        public List<SlotModel> SlotTree = new List<SlotModel>();
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
                    u.name AS reserver_user_name
                FROM 
                    slot s 
                LEFT JOIN 
                    slot_fnl sf ON sf.slot_id = s.id
                LEFT JOIN 
                    reserver r ON sf.reserver_id = r.id
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
                                using (SqlCommand command_2 = new SqlCommand(slot_tree_query, conn)) {
                                    command_2.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = slot_id;
                                    conn.Open();
                                    using (SqlDataReader reader_2 = command_2.ExecuteReader()){
                                        SlotTree Tree = new SlotTree();
                                        while (reader_2.Read()) {
                                            int root_slot_id = reader_2.GetInt32(0);
                                            int second_layer_slot_id = reader_2.GetInt32(1);
                                            int third_layer_slot_id = reader_2.GetInt32(2);
                                            if (root_slot_id != Tree.RootId) {
                                                using (SqlCommand command_root_slot = new SqlCommand(slot_info_query, conn)) {
                                                    command_root_slot.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = root_slot_id;
                                                    conn.Open();
                                                    using (SqlDataReader reader_root_slot = command_2.ExecuteReader()){
                                                        while (reader_root_slot.Read()) {
                                                            
                                                        }
                                                    }
                                                }
                                                Tree.RootId = root_slot_id;
                                            } if (!Tree.SecondLayerExists(second_layer_slot_id)) {
                                                
                                            } if (!Tree.ThirdLayerExists(third_layer_slot_id)) {
                                                
                                            } 
                                        }
                                    }
                                }
                            }
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("AWWWWWWWWWWWW" + e.Message);
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