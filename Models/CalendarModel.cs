using System.Data.SqlClient;

namespace WebApplication2.Models {
    public class HttpGetCalendar {
        private string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private string get_calendar_query = @"
            SELECT 
                c.id AS calendar_id, 
                c.slot_id AS calendar_slot_id, 
                c.row_label AS calendar_row_label, 
                c.column_label AS calendar_column_label, 
                c.type_label AS calendar_type_label,
                cdp.id AS calendar_data_property_id,
                cdp.[key] AS calendar_data_property_key,
                cdp.value AS calendar_data_property_value
            FROM
                calendar c
            LEFT JOIN
                calendar_data_property cdp ON c.id = cdp.calendar_id
                
            WHERE 
                c.slot_id = @slot_id
            ORDER BY 
                c.id;
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
                        Console.WriteLine("this.slot_id: " + this.slot_id);
                        command.Parameters.Add("@slot_id", System.Data.SqlDbType.Int).Value = this.slot_id;
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            CalendarDataModel calendarDataModel = new CalendarDataModel();
                            CalendarDataPropertyModel calendarDataPropertyModel = new CalendarDataPropertyModel();
                            int? current_calendar_id = null;
                            while (reader.Read()) {
                                if (current_calendar_id.HasValue && current_calendar_id.Value == reader.GetInt32(0)) {
                                    calendarDataPropertyModel = new CalendarDataPropertyModel();
                                    calendarDataPropertyModel.id = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                                    Console.WriteLine($"calendarDataPropertyModel.id: {calendarDataPropertyModel.id}");
                                    
                                    calendarDataPropertyModel.key = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty;
                                    Console.WriteLine($"calendarDataPropertyModel.key: {calendarDataPropertyModel.key}");
                                    
                                    calendarDataPropertyModel.value = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
                                    Console.WriteLine($"calendarDataPropertyModel.value: {calendarDataPropertyModel.value}");
                                    
                                    calendarDataModel.calendarDataPropertyModels.Add(calendarDataPropertyModel);
                                } else {
                                    calendarModel.CalendarDataModel.Add(calendarDataModel);
                                    calendarDataModel = new CalendarDataModel();
                                    
                                    calendarDataModel.id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    Console.WriteLine($"calendarDataModel.id: {calendarDataModel.id}");
                                    
                                    calendarDataModel.slot_id = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                    Console.WriteLine($"calendarDataModel.slot_id: {calendarDataModel.slot_id}");
                                    
                                    calendarDataModel.row_label = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                                    Console.WriteLine($"calendarDataModel.row_label: {calendarDataModel.row_label}");
                                    
                                    calendarDataModel.column_label = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                                    Console.WriteLine($"calendarDataModel.column_label: {calendarDataModel.column_label}");
                                    
                                    calendarDataModel.type_label = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty;
                                    Console.WriteLine($"calendarDataModel.type_label: {calendarDataModel.type_label}");

                                    calendarDataPropertyModel = new CalendarDataPropertyModel();
                                    calendarDataPropertyModel.id = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                                    Console.WriteLine($"calendarDataPropertyModel.id: {calendarDataPropertyModel.id}");
                                    
                                    calendarDataPropertyModel.key = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty;
                                    Console.WriteLine($"calendarDataPropertyModel.key: {calendarDataPropertyModel.key}");
                                    
                                    calendarDataPropertyModel.value = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
                                    Console.WriteLine($"calendarDataPropertyModel.value: {calendarDataPropertyModel.value}");
                                    
                                    calendarDataModel.calendarDataPropertyModels.Add(calendarDataPropertyModel);
                                }
                                current_calendar_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                            }
                            if (calendarDataModel.id != null || calendarDataModel.id != 0) {
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