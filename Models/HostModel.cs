using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpPutParentSlotDuplication {
        public string? hostId { get; set; }
        public string? parentSlotId { get; set;}
        public HostedSlots newHostedSlots = new HostedSlots();
        private SlotModel SlotDuplication(string ConnectionQuery, string Query, int slot_id, string get_info_query, int? root_id) {
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
                            Model.ParentSlotId = root_id;
                            Model.HostName = !reader.IsDBNull(9) ? reader.GetString(9) : string.Empty;
                        }
                    }
                }
            }
            return Model;
        }
        public bool Process() {
            return true;
            string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string new_info_query = @"
                INSERT INTO 
            ";
            string get_info_query = @"
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
                    s.id = @slot_id";
            string get_parent_query = @"
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
                    sn1.primary_slot_id = @slot_id;
            ";
            using (SqlConnection conn = new SqlConnection(connectionQuery)) {
                conn.Open();
                using (SqlCommand command = new SqlCommand(get_parent_query, conn)) {
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int, 50).Value = parentSlotId;
                    using (SqlDataReader reader = command.ExecuteReader()){
                        SlotTree Tree = new SlotTree();
                        List<int> visited = new List<int>();
                        while (reader.Read()) {
                            int? root_slot_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                            int? second_layer_slot_id = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                            int? third_layer_slot_id = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                            if (root_slot_id != Tree.RootId && root_slot_id != null && !visited.Contains(root_slot_id.Value)) {
                                Tree.RootId = root_slot_id;
                                Tree.InvitationCode = Tree.RootSlotModel.InvitationCode;
                                visited.Add(root_slot_id.Value);
                            } 
                            if (!Tree.SecondLayerExists(second_layer_slot_id.Value) && second_layer_slot_id != null && !visited.Contains(second_layer_slot_id.Value)) {
                            
                            } 
                            if (third_layer_slot_id != null && !Tree.ThirdLayerExists(third_layer_slot_id.Value) && !visited.Contains(third_layer_slot_id.Value)) {
                                
                            } 
                        }
                    }
                }
            }
        }
    }
    public class HttpPutNewHost {
        public string? invitationCode { get; set; }
        public string? slotName { get; set; }
        public string? hostNameTag { get; set; }
        public int? userId { get; set; }
        public int? newSlotId { get; set; }
        public bool Process() {
            string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Console.WriteLine("HttpPutSlotEdit.Process()");

            string hostCreationQuery = @"
                INSERT INTO host(name, user_id) 
                OUTPUT INSERTED.id 
                VALUES (@host_name, @user_id);
            ";

            string slotCreationQuery = @"
                INSERT INTO slot(name, host_id, is_reservable) 
                OUTPUT INSERTED.id 
                VALUES (@slot_name, @host_id, @is_reservable);
            ";
            string slotNetworkInsertionQuery = @"
                INSERT INTO slot_network(primary_slot_id, parent_slot_id, child_slot_id)
                VALUES (@slot_id, NULL, NULL);
            ";
            string invitationCreationQuery = @"
                INSERT INTO invitation(generated_by, slot_id, is_one_time_usage, code) 
                VALUES (@generated_by, @slot_id, @is_one_time_usage, @code);
            ";

            if (!string.IsNullOrEmpty(invitationCode) && !string.IsNullOrEmpty(slotName) && !string.IsNullOrEmpty(hostNameTag) && userId.HasValue) {
                try {
                    using (SqlConnection conn = new SqlConnection(connectionQuery)) {
                        conn.Open();
                        
                        // Insert into host and get the new host ID
                        int hostId;
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand hostCommand = new SqlCommand(hostCreationQuery, conn, transaction)) {
                                    hostCommand.Parameters.Add("@user_id", System.Data.SqlDbType.Int).Value = this.userId.Value;
                                    hostCommand.Parameters.Add("@host_name", System.Data.SqlDbType.VarChar, 150).Value = this.hostNameTag;
                                    using (SqlDataReader reader = hostCommand.ExecuteReader()) {
                                        if (reader.Read()) {
                                            hostId = reader.GetInt32(0); // Assuming the ID is the first column
                                            Console.WriteLine($"New host ID: {hostId}");
                                        } else {
                                            throw new Exception("Failed to retrieve the new host ID.");
                                        }
                                    }
                                }
                                transaction.Commit(); // Commit the transaction for host creation
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                return false;
                            }
                        }

                        // Insert into slot and get the new slot ID
                        int slotId;
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                        using (SqlCommand slotCommand = new SqlCommand(slotCreationQuery, conn, transaction)) {
                            slotCommand.Parameters.Add("@slot_name", System.Data.SqlDbType.VarChar, 150).Value = this.slotName;
                            slotCommand.Parameters.Add("@host_id", System.Data.SqlDbType.Int).Value = hostId;
                            slotCommand.Parameters.Add("@is_reservable", System.Data.SqlDbType.Bit).Value = 0;
                            slotId = (int)slotCommand.ExecuteScalar(); // Get the newly inserted slot ID
                            newSlotId = slotId;
                            Console.WriteLine($"New slot ID: {slotId}");
                        }
                        transaction.Commit(); // Commit the transaction for slot creation
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during slot creation: {ex.Message}");
                                return false;
                            }
                        }

                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand invitationCommand = new SqlCommand(invitationCreationQuery, conn, transaction)) {
                                    invitationCommand.Parameters.Add("@generated_by", System.Data.SqlDbType.Int).Value = this.userId.Value;
                                    invitationCommand.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slotId;
                                    invitationCommand.Parameters.Add("@is_one_time_usage", System.Data.SqlDbType.Bit).Value = 0;
                                    invitationCommand.Parameters.Add("@code", System.Data.SqlDbType.VarChar, 150).Value = this.invitationCode;
                                    invitationCommand.ExecuteNonQuery();
                                }
                                transaction.Commit(); // Commit the transaction for invitation creation
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during slot network insertion: {ex.Message}");
                                return false;
                            }
                        }
                        
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand networkCommand = new SqlCommand(slotNetworkInsertionQuery, conn, transaction)) {
                                    networkCommand.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slotId;
                                    networkCommand.ExecuteNonQuery();
                                }
                                transaction.Commit(); 
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during invitation creation: {ex.Message}");
                                return false;
                            }
                        }

                    }
                    return true;
                } catch (Exception ex) {
                    Console.WriteLine($"Error processing HttpPutSlotEdit.Process(): {ex.Message}");
                    return false;
                }
            } else {
                Console.WriteLine("UserId, SlotId, or Note is null or empty. Cannot process.");
                return false;
            }
        }

    }
}