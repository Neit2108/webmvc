using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BR_Architects.Models.Data
{
    public class ProjectData // CRUD cho bảng Project
    {
        private readonly DbConnection _db;

        public ProjectData(DbConnection db)
        {
            _db = db;
        }

        public List<Project> GetAllProjects()
        {
            string query = "SELECT * FROM Project";
            DataTable dataTable = _db.ExecuteQuery(query);

            List<Project> projects = new List<Project>();
            foreach (DataRow row in dataTable.Rows)
            {
                projects.Add(MapRowToProject(row));
            }

            return projects;
        }

        public Project GetProjectById(int id)
        {
            string query = "SELECT * FROM Project WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            DataTable dataTable = _db.ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count == 0)
                return null;

            return MapRowToProject(dataTable.Rows[0]);
        }

        public int AddProject(Project project)
        {
            string query = "INSERT INTO Project (tenProject, linkAnh, MoTaChiTiet, DiaDiem, NamHoanThanh) " +
                          "VALUES (@TenProject, @LinkAnh, @MoTaChiTiet, @DiaDiem, @NamHoanThanh); " +
                          "SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TenProject", project.TenProject),
                new SqlParameter("@LinkAnh", project.LinkAnh),
                new SqlParameter("@MoTaChiTiet", (object)project.MoTaChiTiet ?? DBNull.Value),
                new SqlParameter("@DiaDiem", (object)project.DiaDiem ?? DBNull.Value),
                new SqlParameter("@NamHoanThanh", (object)project.NamHoanThanh ?? DBNull.Value)
            };

            return Convert.ToInt32(_db.ExecuteScalar(query, parameters));
        }

        public bool UpdateProject(Project project)
        {
            string query = "UPDATE Project SET tenProject = @TenProject, linkAnh = @LinkAnh, " +
                          "MoTaChiTiet = @MoTaChiTiet, DiaDiem = @DiaDiem, NamHoanThanh = @NamHoanThanh " +
                          "WHERE id = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", project.Id),
                new SqlParameter("@TenProject", project.TenProject),
                new SqlParameter("@LinkAnh", project.LinkAnh),
                new SqlParameter("@MoTaChiTiet", (object)project.MoTaChiTiet ?? DBNull.Value),
                new SqlParameter("@DiaDiem", (object)project.DiaDiem ?? DBNull.Value),
                new SqlParameter("@NamHoanThanh", (object)project.NamHoanThanh ?? DBNull.Value)
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteProject(int id)
        {
            string query = "DELETE FROM Project WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            return _db.ExecuteNonQuery(query, parameter) > 0;
        }

        private Project MapRowToProject(DataRow row)
        {
            var project = new Project
            {
                Id = Convert.ToInt32(row["id"]),
                TenProject = row["tenProject"].ToString(),
                LinkAnh = row["linkAnh"].ToString()
            };

            // Xử lý các cột bổ sung nếu chúng tồn tại trong bảng
            if (row.Table.Columns.Contains("MoTaChiTiet") && row["MoTaChiTiet"] != DBNull.Value)
                project.MoTaChiTiet = row["MoTaChiTiet"].ToString();

            if (row.Table.Columns.Contains("DiaDiem") && row["DiaDiem"] != DBNull.Value)
                project.DiaDiem = row["DiaDiem"].ToString();

            if (row.Table.Columns.Contains("NamHoanThanh") && row["NamHoanThanh"] != DBNull.Value)
                project.NamHoanThanh = Convert.ToInt32(row["NamHoanThanh"]);

            return project;
        }
    }
}