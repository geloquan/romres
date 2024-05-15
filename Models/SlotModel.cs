using System.Data.SqlClient;

namespace WebApplication2.Models {
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