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
        public bool? IsReservable { get; set; }
        public string? InvitationCode { get; set; }
        public string? Note { get; set; }
        public bool Process() {
            string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            Console.Write("HttpPutSlotEdit.Process()");
            string query = @"
                UPDATE features ft
                SET 
                    ft.note = @slot_note
                WHERE 
                    ft.slot_id = @slot_id
                AND 
                    ft.user_id = @user_id;
            ";
            try {
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {

                }
                return true;
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing HttpPutSlotEdit: {ex.Message}");
                return false;  
            }
        }
    }
}