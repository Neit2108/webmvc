// Models/DAL/UserDAL.cs
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BR_Architects.Models.Data
{
    public class UserData // CRUD cho bảng Users
    {
        private readonly DbConnection _db;

        public UserData(DbConnection db)
        {
            _db = db;
        }

        public User GetUserByEmail(string email)
        {
            string query = "SELECT * FROM Users WHERE email = @Email";
            SqlParameter parameter = new SqlParameter("@Email", email);

            DataTable dataTable = _db.ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count == 0)
                return null;

            DataRow row = dataTable.Rows[0];
            User user = new User
            {
                Id = Convert.ToInt32(row["id"]),
                HoTen = row["hoTen"].ToString(),
                Email = row["email"].ToString(),
                Pass = row["pass"].ToString(),
                SecretKey = row["SecretKey"]?.ToString()
            };

            // Check if the Role column exists in the table
            if (row.Table.Columns.Contains("Role"))
            {
                // Check if the Role value is "Admin" (case-insensitive)
                user.IsAdmin = row["Role"]?.ToString()?.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
            }

            return user;
        }

        public User GetUserById(int id)
        {
            string query = "SELECT * FROM Users WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            DataTable dataTable = _db.ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count == 0)
                return null;

            DataRow row = dataTable.Rows[0];
            User user = new User
            {
                Id = Convert.ToInt32(row["id"]),
                HoTen = row["hoTen"].ToString(),
                Email = row["email"].ToString(),
                Pass = row["pass"].ToString(),
                SecretKey = row["SecretKey"]?.ToString()
            };

            // Check if the Role column exists in the table
            if (row.Table.Columns.Contains("Role"))
            {
                // Check if the Role value is "Admin" (case-insensitive)
                user.IsAdmin = row["Role"]?.ToString()?.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
            }

            return user;
        }

        public List<User> GetAllUsers()
        {
            string query = "SELECT * FROM Users";
            DataTable dataTable = _db.ExecuteQuery(query);

            List<User> users = new List<User>();
            foreach (DataRow row in dataTable.Rows)
            {
                User user = new User
                {
                    Id = Convert.ToInt32(row["id"]),
                    HoTen = row["hoTen"].ToString(),
                    Email = row["email"].ToString(),
                    Pass = row["pass"].ToString(),
                    SecretKey = row["SecretKey"]?.ToString()
                };

                // Check if the Role column exists in the table
                if (row.Table.Columns.Contains("Role"))
                {
                    // Check if the Role value is "Admin" (case-insensitive)
                    user.IsAdmin = row["Role"]?.ToString()?.Trim().Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;
                }

                users.Add(user);
            }

            return users;
        }

        public bool UserExistsByEmail(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE email = @Email";
            SqlParameter parameter = new SqlParameter("@Email", email);

            return Convert.ToInt32(_db.ExecuteScalar(query, parameter)) > 0;
        }

        public int AddUser(User user)
        {
            // Check if Role is a column in the Users table
            string roleParam = string.Empty;
            string roleValue = string.Empty;

            // Attempt to check if Role column exists
            try
            {
                string checkQuery = "SELECT COL_NAME(ic.OBJECT_ID, ic.column_id) FROM sys.columns ic " +
                                    "WHERE ic.OBJECT_ID = OBJECT_ID('Users') AND ic.name = 'Role'";
                object result = _db.ExecuteScalar(checkQuery);
                if (result != null && result != DBNull.Value)
                {
                    roleParam = ", Role";
                    roleValue = ", @Role";
                }
            }
            catch (Exception)
            {
                // If there's an error, assume Role column doesn't exist
            }

            string query = $"INSERT INTO Users (hoTen, email, pass, SecretKey{roleParam}) " +
                         $"VALUES (@HoTen, @Email, @Pass, @SecretKey{roleValue}); " +
                         "SELECT SCOPE_IDENTITY();";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@HoTen", user.HoTen),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Pass", user.Pass),
                new SqlParameter("@SecretKey", user.SecretKey ?? (object)DBNull.Value)
            };

            if (!string.IsNullOrEmpty(roleParam))
            {
                parameters.Add(new SqlParameter("@Role", user.IsAdmin ? "Admin" : "User"));
            }

            return Convert.ToInt32(_db.ExecuteScalar(query, parameters.ToArray()));
        }

        public bool UpdateUser(User user)
        {
            // Check if Role is a column in the Users table
            string roleUpdate = string.Empty;

            // Attempt to check if Role column exists
            try
            {
                string checkQuery = "SELECT COL_NAME(ic.OBJECT_ID, ic.column_id) FROM sys.columns ic " +
                                    "WHERE ic.OBJECT_ID = OBJECT_ID('Users') AND ic.name = 'Role'";
                object result = _db.ExecuteScalar(checkQuery);
                if (result != null && result != DBNull.Value)
                {
                    roleUpdate = ", Role = @Role";
                }
            }
            catch (Exception)
            {
                // If there's an error, assume Role column doesn't exist
            }

            string query = $"UPDATE Users SET hoTen = @HoTen, email = @Email, pass = @Pass, SecretKey = @SecretKey{roleUpdate} WHERE id = @Id";

            List<SqlParameter> parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", user.Id),
                new SqlParameter("@HoTen", user.HoTen),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Pass", user.Pass),
                new SqlParameter("@SecretKey", user.SecretKey ?? (object)DBNull.Value)
            };

            if (!string.IsNullOrEmpty(roleUpdate))
            {
                parameters.Add(new SqlParameter("@Role", user.IsAdmin ? "Admin" : "User"));
            }

            return _db.ExecuteNonQuery(query, parameters.ToArray()) > 0;
        }

        public bool DeleteUser(int id)
        {
            string query = "DELETE FROM Users WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            return _db.ExecuteNonQuery(query, parameter) > 0;
        }
    }
}