using System.Data.SqlClient;

namespace WebApplication2.Models {
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