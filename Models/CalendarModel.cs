using System.Data.SqlClient;
using Newtonsoft.Json;

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
                                    
                                    calendarDataPropertyModel.key = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty;
                                    
                                    calendarDataPropertyModel.value = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
                                    
                                    calendarDataPropertyModel.calendar_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    
                                    calendarDataModel.calendarDataPropertyModels.Add(calendarDataPropertyModel);
                                } else {
                                    calendarModel.CalendarDataModel.Add(calendarDataModel);
                                    calendarDataModel = new CalendarDataModel();
                                    
                                    calendarDataModel.id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    
                                    calendarDataModel.slot_id = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
                                    
                                    calendarDataModel.row_label = !reader.IsDBNull(2) ? reader.GetString(2) : string.Empty;
                                    
                                    calendarDataModel.column_label = !reader.IsDBNull(3) ? reader.GetString(3) : string.Empty;
                                    
                                    calendarDataModel.type_label = !reader.IsDBNull(4) ? reader.GetString(4) : string.Empty;

                                    calendarDataPropertyModel = new CalendarDataPropertyModel();
                                    calendarDataPropertyModel.id = reader.IsDBNull(5) ? (int?)null : reader.GetInt32(5);
                                    
                                    calendarDataPropertyModel.key = !reader.IsDBNull(6) ? reader.GetString(6) : string.Empty;
                                    
                                    calendarDataPropertyModel.value = !reader.IsDBNull(7) ? reader.GetString(7) : string.Empty;
                                    
                                    calendarDataPropertyModel.calendar_id = reader.IsDBNull(0) ? (int?)null : reader.GetInt32(0);
                                    
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
    public class HttpPatchCalendar {
        string connectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public int? calendar_id {get;set;}
        public int? slot_id {get;set;}
        [JsonProperty("new_properties")]
        public List<CalendarDataPropertyNewModel> new_properties {get;set;} = new List<CalendarDataPropertyNewModel>();
        [JsonProperty("property_to_delete")]
        public List<int> property_to_delete {get;set;} = new List<int>();

        [JsonProperty("calendar_properties")]
        public List<CalendarDataPropertyModel> calendar_properties { get; set; } = new List<CalendarDataPropertyModel>();
        private string update_query = @"
            UPDATE calendar_data_property SET [key] = @key, value = @value WHERE id = @id
        ";
        private string delete_query = @"
            DELETE FROM calendar_data_property WHERE id = @id;
        ";
        private string insert_query = @"
            INSERT INTO 
                calendar_data_property 
                    (calendar_id, [key], value)
            VALUES
                (@calendar_id, @key, @value);
        ";
        public bool Process() {
            Console.WriteLine("Process()");
            try {
                using (var connection = new SqlConnection(connectionQuery)) {
                    connection.Open();
                    Console.WriteLine("connection: ");
                    using (var transaction = connection.BeginTransaction()) {
                        try {
                            foreach (var property_id in property_to_delete) {
                                Console.WriteLine("property_id: " + property_id);
                                using (var command = new SqlCommand(delete_query, connection, transaction)) {
                                    command.Parameters.AddWithValue("@id", property_id);
                                    command.ExecuteNonQuery();
                                }
                            }
                            foreach (var new_property in new_properties) {
                                Console.WriteLine("new_property.calendar_id: " + new_property.calendar_id);
                                Console.WriteLine("new_property.key: " + new_property.key);
                                Console.WriteLine("new_property.value: " + new_property.value);
                                using (var command = new SqlCommand(insert_query, connection, transaction)) {
                                    command.Parameters.AddWithValue("@calendar_id", new_property.calendar_id);
                                    command.Parameters.AddWithValue("@key", new_property.key);
                                    command.Parameters.AddWithValue("@value", new_property.value);
                                    command.ExecuteNonQuery();
                                }

                            }
                            foreach (var property in calendar_properties) {
                                Console.WriteLine("foreach: ");
                                Console.WriteLine("property: " + property.key);
                                if (property.id.HasValue) {
                                    Console.WriteLine("if: ");

                                    using (var command = new SqlCommand(update_query, connection, transaction)) {
                                        command.Parameters.AddWithValue("@id", property.id.Value);
                                        command.Parameters.AddWithValue("@key", property.key?? (object)DBNull.Value);
                                        command.Parameters.AddWithValue("@value", property.value?? (object)DBNull.Value);

                                        command.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        } catch (Exception ex) {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                return true;
            } catch (Exception ex) {
                Console.WriteLine("Error Process: " + ex.Message);
                return false;
            }
        }
    }
}