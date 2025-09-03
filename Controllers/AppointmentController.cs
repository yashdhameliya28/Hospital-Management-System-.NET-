using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class AppointmentController : Controller
    {
        public IConfiguration Configuration;

        public AppointmentController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region List from DB

        public DataTable ApList(string SP)
        {
            string connectionString = this.Configuration.GetConnectionString("myConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
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
            catch (Exception e)
            {
                TempData["Unsucessfully"] = "Not Delete.";
                return RedirectToAction("AppointmentList");
            }

        }

        #endregion


        #region ADD/EDIT DB
        [HttpPost]
        public IActionResult AppintmentSave(AppointmentModel appointmentModel)
        {
            if (ModelState.IsValid)
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

                            if (appointmentModel.AppointmentID != 0)
                            {
                                command.CommandText = "PR_Appointment_UpdateByPK";
                                command.Parameters.AddWithValue("@APPOINMENTID", appointmentModel.AppointmentID);
                                appointmentModel.Modified = DateTime.Now;
                            }
                            else
                            {
                                command.CommandText = "PR_Appointment_Insert";
                                appointmentModel.Created = DateTime.Now;
                                appointmentModel.Modified = DateTime.Now;
                            }

                            command.Parameters.AddWithValue("@DOCTORID", appointmentModel.DoctorID);
                            command.Parameters.AddWithValue("@PATIENTID", appointmentModel.PatientID);
                            command.Parameters.AddWithValue("@APPOINMENTDATE", appointmentModel.AppointmentDate);
                            command.Parameters.AddWithValue("@APPOINMENTSTATUS", appointmentModel.AppointmentStatus);
                            command.Parameters.AddWithValue("@DESCRIPTION", appointmentModel.Description);
                            command.Parameters.AddWithValue("@SPECIALREMARKS", appointmentModel.SpecialRemarks);
                            command.Parameters.AddWithValue("@MODIFIED", appointmentModel.Modified);
                            command.Parameters.AddWithValue("@USERID", appointmentModel.UserID);
                            command.Parameters.AddWithValue("@TOTALCONSULTEDAMOUNT", appointmentModel.TotalConsultedAmount);

                            int rows = command.ExecuteNonQuery();

                            TempData["AppointmentInsertUpdateMessage"] = rows > 0
                                ? (appointmentModel.AppointmentID == 0 ? "Appointment added successfully!" : "Appointment updated successfully!")
                                : "Appointment Add/update Failed";
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["AppointmentInsertUpdateMessage"] = "Error: " + e.Message;
                }
            }
            else
            {
                return View("AppointmentAddEdit", appointmentModel);
            }

            return RedirectToAction("AppointmentList");
        }
        #endregion

        #region GET method for Edit - This was missing and causing 404
        public IActionResult AppointmentSave(int? AppointmentID)
        {
            if(AppointmentID == null)
            {
                ViewBag.ErrorMessage = "Appointment ID is required";
                return RedirectToAction("AppointmentList");
            }

            string connectionString = this.Configuration.GetConnectionString("myConnection");
            AppointmentModel appointmentModel = new AppointmentModel();

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Appointment_SelectByPK";

                        command.Parameters.AddWithValue("@APPOINTMENTID", AppointmentID);

                        using(SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                appointmentModel.AppointmentID = Convert.ToInt32(reader["AppointmentID"]);
                                appointmentModel.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                                appointmentModel.PatientID = Convert.ToInt32(reader["PatientID"]);
                                appointmentModel.AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);
                                appointmentModel.AppointmentStatus = reader["AppointmentStatus"].ToString();
                                appointmentModel.Description = reader["Description"].ToString();
                                appointmentModel.SpecialRemarks = reader["SpecialRemarks"].ToString();
                                appointmentModel.Created = Convert.ToDateTime(reader["Created"]);
                                appointmentModel.Modified = Convert.ToDateTime(reader["Modified"]);
                                appointmentModel.UserID = Convert.ToInt32(reader["UserID"]);
                                appointmentModel.TotalConsultedAmount = Convert.ToInt32(reader["TotalConsultedAmount"]);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Appointment not found for edit";
                                return RedirectToAction("AppointmentList");
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TempData["ErrorMessage"] = "Error loading doctor: " + e.Message;
                return RedirectToAction("AppointmentList");
            }

            return View("AppointmentAddEdit", appointmentModel);
        }
        #endregion


        public IActionResult AppointmentList()
        {
            DataTable table = ApList("PR_Appointment_SelectAll");
            return View(table);
        }

        [HttpGet]
        public IActionResult AppointmentAddEdit()
        {
            return View();
        }
    }
}
