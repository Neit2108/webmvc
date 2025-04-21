using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BR_Architects.Models.Data
{
    public class ContactData // CRUD cho bảng Contact
    {
        private readonly DbConnection _db;

        public ContactData(DbConnection db)
        {
            _db = db;
        }

        public List<Contact> GetAllContacts()
        {
            string query = "SELECT * FROM Contact";
            DataTable dataTable = _db.ExecuteQuery(query);

            List<Contact> contacts = new List<Contact>();
            foreach (DataRow row in dataTable.Rows)
            {
                contacts.Add(MapRowToContact(row));
            }

            return contacts;
        }

        public Contact GetContactById(int id)
        {
            string query = "SELECT * FROM Contact WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            DataTable dataTable = _db.ExecuteQuery(query, parameter);

            if (dataTable.Rows.Count == 0)
                return null;

            return MapRowToContact(dataTable.Rows[0]);
        }

        public int AddContact(Contact contact)
        {
            string query = "INSERT INTO Contact (nguoiGui, email, tieuDe, noiDung) " +
                          "VALUES (@NguoiGui, @Email, @TieuDe, @NoiDung); " +
                          "SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@NguoiGui", contact.NguoiGui),
                new SqlParameter("@Email", contact.Email),
                new SqlParameter("@TieuDe", contact.TieuDe),
                new SqlParameter("@NoiDung", contact.NoiDung),
            };

            return Convert.ToInt32(_db.ExecuteScalar(query, parameters));
        }

        public bool UpdateContact(Contact contact)
        {
            string query = "UPDATE Contact SET nguoiGui = @NguoiGui, email = @Email, " +
                          "tieuDe = @TieuDe, noiDung = @NoiDung" +
                          "WHERE id = @Id";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", contact.Id),
                new SqlParameter("@NguoiGui", contact.NguoiGui),
                new SqlParameter("@Email", contact.Email),
                new SqlParameter("@TieuDe", contact.TieuDe),
                new SqlParameter("@NoiDung", contact.NoiDung),
            };

            return _db.ExecuteNonQuery(query, parameters) > 0;
        }

        public bool DeleteContact(int id)
        {
            string query = "DELETE FROM Contact WHERE id = @Id";
            SqlParameter parameter = new SqlParameter("@Id", id);

            return _db.ExecuteNonQuery(query, parameter) > 0;
        }

        private Contact MapRowToContact(DataRow row)
        {
            var contact = new Contact
            {
                Id = Convert.ToInt32(row["id"]),
                NguoiGui = row["nguoiGui"].ToString(),
                Email = row["email"].ToString(),
                TieuDe = row["tieuDe"].ToString(),
                NoiDung = row["noiDung"].ToString()
            };

            return contact;
        }
    }
}