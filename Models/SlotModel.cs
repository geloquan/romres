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
    public class HttpPutSlotEdit {
        public int? SlotId { get; set; }
        public bool? IsReservable { get; set; }
        public string? InvitationCode { get; set; }
        public string? Note { get; set; }
        public bool Process() {
            Console.Write("HttpPutSlotEdit.Process()");
            string query = @"
                
            ";
            try {
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