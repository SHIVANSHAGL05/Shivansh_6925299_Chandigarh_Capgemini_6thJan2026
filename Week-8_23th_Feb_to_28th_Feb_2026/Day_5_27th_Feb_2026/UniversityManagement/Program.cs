using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace UniversityManagement
{
    internal class Program
    {
        static string conStr =
        "Server=Blackpearl\\SQLEXPRESS;" +
        "Database=UniversityDB;" +
        "Integrated Security=True;" +
        "TrustServerCertificate=True;";

        static void Main()
        {
            int choice;

            do
            {
                Console.WriteLine("\n--- STUDENT MANAGEMENT SYSTEM ---");
                Console.WriteLine("1. Insert Student");
                Console.WriteLine("2. Update Student Email");
                Console.WriteLine("3. Delete Student");
                Console.WriteLine("4. View Students");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                choice = Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        InsertStudent();
                        break;

                    case 2:
                        UpdateStudent();
                        break;

                    case 3:
                        DeleteStudent();
                        break;

                    case 4:
                        GetStudents();
                        break;

                    case 0:
                        Console.WriteLine("Exiting application...");
                        break;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }

            } while (choice != 0);
        }

        static void InsertStudent()
        {
            Console.Write("Enter First Name: ");
            string fname = Console.ReadLine();

            Console.Write("Enter Last Name: ");
            string lname = Console.ReadLine();

            Console.Write("Enter Email: ");
            string email = Console.ReadLine();

            Console.Write("Enter Dept Id: ");
            int deptId = Convert.ToInt32(Console.ReadLine());

            using SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("sp_InsertStudent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FirstName", fname);
            cmd.Parameters.AddWithValue("@LastName", lname);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@DeptId", deptId);

            con.Open();
            cmd.ExecuteNonQuery();

            Console.WriteLine(" Student inserted successfully");
        }

        static void UpdateStudent()
        {
            Console.Write("Enter Student ID to update: ");
            int id = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter new Email: ");
            string email = Console.ReadLine();

            using SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("sp_UpdateStudent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StudentId", id);
            cmd.Parameters.AddWithValue("@Email", email);

            con.Open();
            cmd.ExecuteNonQuery();

            Console.WriteLine("Student updated successfully");
        }

        static void DeleteStudent()
        {
            Console.Write("Enter Student ID to delete: ");
            int id = Convert.ToInt32(Console.ReadLine());

            using SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("sp_DeleteStudent", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@StudentId", id);

            con.Open();
            cmd.ExecuteNonQuery();

            Console.WriteLine(" Student deleted successfully");
        }

        static void GetStudents()
        {
            using SqlConnection con = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand("sp_GetStudents", con);
            cmd.CommandType = CommandType.StoredProcedure;

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            Console.WriteLine("\nID\tName\tEmail");
            Console.WriteLine("------------------------------");

            while (dr.Read())
            {
                Console.WriteLine($"{dr["StudentId"]}\t{dr["FirstName"]}\t{dr["Email"]}");
            }
        }
    }
}