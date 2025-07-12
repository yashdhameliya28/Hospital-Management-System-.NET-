using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DoctorController : Controller
    {

        public IConfiguration configuration;

        public DoctorController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        #region List From Db
        public DataTable DList(String SP)
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = SP;
                    //;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
        }
        #endregion List From Db

        #region Delete DB
        public IActionResult DocterDelete(int DOCTORID)
        {

            try
            {

                string connectionString = this.configuration.GetConnectionString("myConnection");

                using (SqlConnection connectioin = new SqlConnection(connectionString))
                {
                    connectioin.Open();

                    using (SqlCommand command = connectioin.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Doctor_DeleteByPK";

                        command.Parameters.Add("DOCTORID", SqlDbType.Int).Value = DOCTORID;
                        TempData["Sucessfully"] = "Deleted Sucessfully.";

                        command.ExecuteNonQuery();

                        return RedirectToAction("DoctorList");

                    }
                }
            }
            catch(Exception e)
            {
                TempData["Unsucessfully"] = "Not Delete.";
                return RedirectToAction("DoctorList");
            }
        }
        #endregion

        #region Add/Edit DB 
        public IActionResult DcotorAddEdit(DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                doctorModel.IsActive = true;
                doctorModel.Created = DateTime.Now;
                doctorModel.Modified = DateTime.Now;

                string connectionString = this.configuration.GetConnectionString("myConnection");

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                doctorModel.UserID = 2;

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doctor_Insert";

                command.Parameters.AddWithValue("@NAME", SqlDbType.VarChar).Value = doctorModel.Name;
                command.Parameters.AddWithValue("@PHONE", SqlDbType.VarChar).Value = doctorModel.Phone;
                command.Parameters.AddWithValue("@EMAIL", SqlDbType.VarChar).Value = doctorModel.Email;
                command.Parameters.AddWithValue("@QUALIFICATION", SqlDbType.VarChar).Value = doctorModel.Qualification;
                command.Parameters.AddWithValue("@SPECIALIZATION", SqlDbType.VarChar).Value = doctorModel.Specialization;
                command.Parameters.AddWithValue("@ISACTIVE", SqlDbType.Bit).Value = doctorModel.IsActive;
                command.Parameters.AddWithValue("@CREATED", SqlDbType.DateTime).Value = doctorModel.Created;
                command.Parameters.AddWithValue("@MODIFIED", SqlDbType.DateTime).Value = doctorModel.Modified;
                command.Parameters.AddWithValue("@USERID", SqlDbType.Int).Value = doctorModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("DoctorList");

            }

            return View(doctorModel);
        }
        #endregion

        public IActionResult DoctorList()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            DataTable table = DList("PR_Doctor_SelectAll");
            

            return View(table);
        }



        public IActionResult DoctorAddEdit()
        {
            return View();
        }
    }
}
