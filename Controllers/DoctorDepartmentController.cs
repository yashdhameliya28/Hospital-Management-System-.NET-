using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DoctorDepartmentController : Controller
    {

        public IConfiguration configuration;

        public DoctorDepartmentController (IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #region List From DB
        public DataTable DDlist(string SP)
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = SP;

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable(); 
                        table.Load(reader);
                        return table;
                    }
                }
            }
        }
        #endregion

        #region Delete
        public IActionResult DoctorDepartmentDelete(int DOCTORDEPARTMENTID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("myConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_DoctorDepartment_DeleteByPK";

                        command.Parameters.Add("@DOCTORDEPARTMENTID", SqlDbType.Int).Value = DOCTORDEPARTMENTID;
                        TempData["Sucessfully"] = "Deleted Sucessfully.";
                        command.ExecuteNonQuery();

                        return RedirectToAction("DoctorDepartmentList");
                    }
                }
            }
            catch(Exception e)
            {
                TempData["Unsucessfully"] = "Not Delete.";
                return RedirectToAction("DoctorDepartmentList");
            }
        }
        #endregion

        #region ADD/EDIT
        public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModel doctorDepartmentModel)
        {
            if (ModelState.IsValid)
            {
                doctorDepartmentModel.Created = DateTime.Now;
                doctorDepartmentModel.Modified = DateTime.Now;
                doctorDepartmentModel.UserID = 2;

                string connectionString = this.configuration.GetConnectionString("myConnection");
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();

                SqlCommand command = connection.CreateCommand();

                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DoctorDepartment_Insert";

                command.Parameters.AddWithValue("@DOCTORID", SqlDbType.Int).Value = doctorDepartmentModel.DoctorID;
                command.Parameters.AddWithValue("@DEPARTMENTID", SqlDbType.Int).Value = doctorDepartmentModel.DepartmentID;
                command.Parameters.AddWithValue("@CREATED", SqlDbType.Int).Value = doctorDepartmentModel.Created;
                command.Parameters.AddWithValue("@MODIFIED", SqlDbType.Int).Value = doctorDepartmentModel.Modified;
                command.Parameters.AddWithValue("@USERID", SqlDbType.Int).Value = doctorDepartmentModel.UserID;

                command.ExecuteNonQuery();
                return RedirectToAction("DoctorDepartmentList");
            }

            return View(doctorDepartmentModel);
        }
        #endregion

        public IActionResult DoctorDepartmentList()
        {
            DataTable table = DDlist("PR_DoctorDepartment_SelectAll");

            return View(table);
        }

        public IActionResult DoctorDepartmentAddEdit()
        {
            return View();
        }
    }
}
