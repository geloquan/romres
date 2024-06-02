using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpPostDelete {
        public int? user_id {get;set;}
        public int? slot_id {get;set;}
        string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string get_slot_network_query = @"
            SELECT 
                sn1.primary_slot_id AS root_slot_id,
                sn1.child_slot_id AS second_layer_slot_id,
                sn2.child_slot_id AS third_layer_slot_id,
                sn3.child_slot_id AS fourth_layer_slot_id
            FROM 
                slot_network sn1
            LEFT JOIN 
                slot_network sn2 ON sn1.child_slot_id = sn2.primary_slot_id
            LEFT JOIN 
                slot_network sn3 ON sn2.child_slot_id = sn3.primary_slot_id
            WHERE 
                sn1.primary_slot_id = @slot_id;
        ";
        private string delete_slot_query = @"
            
        ";
        private string GetDeleteQuery(string table_name) {
            if ("slot_network" == table_name) {
                return @"
                    DELETE FROM
                        slot_network
                    WHERE
                        primary_slot_id = @slot_id
                    OR 
                        parent_slot_id = @slot_id
                    OR
                        child_slot_id = @slot_id;
                ";
             };
            if ("slot" == table_name) {
                return @"
                    DELETE FROM
                        slot
                    WHERE
                        id = @slot_id;
                ";
             };
            string delete_query = $@"
                DELETE FROM
                    {table_name}
                WHERE
                    slot_id = @slot_id;
            ";
            return delete_query;
        }
        private void Delete(int slot_id, SqlConnection conn, SqlTransaction transaction) {
            Console.WriteLine("delete()");
            List<string> table_list = new List<string> { 
                "edge", "favorites_tagging",
                "invitation", "slot_fnl",
                "user_favorites", "slot_network",
                "slot"
            };
            try {
                foreach (string table in table_list) {
                    string query = GetDeleteQuery(table);
                    using (SqlCommand deleteQueryCommand = new SqlCommand(query, conn, transaction)) {
                        deleteQueryCommand.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slot_id;
                        deleteQueryCommand.ExecuteNonQuery();
                    }
                };
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
            }
            Console.WriteLine("delete() END");
        }
        public bool Process() {
            Console.WriteLine("'process()'");
            try {
                using (SqlConnection conn = new SqlConnection(this.connectionQuery)) {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(get_slot_network_query, conn)) {
                        command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                        List<int> visited = new List<int>();
                        using (SqlDataReader readere = command.ExecuteReader()){
                            while (readere.Read()) {
                                int? first = readere.IsDBNull(0) ? (int?)null : readere.GetInt32(0);
                                int? second = readere.IsDBNull(1) ? (int?)null : readere.GetInt32(1);
                                int? third = readere.IsDBNull(2) ? (int?)null : readere.GetInt32(2);
                                int? fourth = readere.IsDBNull(3) ? (int?)null : readere.GetInt32(3);
                                
                                using (SqlConnection conn2 = new SqlConnection(connectionQuery)) {
                                    conn2.Open();
                                    Console.WriteLine("q first: ");
                                    if (!first.HasValue || visited.Contains(first.Value)) {
                                        Console.WriteLine("continue");
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Console.WriteLine("try");
                                            Delete(first.Value, conn2, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine("Error: " + ex.Message);
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("w second: " );
                                    if (!second.HasValue || visited.Contains(second.Value)) {
                                        Console.WriteLine("continue");
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(second.Value, conn2, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine("Error: " + ex.Message);
                                            return false;
                                        }
                                    }
                                    
                                    Console.WriteLine("e third: ");
                                    if (!third.HasValue || visited.Contains(third.Value)) {
                                        Console.WriteLine("continue");
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(second.Value, conn2, transaction);
                                            Console.WriteLine("continue");
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine("Error: " + ex.Message);
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("r fourth: ");
                                    if (!fourth.HasValue || visited.Contains(fourth.Value)) {
                                        Console.WriteLine("continue");
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(fourth.Value, conn2, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine("Error: " + ex.Message);
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("t");
                                }
                            }
                        }
                    }
                }
                return true; 
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
    public class HttpPutReserve {
        public int? UserId { get; set; }
        public bool? Reserve { get; set; }
        public bool Process() {
            string query = @"

            ";
            try {
                
                return true;
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing HttpPutReserve: {ex.Message}");
                return false;  
            }
        }
    }
    public class HttpGetSlotSearch {
        public string? InvitationCode { get; set; }
        public SlotTree slotTree = new SlotTree();
        private SlotModel SlotInfoQuery(string ConnectionQuery, string Query, int slot_id, int? parent_id, string? parent_name, string? root_slot_name, int? root_slot_id) {
            SlotModel Model = new SlotModel();
            using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                using (SqlCommand command = new SqlCommand(Query, conn)) {
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = slot_id;
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
                            Model.ParentSlotId = parent_id;
                            Model.ParentSlotName = parent_name ?? "";
                            Model.RootSlotId = root_slot_id ?? null;
                            Model.RootSlotName = root_slot_name ?? "";
                            Model.HostName = !reader.IsDBNull(9) ? reader.GetString(9) : string.Empty;
                        }
                    }

                }
            }
            return Model;
        }
        public bool Process() {
            string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string check_code_exists_query = @"
                SELECT slot_id FROM invitation WHERE code = @invitation_code;
            "; 
            string slot_tree_query = @"
            SELECT 
                sn1.primary_slot_id as root_slot_id,
                sn1.child_slot_id AS second_layer_slot_id,
                sn2.child_slot_id AS third_layer_slot_id,
                s1.name as root_name,
                s2.name as second_layer_name,
                s3.name as third_layer_name
            FROM 
                slot_network sn1
            LEFT JOIN 
                slot_network sn2 ON sn1.child_slot_id = sn2.primary_slot_id
            LEFT JOIN
                slot s1 ON sn1.primary_slot_id = s1.id
            LEFT JOIN
                slot s2 ON sn1.child_slot_id = s2.id
            LEFT JOIN
                slot s3 ON sn2.child_slot_id = s3.id
            WHERE 
                sn1.primary_slot_id = @slot_id
            ";
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
                WHERE 
                    s.id = @slot_id;
            ";
            try {
                SlotTree Tree = new SlotTree();
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command_1 = new SqlCommand(check_code_exists_query, conn)) {
                        command_1.Parameters.Add("@invitation_code", System.Data.SqlDbType.VarChar, 150).Value = this.InvitationCode;
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
                                            while (reader_2.Read()) {
                                                int? root_slot_id = reader_2.IsDBNull(0) ? (int?)null : reader_2.GetInt32(0);
                                                int? second_layer_slot_id = reader_2.IsDBNull(1) ? (int?)null : reader_2.GetInt32(1);
                                                int? third_layer_slot_id = reader_2.IsDBNull(2) ? (int?)null : reader_2.GetInt32(2);
                                                string? root_slot_name = reader_2.IsDBNull(3) ? null : reader_2.GetString(3);
                                                string? second_layer_slot_name = reader_2.IsDBNull(4) ? null : reader_2.GetString(4);
                                                string? third_layer_slot_name = reader_2.IsDBNull(5) ? null : reader_2.GetString(5);
                                                
                                                if (root_slot_id != null && root_slot_id != Tree.RootId) {
                                                    Tree.RootSlotModel = SlotInfoQuery(ConnectionQuery, slot_info_query, root_slot_id.Value, null, null, root_slot_name, root_slot_id);
                                                    Tree.RootId = root_slot_id;
                                                    Tree.InvitationCode = Tree.RootSlotModel.InvitationCode;
                                                } 
                                                if (second_layer_slot_id != null && !Tree.SecondLayerExists(second_layer_slot_id.Value)) {
                                                    Tree.AddSecondLayerChildren(SlotInfoQuery(ConnectionQuery, slot_info_query, second_layer_slot_id.Value, Tree.RootId, root_slot_name, root_slot_name, root_slot_id));
                                                    Tree.AddSecondLayer(second_layer_slot_id.Value);
                                                } 
                                                if (third_layer_slot_id != null && !Tree.ThirdLayerExists(third_layer_slot_id.Value)) {
                                                    Tree.AddThirdLayerChildren(SlotInfoQuery(ConnectionQuery, slot_info_query, third_layer_slot_id.Value, second_layer_slot_id.Value, second_layer_slot_name, root_slot_name, root_slot_id));
                                                    Tree.AddThirdLayer(third_layer_slot_id.Value);
                                                } 
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    };
                };
                this.slotTree = Tree;
                return true;
            } 
            catch (Exception e) {
                Console.WriteLine("HttpGetSlotSearch.process() " + e.Message);
                return false;
            }
        }
        
    }
    public class HttpPutSlotNoteEdit {
        public int? UserId { get; set; }
        public int? SlotId { get; set; }
        public string? Note { get; set; }
        public bool Process() {
            string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Console.WriteLine("HttpPutSlotEdit.Process()");
            string query = @"
                UPDATE favorites_tagging
                SET 
                    note = @slot_note
                WHERE 
                    slot_id = @slot_id
                AND 
                    user_id = @user_id;

                IF @@ROWCOUNT = 0
                BEGIN
                    INSERT INTO favorites_tagging (slot_id, user_id, note)
                    VALUES (@slot_id, @user_id, @slot_note);
                END
            ";
            if (UserId.HasValue && SlotId.HasValue) {
                try {
                    using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                        using (SqlCommand command = new SqlCommand(query, conn)) {
                            command.Parameters.Add("@user_id", System.Data.SqlDbType.Int, 50).Value = this.UserId.Value;
                            command.Parameters.Add("@slot_note", System.Data.SqlDbType.VarChar, 150).Value = this.Note;
                            command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = this.SlotId.Value;
                            conn.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0) {
                                return true;
                            } else {
                                return false;
                            }
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine($"Error processing HttpPutSlotNoteEdit.Process(): {ex.Message}");
                    return false;  
                }
            } else {
                Console.WriteLine("UserId, SlotId, or Note is null or empty. Cannot process.");
                return false;
            }
        }
    }

    
}