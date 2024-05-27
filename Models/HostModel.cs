using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpPostAddChild {
        private string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string get_parent_slot_id_query = @"
            SELECT TOP 1 parent_slot_id FROM slot_network WHERE primary_slot_id = @primary_slot_id;
        ";
        private string slot_creation_query = @"
            INSERT INTO slot(name, host_id, is_reservable) 
            OUTPUT INSERTED.id 
            VALUES (@slot_name, @host_id, @is_reservable);
        ";
        private string get_host_id_query = @"
            SELECT host_id FROM slot WHERE id = @slot_id;
        ";
        private string invitationCreationQuery = @"
            INSERT INTO invitation(generated_by, slot_id, is_one_time_usage, code) 
            VALUES (@generated_by, @slot_id, @is_one_time_usage, @code);
        ";
        private string slot_network_insertion_query = @"
            INSERT INTO slot_network(primary_slot_id, parent_slot_id, child_slot_id)
            VALUES (@primary_slot_id, @parent_slot_id, @child_slot_id);
        ";
        
        public int? slot_id { get; set; }
        public int? user_id {get;set;}
        public string? child_slot_invitation_code {get; set;}
        public string? child_slot_name {get; set;}
        public bool? child_slot_is_reservable {get; set;}
        private int? parent_slot_id { get; set; }
        private int? host_id {get; set;}
        private int? new_child_slot_id {get;set;}
        public bool Process() {
            Console.WriteLine("process() ");
            if (slot_id.HasValue &&
                !string.IsNullOrEmpty(child_slot_invitation_code) &&
                !string.IsNullOrEmpty(child_slot_name) &&
                child_slot_is_reservable.HasValue
                ) {
                try {
                    using (SqlConnection conn = new SqlConnection(connectionQuery)) {
                        conn.Open();
                        Console.WriteLine("get_host_id_query...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(get_host_id_query, conn, transaction)) {
                                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                                    using (SqlDataReader reader = command.ExecuteReader()) {
                                        if (reader.Read()) {
                                            this.host_id = reader.GetInt32(0);
                                        } else {
                                            throw new Exception("Failed to retrieve the new host ID.");
                                        }
                                    }
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing get_host_id_query: {ex.Message}");
                            }
                        }
                        Console.WriteLine("get_parent_slot_id_query...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(get_parent_slot_id_query, conn, transaction)) {
                                    command.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                                    using (SqlDataReader reader = command.ExecuteReader()) {
                                        if (reader.Read()) {
                                            if (!reader.IsDBNull(0)) {
                                                this.parent_slot_id = reader.GetInt32(0);
                                            }
                                            else {
                                                this.parent_slot_id = null;
                                            }
                                        } else {
                                            this.parent_slot_id = null;
                                        }
                                    }
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing get_host_id_query: {ex.Message}");
                            }
                        }
                        Console.WriteLine("slot_creation_query...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(slot_creation_query, conn, transaction)) {
                                    command.Parameters.Add("@slot_name", System.Data.SqlDbType.VarChar, 150).Value = this.child_slot_name;
                                    command.Parameters.Add("@host_id", System.Data.SqlDbType.Int).Value = (object)this.host_id ?? DBNull.Value;
                                    command.Parameters.Add("@is_reservable", System.Data.SqlDbType.Bit).Value = this.child_slot_is_reservable.Value ? 1 : 0;
                                    using (SqlDataReader reader = command.ExecuteReader()) {
                                        if (reader.Read()) {
                                            this.new_child_slot_id = reader.GetInt32(0);
                                        } else {
                                            throw new Exception("Failed to retrieve the new host ID.");
                                        }
                                    }
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing slot_creation_query: {ex.Message}");
                                return false;
                            }
                        }
                        Console.WriteLine("invitationCreationQuery...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(invitationCreationQuery, conn, transaction)) {
                                    command.Parameters.Add("@generated_by", System.Data.SqlDbType.Int).Value = (object)this.host_id ?? DBNull.Value;
                                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.new_child_slot_id;
                                    command.Parameters.Add("@is_one_time_usage", System.Data.SqlDbType.Int).Value = 0;
                                    command.Parameters.Add("@code", System.Data.SqlDbType.VarChar, 150).Value = this.child_slot_invitation_code;
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing invitationCreationQuery: {ex.Message}");
                                return false;
                            }
                        }
                        Console.WriteLine("slot_network_insertion_query...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(slot_network_insertion_query, conn, transaction)) {
                                    command.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                                    command.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = this.parent_slot_id.HasValue ? (object)this.parent_slot_id.Value : DBNull.Value;
                                    command.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = this.new_child_slot_id;
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing slot_network_insertion_query: {ex.Message}");
                                return false;
                            }
                        }
                        Console.WriteLine("slot_network_insertion_query...");
                        using (SqlTransaction transaction = conn.BeginTransaction()) {
                            try {
                                using (SqlCommand command = new SqlCommand(slot_network_insertion_query, conn, transaction)) {
                                    command.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = this.new_child_slot_id;
                                    command.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                                    command.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = DBNull.Value;
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                            } catch (Exception ex) {
                                transaction.Rollback();
                                Console.WriteLine($"Error processing slot_network_insertion_query: {ex.Message}");
                                return false;
                            }
                        }
                    }
                    return true;
                } catch (Exception ex) {
                    Console.WriteLine("Error: " + ex.Message);
                    return false;
                }
            } else {
                return false;
            }
        }
    }
    public class HttpPatchEditSlot {
        public bool Process() {
            return true;
        }
    }
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
                foreach (int slot_id in this.primarySlotId) {
                    Console.WriteLine("slot_id: " + slot_id);
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
                }
                return true; 
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
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
        private int? parentSlotId { get; set;}
        public int? primarySlotId { get; set; }
        public HostedSlots newHostedSlots = new HostedSlots();
        private int Duplication(string ConnectionQuery, int slot_id) {
            Console.WriteLine("Duplication()");
            if (slot_id == null) return 0;
            if (slot_id == 0) return 0;
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
                                        newEdgeCommand.ExecuteNonQuery();
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
        public bool Process2() {
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
            Console.WriteLine("00");
            using (SqlConnection conn = new SqlConnection(connectionQuery)) {
                conn.Open();
                Console.WriteLine("11");
                using (SqlCommand command = new SqlCommand(get_parent_query, conn)) {
                    Console.WriteLine("get_parent_query");
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.primarySlotId;
                    using (SqlDataReader readere = command.ExecuteReader()){
                        if (readere.HasRows) {
                            if (readere.Read()) {
                                this.parentSlotId = readere.GetInt32(0);
                                
                            } else {
                                this.parentSlotId = null;
                            }
                        } else {
                            this.parentSlotId = null;
                        }
                    }
                }
                int? parent_parent_slot_id = null;
                if (this.parentSlotId != null) {
                    using (SqlCommand command = new SqlCommand(get_parent_query, conn)) {
                        Console.WriteLine("get_parent_query");
                        command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.parentSlotId;
                        using (SqlDataReader readere = command.ExecuteReader()){
                            if (readere.HasRows) {
                                if (readere.Read()) {
                                    parent_parent_slot_id = readere.GetInt32(0);
                                    
                                } else {
                                    parent_parent_slot_id = null;
                                }
                            } else {
                                parent_parent_slot_id = null;
                            }
                        }
                    }
                }
                using (SqlCommand command = new SqlCommand(get_network_query_2, conn)) {
                    command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.primarySlotId;
                    using (SqlDataReader readere = command.ExecuteReader()){
                        Dictionary<int, int> duplicatedValues = new Dictionary<int, int>();
                        bool no_parent_no_child = true;
                        while (readere.Read()) {
                            Console.WriteLine("readere.Read()");
                            int? first = readere.IsDBNull(0) ? (int?)null : readere.GetInt32(0);
                            int? second = readere.IsDBNull(1) ? (int?)null : readere.GetInt32(1);
                            int? third = readere.IsDBNull(2) ? (int?)null : readere.GetInt32(2);
                            int? fourth = readere.IsDBNull(3) ? (int?)null : readere.GetInt32(3);
                            
                            int new_first;
                            if (first.HasValue) {
                                if (duplicatedValues.TryGetValue(first.Value, out int duplicatedValue)) {
                                    new_first = duplicatedValue;
                                }
                                else {
                                    new_first = Duplication(connectionQuery, first.Value);
                                    duplicatedValues.Add(first.Value, new_first);
                                }
                            }
                            else {
                                new_first = 0;
                            }
                            
                            int new_second;
                            if (second.HasValue) {
                                if (duplicatedValues.TryGetValue(second.Value, out int duplicatedValue)) {
                                    new_second = duplicatedValue;
                                }
                                else {
                                    new_second = Duplication(connectionQuery, second.Value);
                                    duplicatedValues.Add(second.Value, new_second);
                                }
                            }
                            else {
                                new_second = 0;
                            }
                            
                            int new_third;
                            if (third.HasValue) {
                                if (duplicatedValues.TryGetValue(third.Value, out int duplicatedValue)) {
                                    new_third = duplicatedValue;
                                }
                                else {
                                    new_third = Duplication(connectionQuery, third.Value);
                                    duplicatedValues.Add(third.Value, new_third);
                                }
                            }
                            else {
                                new_third = 0;
                            }
                            
                            int new_fourth;
                            if (fourth.HasValue) {
                                if (duplicatedValues.TryGetValue(fourth.Value, out int duplicatedValue)) {
                                    new_fourth = duplicatedValue;
                                }
                                else {
                                    new_fourth = Duplication(connectionQuery, fourth.Value);
                                    duplicatedValues.Add(fourth.Value, new_fourth);
                                }
                            }
                            else {
                                new_fourth = 0;
                            }
                            
                            if (this.parentSlotId != null) {
                                Console.WriteLine("thisparentslotid: " + this.parentSlotId);
                                Console.WriteLine("first: " + first + " - second: " + second + " - third: " + third + " - fourth: " + fourth + "\n==");
                                Console.WriteLine("first: " + new_first + " - second: " + new_second + " - third: " + new_third + " - fourth: " + new_fourth);
                                using (SqlConnection conn2 = new SqlConnection(connectionQuery)) {
                                    conn2.Open();
                                    Console.WriteLine("new parent slot child creating... ");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            if (parent_parent_slot_id.HasValue) {
                                                using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                    newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = this.parentSlotId;
                                                    newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = parent_parent_slot_id.Value;
                                                    newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                    newNetworkCommand.ExecuteNonQuery();
                                                }
                                                Console.WriteLine("First new parent slot has parent parent."); 
                                                transaction.Commit();
                                            } else {
                                                using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                    newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = this.parentSlotId;
                                                    newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                                    newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                    newNetworkCommand.ExecuteNonQuery();
                                                }
                                                Console.WriteLine("First new parent slot has NO parent parent."); 
                                                transaction.Commit();
                                            }
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new parent slot: {ex.Message}");
                                        }
                                    }
                                    
                                    Console.WriteLine("q");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = this.parentSlotId;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("First q."); 
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_first new_second: {ex.Message}");
                                        }
                                    }
                                    Console.WriteLine("q11");
                                    if (new_first != 0 && new_second == 0) {
                                        using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                            try {
                                                using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                    newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                    newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = this.parentSlotId;
                                                    newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                                    newNetworkCommand.ExecuteNonQuery();
                                                }
                                                Console.WriteLine("First q11."); 
                                                transaction.Commit();
                                            } catch (Exception ex) {
                                                transaction.Rollback();
                                                Console.WriteLine($"Error processing new_first != 0 || new_second != 0: {ex.Message}");
                                            }
                                        }
                                        continue;
                                    } 
                                    //Console.WriteLine("11ww");
                                    //if (new_second != 0 && new_third == 0) {
                                    //    Console.WriteLine("new_second != 0 && new_third == 0");
                                    //    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    //        try {
                                    //            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                    //                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                    //                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                    //                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                    //                newNetworkCommand.ExecuteNonQuery();
                                    //            }
                                    //            Console.WriteLine("First 11ww."); 
                                    //            transaction.Commit();
                                    //        } catch (Exception ex) {
                                    //            transaction.Rollback();
                                    //            Console.WriteLine($"Error processing new_first != 0 || new_second != 0: {ex.Message}");
                                    //            return false;
                                    //        }
                                    //    }
                                    //    continue;
                                    //}
                                    //Console.WriteLine("qq221");
                                    //if (new_third != 0 && new_fourth == 0) {
                                    //    Console.WriteLine("new_third != 0 && new_fourth == 0");
                                    //    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                    //        try {
                                    //            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                    //                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                    //                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                    //                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                    //                newNetworkCommand.ExecuteNonQuery();
                                    //            }
                                    //            Console.WriteLine("First qq221."); 
                                    //            transaction.Commit();
                                    //        } catch (Exception ex) {
                                    //            transaction.Rollback();
                                    //            Console.WriteLine($"Error processing new_first != 0 || new_second != 0: {ex.Message}");
                                    //            return false;
                                    //        }
                                    //    }
                                    //    continue;
                                    //}
                                    Console.WriteLine("w");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("second Network creation successful."); 
                                            transaction.Commit(); // Commit the transaction for host creation
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_second new_third: {ex.Message}");
                                        }
                                    }
                                    
                                    
                                    Console.WriteLine("e");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_fourth;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("third Network creation successful."); 
                                            transaction.Commit(); // Commit the transaction for host creation
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_third new_fourth: {ex.Message}");
                                        }
                                    }
                                    Console.WriteLine("t");
                                }
                            } else {
                                Console.WriteLine("thisparentslotid: <null>");
                                Console.WriteLine("first: " + new_first + " - second: " + new_second + " - third: " + new_third + " - fourth: " + new_fourth);
                                using (SqlConnection conn2 = new SqlConnection(connectionQuery)) {
                                    conn2.Open();
                                    Console.WriteLine("q11");
                                    if (new_first != 0 && new_second == 0) {
                                        using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                            try {
                                                using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                    newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                    newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                                    newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                                    newNetworkCommand.ExecuteNonQuery();
                                                }
                                                Console.WriteLine("NO PARENT NO CHILD."); 
                                                transaction.Commit();
                                                return true;
                                            } catch (Exception ex) {
                                                transaction.Rollback();
                                                Console.WriteLine($"Error processing HttpPutSlotEdit.Process() during network creation: {ex.Message}");
                                                return false;
                                            }
                                        }
                                    }
                                    Console.WriteLine("q");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = (object)null ?? DBNull.Value;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("First Network creation successful."); 
                                            transaction.Commit();
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_first new_second: {ex.Message}");
                                        }
                                    }
                                    Console.WriteLine("w");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = new_first;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("second Network creation successful."); 
                                            transaction.Commit(); // Commit the transaction for host creation
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_second new_third: {ex.Message}");
                                        }
                                    }
                                    
                                    
                                    Console.WriteLine("e");
                                    using (SqlTransaction transaction = conn2.BeginTransaction()) {
                                        try {
                                            using (SqlCommand newNetworkCommand = new SqlCommand(this.new_network_query, conn2, transaction)) {
                                                newNetworkCommand.Parameters.Add("@primary_slot_id", System.Data.SqlDbType.Int).Value = new_third;
                                                newNetworkCommand.Parameters.Add("@parent_slot_id", System.Data.SqlDbType.Int).Value = new_second;
                                                newNetworkCommand.Parameters.Add("@child_slot_id", System.Data.SqlDbType.Int).Value = new_fourth;
                                                newNetworkCommand.ExecuteNonQuery();
                                            }
                                            Console.WriteLine("third Network creation successful."); 
                                            transaction.Commit(); // Commit the transaction for host creation
                                        } catch (Exception ex) {
                                            transaction.Rollback();
                                            Console.WriteLine($"Error processing new_third new_fourth: {ex.Message}");
                                        }
                                    }
                                    Console.WriteLine("t");
                                }
                            }
                        }
                        return true; 
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
            string invitationCreationQuery = @"
                INSERT INTO invitation(generated_by, slot_id, is_one_time_usage, code) 
                VALUES (@generated_by, @slot_id, @is_one_time_usage, @code);
            ";
            string slotNetworkInsertionQuery = @"
                INSERT INTO slot_network(primary_slot_id, parent_slot_id, child_slot_id)
                VALUES (@slot_id, NULL, NULL);
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