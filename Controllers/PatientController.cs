using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {

        public IConfiguration configuration;
        
        public PatientController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region List from DB
         public DataTable Plist(string SP)
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");
            
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = SP;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            } 
        }
        #endregion

        #region DELETE

        public IActionResult PatientDelete(int PATIENTID)
        {
            try
            {
            string connectionString = this.configuration.GetConnectionString("myConnection");

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Patient_DeleteByPK";

                        command.Parameters.Add("PATIENTID", SqlDbType.Int).Value = PATIENTID;
                        TempData["Sucessfully"] = "Deleted Sucessfully.";
                        command.ExecuteNonQuery();

                        return RedirectToAction("Plist");
                    }
                }
            }
            catch(Exception e)
            {
                TempData["Unsucessfully"] = "Not Delete.";
                return RedirectToAction("Plist");
            }

         
        }

        #endregion

        #region ADD/EDIT
        public IActionResult PatientAddEdit(PatientModel patientModel)
        {
            if (ModelState.IsValid)
            {
                patientModel.IsActive = true;
                patientModel.Created = DateTime.Now;
                patientModel.Modified = DateTime.Now;

                string connectionString = this.configuration.GetConnectionString("myConnection");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                patientModel.UserID = 2;

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Patient_Insert";

                command.Parameters.AddWithValue("@NAME", SqlDbType.VarChar).Value = patientModel.Name;
                command.Parameters.AddWithValue("@DATEOFBIRTH", SqlDbType.DateTime).Value = patientModel.DateOfBirth;
                command.Parameters.AddWithValue("@GENDER", SqlDbType.VarChar).Value = patientModel.Gender;
                command.Parameters.AddWithValue("@EMAIL", SqlDbType.VarChar).Value = patientModel.Email;
                command.Parameters.AddWithValue("@PHONE", SqlDbType.VarChar).Value = patientModel.Phone;
                command.Parameters.AddWithValue("@ADDRESS", SqlDbType.VarChar).Value = patientModel.Address;
                command.Parameters.AddWithValue("@CITY", SqlDbType.VarChar).Value = patientModel.City;
                command.Parameters.AddWithValue("@STATE", SqlDbType.VarChar).Value = patientModel.State;
                command.Parameters.AddWithValue("@ISACTIVE", SqlDbType.Bit).Value = patientModel.IsActive;
                command.Parameters.AddWithValue("@CREATED", SqlDbType.DateTime).Value = patientModel.Created;
                command.Parameters.AddWithValue("@MODIFIED", SqlDbType.DateTime).Value = patientModel.Modified;
                command.Parameters.AddWithValue("@USERID", SqlDbType.Int).Value = patientModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("PatientList");

            }
            return View(patientModel);
        }
        #endregion

        public IActionResult PatientList()
        {
            //string connectionString = configuration.GetConnectionString("ConnectionString");
            DataTable table = Plist("PR_Patient_SelectAll");
            
            return View(table);
        }

        public IActionResult PatientAddEdit()
        {
            return View();
        }
    }
}
