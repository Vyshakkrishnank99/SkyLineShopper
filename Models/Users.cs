using System.Data;

namespace SkyLineShoppers.Models
{
    public class Users
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public  int Telephone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string profilePicUrl { get; set; }
        public static Users Create(IDataReader reader)
        {
            var i = new Users
            {
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? "" : reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? "" : reader.GetString(reader.GetOrdinal("LastName")),
                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? "" : reader.GetString(reader.GetOrdinal("Username")),
                Telephone = reader.GetInt32(reader.GetOrdinal("Telephone")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader.GetString(reader.GetOrdinal("Password")),
                profilePicUrl = reader.IsDBNull(reader.GetOrdinal("profilePicUrl")) ? "" : reader.GetString(reader.GetOrdinal("profilePicUrl"))
            };
            return i;
        }

    }
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public static Login Create(IDataReader reader)
        {
            var i = new Login
            {
                Username = reader.IsDBNull(reader.GetOrdinal("Username")) ? "" : reader.GetString(reader.GetOrdinal("Username")),
                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? "" : reader.GetString(reader.GetOrdinal("Password")),
            };
            return i;
        }
    }
    public class UserRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public long Telephone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile File { get; set; } // This is for file upload
    }

}
