using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpGetCalendar {
        private string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string get_calendar_query = @"
            SELECT 
                id, slot_id, row_label, column_label, type_label
            FROM
                calendar
            JOIN
                calendar
            WHERE 
                slot_id = @slot_id;
        ";
        public int slot_id {get;set;}
        public int user_id {get;set;}
        public CalendarModel calendarModel {get;set;} = new CalendarModel();
        public bool Process() {
            Console.WriteLine("process()");
            try {
                using (SqlConnection conn = new SqlConnection(this.connectionQuery)) {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(get_calendar_query, conn)) {
                        command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                CalendarDataModel calendarDataModel = new CalendarDataModel();
                                calendarDataModel.id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                calendarDataModel.slot_id = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                calendarDataModel.row_label = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                                calendarDataModel.column_label = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                                calendarDataModel.type_label = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty;
                                calendarModel.CalendarDataModel.Add(calendarDataModel);
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
    public class HttpPostCalendarChanges {
        public CalendarModel calendarModel;
        public bool Process() {
            try {
                
                return true; 
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
        }
    }
}