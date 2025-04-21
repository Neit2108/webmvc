// Models/DAL/TeamMemberDAL.cs - Updated version without phone number

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BR_Architects.Models.Data
{
    public class TeamMemberData // CRUD cho bảng TeamMember
    {
        private readonly DbConnection _db;
        private readonly UserData _userDAL;

        public TeamMemberData(DbConnection db, UserData userDAL)
        {
            _db = db;
            _userDAL = userDAL;
        }

        public List<TeamMember> GetAllTeamMembers()
        {
            string query = "SELECT * FROM TeamMember";
            DataTable dataTable = _db.ExecuteQuery(query);

            List<TeamMember> teamMembers = new List<TeamMember>();
            foreach (DataRow row in dataTable.Rows)
            {
                teamMembers.Add(MapRowToTeamMember(row));
            }

            return teamMembers;
        }

        public TeamMember GetTeamMemberById(int id)
        {
            string query = "SELECT * FROM TeamMember WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            DataTable dataTable = _db.ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count == 0)
                return null;

            return MapRowToTeamMember(dataTable.Rows[0]);
        }

        public int AddTeamMember(TeamMember teamMember)
        {
            string query = "INSERT INTO TeamMember (userId, chucVu, gioiThieu, linkAnh) " +
                          "VALUES (@UserId, @ChucVu, @GioiThieu, @LinkAnh); " +
                          "SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@UserId", teamMember.UserId),
                new SqlParameter("@ChucVu", teamMember.ChucVu),
                new SqlParameter("@GioiThieu", (object)teamMember.GioiThieu ?? DBNull.Value),
                new SqlParameter("@LinkAnh", teamMember.LinkAnh)
            };

            return Convert.ToInt32(_db.ExecuteScalar(query, parameters));
        }

        public bool UpdateTeamMember(TeamMember teamMember)
        {
            string query = "UPDATE TeamMember SET userId = @UserId, chucVu = @ChucVu, " +
                          "gioiThieu = @GioiThieu, linkAnh = @LinkAnh " +
                          "WHERE id = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", teamMember.Id),
                new SqlParameter("@UserId", teamMember.UserId),
                new SqlParameter("@ChucVu", teamMember.ChucVu),
                new SqlParameter("@GioiThieu", (object)teamMember.GioiThieu ?? DBNull.Value),
                new SqlParameter("@LinkAnh", teamMember.LinkAnh)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }
        public bool DeleteTeamMember(int id)
        {
            string query = "DELETE FROM TeamMember WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            return _db.ExecuteNonQuery(query, parameter) > 0;
        }

        private TeamMember MapRowToTeamMember(DataRow row)
        {
            int userId = Convert.ToInt32(row["userId"]);
            var user = _userDAL.GetUserById(userId);
            var teamMember = new TeamMember
            {
                Id = Convert.ToInt32(row["id"]),
                UserId = userId,
                ChucVu = row["chucVu"].ToString(),
                GioiThieu = row["gioiThieu"].ToString(),
                LinkAnh = row["linkAnh"].ToString(),
                User = user
            };

            return teamMember;
        }
    }
}