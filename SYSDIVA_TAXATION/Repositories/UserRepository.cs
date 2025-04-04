using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SYSDIVA_TAXATION.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SYSDIVA_TAXATION.Repositories
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        //Get all user data //
        public List<Users> GetUsers()
        {
            List<Users> users = new List<Users>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Email, Age, Salary, Gender, CreatedOn, IsActive FROM Users";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new Users
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Age = reader.GetInt32(reader.GetOrdinal("Age")),
                        Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                        Gender = reader.GetString(reader.GetOrdinal("Gender")),
                        CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }

                return users;
            }
        }
        //Create the new user //
        public bool InsertUser(Users user)
        {
            // Ensure CreatedOn is within SQL Server's valid range
            if (user.CreatedOn == null || user.CreatedOn < new DateTime(1753, 1, 1))
            {
                user.CreatedOn = DateTime.UtcNow; // Default to current UTC date if invalid
            }

            bool isInserted = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO Users (Name, Email, Age, Salary, Gender, CreatedOn, IsActive) " +
                               "VALUES (@Name, @Email, @Age, @Salary, @Gender, @CreatedOn, @IsActive)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Ensure null safety and data integrity
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = (object)user.Name ?? DBNull.Value;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = (object)user.Email ?? DBNull.Value;
                    cmd.Parameters.Add("@Age", SqlDbType.Int).Value = user.Age > 0 ? (object)user.Age : DBNull.Value;
                    cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = user.Salary > 0 ? (object)user.Salary : DBNull.Value;
                    cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = (object)user.Gender ?? DBNull.Value;
                    cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = (object)user.CreatedOn ?? DBNull.Value;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = user.IsActive;

                    try
                    {
                        conn.Open();
                        isInserted = cmd.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex)
                    {
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        // Log error properly using a logging framework like Serilog or NLog
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"General Error: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            return isInserted;
        }


        // **Delete Employee**
        public bool DeleteUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM Employees WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        // **Update Employee**
        public bool UpdateUser(Users users)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Users SET Name = @Name, Email = @Email, Age = @Age, Salary = @Salary, Gender = @Gender, IsActive = @IsActive WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", users.Id);
                cmd.Parameters.AddWithValue("@Name", users.Name);
                cmd.Parameters.AddWithValue("@Email", users.Email);
                cmd.Parameters.AddWithValue("@Age", users.Age);
                cmd.Parameters.AddWithValue("@Salary", users.Salary);
                cmd.Parameters.AddWithValue("@Gender", users.Gender);
                cmd.Parameters.AddWithValue("@IsActive", users.IsActive);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public Users GetUserById(int id)
        {
            Users user = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT Id, Name, Email, Age, Salary, Gender, CreatedOn, IsActive FROM Users WHERE Id = @Id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    user = new Users
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Email = reader["Email"].ToString(),
                        Age = Convert.ToInt32(reader["Age"]),
                        Salary = Convert.ToDecimal(reader["Salary"]),
                        Gender = reader["Gender"].ToString(),
                        CreatedOn = Convert.ToDateTime(reader["CreatedOn"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    };
                }
                reader.Close();
            }
            return user;
        }
        //------------------------service code for studet------------
        public List<Student> GetAllStudent()
        {

            List<Student> students = new List<Student>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                string Query = "SELECT Id, Name, Age, Class, Email, IsActive FROM Students;";
                SqlCommand cmd = new SqlCommand(Query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Age = reader.GetInt32(2),
                        Email = reader.GetString(3),
                        Class = reader.GetString(4),
                        IsActive = (bool)reader.GetBoolean(5)
                    });

                }
            }
            return students;
        }
        public bool AddStudent(Student student)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // ✅ Fixed SQL Query (Added missing comma)
                string query = "INSERT INTO Students (Name, Age, Class, Email, IsActive) " +
                               "VALUES (@Name, @Age, @Class, @Email, @IsActive)";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Name", student.Name);
                cmd.Parameters.AddWithValue("@Age", student.Age);
                cmd.Parameters.AddWithValue("@Class", student.Class);
                cmd.Parameters.AddWithValue("@Email", student.Email);
                cmd.Parameters.AddWithValue("@IsActive", student.IsActive ? 1 : 0); // ✅ Ensure BIT type

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


    }
}


