using APBD_3.DTOs.Requests;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace APBD_3.Services
{
    public class SqlServerStudentDbService: IStudentServiceDb
    {
        String CONNECTION_STRING = @"Data Source=LAPTOP-11FAC326\SQLEXPRESS;Initial Catalog=s19047;Integrated Security=True";

        public void EnrollStudent(EnrollStudentRequest request)
        {
            using (var client = new SqlConnection(CONNECTION_STRING))
            using (var command = new SqlCommand())
            {
                command.CommandText = "SELECT IdStudy FROM Studies where Name = '@Name';";
                command.Parameters.AddWithValue("Name", request.Studies);
                command.Connection = client;

                client.Open();
                var tran = client.BeginTransaction();
                try
                {
                    var reader = command.ExecuteReader();

                    //check if studies exist else rollback + 404
                    if (!reader.Read())
                    {
                        tran.Rollback();
                        //studies does not exist
                    }
                    int IdStudy = (int)reader["IdStudy"];

                    // Note that the assignment asks to search for values where semester = 1; However, our tables have 
                    // years as semesters so for example 2020 , for that reason I just assumed 2020 as the default semester
                    //instead of 1
                    int enrollId = 0;
                    command.CommandText = "SELECT * FROM Enrollment  where Semester = 2020 AND IdStudy='@IdStudy'";
                    command.Parameters.AddWithValue("IdStudy", IdStudy);

                    reader = command.ExecuteReader();

                    //check if enrollment already exists , else insert one 

                    if (!reader.Read())
                    {
                        command.CommandText = "Select Top 1 IdEnrollment from Enrollment Order By  IdEnrollment DESC";
                        reader = command.ExecuteReader();
                        enrollId = int.Parse(reader["IdEnrollment"].ToString());

                        command.CommandText = "insert into Enrollment(IdEnrollment,IdStudy, Semester, StartDate) values (@enrollId, @IdStudy, @Semester, '@Date')";
                        command.Parameters.AddWithValue("enrollId", ++enrollId);
                        command.Parameters.AddWithValue("IdStudy", IdStudy);
                        command.Parameters.AddWithValue("Semester", "2020");
                        command.Parameters.AddWithValue("Date", DateTime.Now);

                    }
                    else
                    {
                        enrollId = int.Parse(reader["IdEnrollment"].ToString());
                    }


                    //check if index number was assigned to any other student , if not insert student 
                    command.CommandText = "Select * from Student where IndexNumber = '@index'";
                    command.Parameters.AddWithValue("index", request.IndexNumber);
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Student Already Exists!");
                    }

                    command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@FirstName,@LastName,@Birthdate,@enrollId";
                    command.Parameters.AddWithValue("FirstName", request.FirstName);
                    command.Parameters.AddWithValue("LastName", request.LastName);
                    command.Parameters.AddWithValue("BirthDate", request.Birthdate);
                    command.Parameters.AddWithValue("enrollId", enrollId);
                    command.ExecuteNonQuery();


                    tran.Commit();
                    client.Close();

                }
                catch (SqlException e)
                {
                    tran.Rollback();

                }

            }

        }

        public void PromoteStudents(String Studies, int Semester)
        {
             //check if Enrollment exists
            using (var client = new SqlConnection(CONNECTION_STRING))
            using (var command = new SqlCommand())
            {

                command.CommandText = "EXEC PromoteStudents @StudiesName = '@studies' , @Semester = '@sem';";
                command.Parameters.AddWithValue("sem", Semester);
                command.Parameters.AddWithValue("studies", Studies);
                var reader = command.ExecuteReader();
            }
        }
    }
}
