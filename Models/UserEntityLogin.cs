using System.Data;
using System.Data.SqlClient;
namespace WebApplication2.Models {
    public class UserEntityLogin {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public void DirectLogin(int UserId) {
            string query = "SELECT id, name, password FROM dbo.[user] WHERE id = @user_id;";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command = new SqlCommand(query, conn)) {
                        command.Parameters.Add("@user_id", System.Data.SqlDbType.Int, 50).Value = UserId;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader()){
                            while (reader.Read()) {
                                this.Id = reader.GetInt32(0);
                                this.Name = reader.GetString(1);
                                this.Password = reader.GetString(2);
                            } 
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("UserEntityLogin.DirectLogin() : " + e.Message);
            }
        }
        public LoginSuccessModel Verify(LoginModel loginModel) {
            LoginSuccessModel loginSuccessModel = new LoginSuccessModel {
                Valid = false,
                loginModel = loginModel
            };
            string query = "SELECT * FROM dbo.[user] WHERE name = @name AND password = @password;";
            try {
                string ConnectionQuery = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=rom;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection conn = new SqlConnection(ConnectionQuery)) {
                    using (SqlCommand command = new SqlCommand(query, conn)) {
                        command.Parameters.Add("@name", System.Data.SqlDbType.VarChar, 50).Value = loginModel.UserName;
                        command.Parameters.Add("@password", System.Data.SqlDbType.VarChar, 50).Value = loginModel.UserPassword;
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader()){
                            if (reader.HasRows) {
                                loginSuccessModel.Valid = true;
                                return loginSuccessModel;
                            } else {
                                return loginSuccessModel;
                            }
                        };
                    };
                };
            } 
            catch (Exception e) {
                Console.WriteLine("UserEntityLogin.Verify() : " + e.Message);
                return loginSuccessModel;
            }
        }
    }
}