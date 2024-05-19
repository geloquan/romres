using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpPostDeleteSelectedHosts {
        public List<int> primarySlotId { get; set; } = new List<int>();
        public int? userId { get; set; }
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
        private string delete_query = @"
            DELETE FROM
                @table_name
            WHERE
                slot_id = @slot_id;
        ";
        private string delete_favorites_tagging_query = @"";
        private string delete_invitation_query = @"";
        private string delete_slot_fnl_query = @"";
        private string delete_user_favorites_query = @"";
        private string delete_network_query = @"";
        private string delete_slot_query = @"";
        private string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string GetDeleteQuery(string table_name) {
            string delete_query = $@"
                DELETE FROM
                    {table_name}
                WHERE
                    slot_id = @slot_id;
            ";
            return delete_query;
        }
        private void Delete(int slot_id, SqlConnection conn, SqlTransaction transaction) {
            List<string> table_list = new List<string> { 
                "edge", "favorites_tagging",
                "invitation", "slot_fnl",
                "user_favorites", "slot_network",
                "slot"
            };
            foreach (string table in table_list) {
                string query = GetDeleteQuery(table);
                using (SqlCommand deleteQueryCommand = new SqlCommand(query, conn, transaction)) {
                    deleteQueryCommand.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slot_id;
                    deleteQueryCommand.ExecuteNonQuery();
                }
                transaction.Commit();
            };
        }
        public bool Process() {
            foreach (int slot_id in this.primarySlotId) {
                using (SqlConnection conn = new SqlConnection(this.connectionQuery)) {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(this.get_slot_network_query, conn)) {
                        command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slot_id;
                        List<int> visited = new List<int>();
                        using (SqlDataReader readere = command.ExecuteReader()){
                            while (readere.Read()) {
                                int? first = readere.IsDBNull(0) ? (int?)null : readere.GetInt32(0);
                                int? second = readere.IsDBNull(1) ? (int?)null : readere.GetInt32(1);
                                int? third = readere.IsDBNull(2) ? (int?)null : readere.GetInt32(2);
                                int? fourth = readere.IsDBNull(3) ? (int?)null : readere.GetInt32(3);
                                
                                using (SqlConnection conn2 = new SqlConnection(connectionQuery)) {
                                    conn2.Open();
                                    Console.WriteLine("q");
                                    if (first == 0 || visited.Contains(first.Value)) {
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(first.Value, conn, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("w");
                                    if (second == 0 || visited.Contains(second.Value)) {
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(second.Value, conn, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            return false;
                                        }
                                    }
                                    
                                    Console.WriteLine("e");
                                    if (third == 0 || visited.Contains(third.Value)) {
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(second.Value, conn, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("r");
                                    if (fourth == 0 || visited.Contains(fourth.Value)) {
                                        continue;
                                    }
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            Delete(fourth.Value, conn, transaction);
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            return false;
                                        }
                                    }
                                    Console.WriteLine("t");
                                    return true; 
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
    }
    public class HttpPutParentSlotDuplication {
        private string get_parent_query = @"
            SELECT 
                primary_slot_id
            FROM 
                slot_network
            WHERE
                child_slot_id = @slot_id;
        ";
        private string new_edge_query = @"
            INSERT INTO edge
                (x, y, slot_id)
            VALUES
                (
                    @x,
                    @y,
                    @slot_id
                );
                
        ";
        private string new_network_query = @"
            INSERT INTO 
                slot_network (primary_slot_id, parent_slot_id, child_slot_id)
            VALUES
                (@primary_slot_id, @parent_slot_id, @child_slot_id);
        ";
        private string new_slot_query = @"
            INSERT INTO 
                slot (name, host_id)
            OUTPUT 
                INSERTED.id 
            VALUES
                (@name, @host_id);
        ";
        private string get_network_query = @"
            SELECT 
                sn1.primary_slot_id as root_slot_id,
                sn1.child_slot_id AS second_layer_slot_id,
                sn2.child_slot_id AS third_layer_slot_id,
                s1.name as root_name,
                s2.name as second_layer_name,
                s3.name as third_layer_name,
                e1.x AS root_edge_X,
                e1.y AS root_edge_Y,
                e2.x AS second_layer_edge_X,
                e2.y AS second_layer_edge_Y,
                e3.x AS third_layer_edge_X,
                e3.y AS third_layer_edge_Y
            FROM 
                slot_network sn1
            LEFT JOIN 
                slot_network sn2 ON sn1.child_slot_id = sn2.primary_slot_id
            LEFT JOIN
                slot s1 ON sn1.primary_slot_id = s1.id
            LEFT JOIN
                slot s2 ON sn1.child_slot_id   = s2.id
            LEFT JOIN
                slot s3 ON sn2.child_slot_id   = s3.id
                
            LEFT JOIN 
                edge e1 ON sn1.primary_slot_id = e1.slot_id
            LEFT JOIN 
                edge e2 ON sn1.child_slot_id   = e2.slot_id
            LEFT JOIN 
                edge e3 ON sn2.child_slot_id   = e3.slot_id
                
            WHERE 
                sn1.primary_slot_id = @slot_id;
        ";
        public string slot_info_query = @"
            SELECT 
                e.x AS edge_X,
                e.y AS edge_Y,
                r.duration_start AS reserve_duration_start,
                r.duration_end AS reserve_duration_end,
                s.name AS slot_name,
                s.id AS slot_id,
                h.name,
                h.id
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
            WHERE 
                s.id = @slot_id;
        ";

        public int? hostId { get; set; }
        public int? rootId { get; set; }
        public int? slotId { get; set;}
        public int? parentSlotId { get; set;}
        public HostedSlots newHostedSlots = new HostedSlots();
        private int Duplication(string ConnectionQuery, int slot_id) {
            if (slot_id == null) return 0;
            if (slot_id == 0) return 0;
            Console.WriteLine("Duplication()");
            int newSlotId = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                conn.Open();
                using (SqlCommand command = new SqlCommand(this.slot_info_query, conn)) {
                    Console.WriteLine("Executing slot_info_query");
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = slot_id;
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            double x = reader.IsDBNull(0) ? 0 : reader.GetDouble(0);
                            double y = reader.IsDBNull(1) ? 0 : reader.GetDouble(1);
                            string name = reader.GetString(4);
                            int hostId = reader.GetInt32(7);
                            reader.Close();
                            Console.WriteLine("Executing new_slot_query");
                            using (SqlTransaction transaction = conn.BeginTransaction()) {
                                try {
                                    using (SqlCommand getInfoCommand = new SqlCommand(this.new_slot_query, conn, transaction)) {
                                        getInfoCommand.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 150).Value = name + " (copy)";
                                        getInfoCommand.Parameters.Add("@host_id", System.Data.SqlDbType.Int).Value = hostId;
                                        newSlotId = (int)getInfoCommand.ExecuteScalar();
                                    }
                                    transaction.Commit();
                                } catch (Exception ex) {
                                    transaction.Rollback();
                                    Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                }
                            }
                            Console.WriteLine("Executing new_edge_query");
                            using (SqlTransaction transaction = conn.BeginTransaction()) {
                                try {
                                    using (SqlCommand newEdgeCommand = new SqlCommand(this.new_edge_query, conn, transaction)) {
                                        newEdgeCommand.Parameters.Add("@x", System.Data.SqlDbType.Float).Value = x;
                                        newEdgeCommand.Parameters.Add("@y", System.Data.SqlDbType.Float).Value = y;
                                        newEdgeCommand.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = newSlotId;
                                    }
                                    transaction.Commit();
                                } catch (Exception ex) {
                                    transaction.Rollback();
                                    Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                }
                            }
                            Console.WriteLine("ends");
                        }
                    }
                }
            }
            Console.WriteLine("'exited'");
            return newSlotId;
        }

        public bool Process() {
            Console.WriteLine("process()");
            string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string get_network_query_2 = @"
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
                sn1.primary_slot_id = @slot_id";
            Console.WriteLine(00);
            using (SqlConnection conn = new SqlConnection(connectionQuery)) {
                conn.Open();
                Console.WriteLine(11);
                using (SqlCommand command = new SqlCommand(get_network_query_2, conn)) {
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.slotId;
                    Console.WriteLine("22");
                    using (SqlDataReader readere = command.ExecuteReader()){
                        List<int> visited = new List<int>();
                        while (readere.Read()) {
                            Console.WriteLine("readere.Read()");
                            int? first = readere.IsDBNull(0) ? (int?)null : readere.GetInt32(0);
                            int? second = readere.IsDBNull(1) ? (int?)null : readere.GetInt32(1);
                            int? third = readere.IsDBNull(2) ? (int?)null : readere.GetInt32(2);
                            int? fourth = readere.IsDBNull(3) ? (int?)null : readere.GetInt32(3);
                            
                            int new_first = first.HasValue ? Duplication(connectionQuery, first.Value) : 0;
                            int new_second = second.HasValue ? Duplication(connectionQuery, second.Value) : 0; 
                            int new_third = third.HasValue ? Duplication(connectionQuery, third.Value) : 0; 
                            int new_fourth = fourth.HasValue ? Duplication(connectionQuery, fourth.Value) : 0;
                            
                            using (SqlConnection conn2 = new SqlConnection(connectionQuery)) {
                                conn2.Open();
                                Console.WriteLine("q");
                                if (new_first == 0 || visited.Contains(new_first)) {
                                    continue;
                                }
                                using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    try {
                                        using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                            newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = parentSlotId;
                                            newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                            newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                            newNetworkCommand.ExecuteNonQuery();
                                        }
                                        visited.Add(new_first);
                                        Console.WriteLine("First Network creation successful."); 
                                        transaction.Commit();
                                    } catch (Exception ex) {
                                        transaction.Rollback();
                                        Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during network creation: {ex.Message}");
                                        return false;
                                    }
                                }
                                Console.WriteLine("w");
                                if (new_second == 0 || visited.Contains(new_second)) {
                                    continue;
                                }
                                using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    try {
                                        using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                            newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                            newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                            newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                            newNetworkCommand.ExecuteNonQuery();
                                        }
                                        visited.Add(new_second);
                                        Console.WriteLine("second Network creation successful."); 
                                        transaction.Commit(); // Commit the transaction for host creation
                                    } catch (Exception ex) {
                                        transaction.Rollback();
                                        Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                        return false;
                                    }
                                }
                                
                                
                                Console.WriteLine("e");
                                if (new_third == 0 || visited.Contains(new_third)) {
                                    continue;
                                }
                                using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    try {
                                        using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                            newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                            newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                            newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                            newNetworkCommand.ExecuteNonQuery();
                                        }
                                        visited.Add(new_third);
                                        Console.WriteLine("third Network creation successful."); 
                                        transaction.Commit(); // Commit the transaction for host creation
                                    } catch (Exception ex) {
                                        transaction.Rollback();
                                        Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                        return false;
                                    }
                                }
                                Console.WriteLine("r");
                                if (new_fourth == 0 || visited.Contains(new_fourth)) {
                                    continue;
                                }
                                using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    try {
                                        using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                            newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                            newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                            newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_fourth;
                                            newNetworkCommand.ExecuteNonQuery();
                                        }
                                        visited.Add(new_fourth);
                                        Console.WriteLine("fourth Network creation successful."); 
                                        transaction.Commit(); // Commit the transaction for host creation
                                    } catch (Exception ex) {
                                        transaction.Rollback();
                                        Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during host creation: {ex.Message}");
                                        return false;
                                    }
                                }
                                Console.WriteLine("t");
                                return true; 
                            }
                        }
                    }
                }
            }
            return false; 
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