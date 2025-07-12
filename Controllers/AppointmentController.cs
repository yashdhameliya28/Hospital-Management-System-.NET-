using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class AppointmentController : Controller
    {
        public IConfiguration Configuration;

        public AppointmentController (IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region List from DB

        public DataTable ApList(string SP)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnection");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection .Open();

                using(SqlCommand command = connection.CreateCommand())
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


        #region DELETE
        public IActionResult AppointmentDelete(int APPOINMENTID)
        {
            try
            {
                string connectionString = this.Configuration.GetConnectionString("myConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Appointment_DeleteByPK";

                        command.Parameters.Add("@APPOINMENTID", SqlDbType.Int).Value = APPOINMENTID;
                        TempData["Sucessfully"] = "Deleted Sucessfully.";
                        command.ExecuteNonQuery();


                        return RedirectToAction("AppointmentList");
                    }
                }
            }
            catch(Exception e)
            {
                TempData["Unsucessfully"] = "Not Delete.";
                return RedirectToAction("AppointmentList");
            }
            
        }

        #endregion


        #region ADD/EDIT DB
        public IActionResult AppointmentAddEdit(AppointmentModel appointmentModel)
        {
            if (ModelState.IsValid)
            {
                appointmentModel.Created = DateTime.Now;
                appointmentModel.Modified = DateTime.Now;
                appointmentModel.UserID = 2;

                string connectionString = this.Configuration.GetConnectionString("myConnection");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Appointment_Insert";

                //command.Parameters.AddWithValue("@DOCTORID", SqlDbType.Int).Value = appointmentModel.DoctorID;
                command.Parameters.AddWithValue("@APPOINMENTDATE", SqlDbType.Int).Value = appointmentModel.AppointmentDate;
                //command.Parameters.AddWithValue("", SqlDbType.Int).Value = appointmentModel.DoctorID;
                command.Parameters.AddWithValue("@DESCRIPTION", SqlDbType.Int).Value = appointmentModel.Description;
                command.Parameters.AddWithValue("@SPECIALREMARKS", SqlDbType.Int).Value = appointmentModel.SpecialRemarks;
                command.Parameters.AddWithValue("@CREATED", SqlDbType.Int).Value = appointmentModel.Created;
                command.Parameters.AddWithValue("@MODIFIED", SqlDbType.Int).Value = appointmentModel.Modified;
                command.Parameters.AddWithValue("@USERID", SqlDbType.Int).Value = appointmentModel.UserID;

                command.ExecuteNonQuery();

                return RedirectToAction("AppointmentList");

            }
            return View(appointmentModel);
        }
        #endregion

        public IActionResult AppointmentList()
        {
            DataTable table = ApList("PR_Appointment_SelectAll");
            return View(table);
        }

        public IActionResult AppointmentAddEdit()
        {
            return View();
        }
    }
}
