using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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

        #region Delete DB
        public IActionResult DoctorDelete(int DOCTORID)
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
                        command.CommandText = "PR_Doctor_DeleteByPK";

                        command.Parameters.Add("DOCTORID", SqlDbType.Int).Value = DOCTORID;
                        command.ExecuteNonQuery();

                        TempData["Successfully"] = "Deleted Successfully.";
                        return RedirectToAction("DoctorList");
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Unsuccessfully"] = "Delete Failed: " + e.Message;
                return RedirectToAction("DoctorList");
            }
        }
        #endregion

        #region Add/Edit DB 
        [HttpPost]
        public IActionResult DoctorSave(DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
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

                            if (doctorModel.DoctorID != 0)
                            {
                                command.CommandText = "PR_Doctor_UpdateByPK";
                                command.Parameters.AddWithValue("@DOCTORID", doctorModel.DoctorID);
                                doctorModel.Modified = DateTime.Now;
                            }
                            else
                            {
                                command.CommandText = "PR_Doctor_Insert";
                                doctorModel.UserID = 6;
                                doctorModel.Created = DateTime.Now;
                                doctorModel.Modified = DateTime.Now;
                            }

                            command.Parameters.Add("@Name", SqlDbType.VarChar).Value = doctorModel.Name ?? "";
                            command.Parameters.Add("@Phone", SqlDbType.VarChar).Value = doctorModel.Phone ?? "";
                            command.Parameters.Add("@Email", SqlDbType.VarChar).Value = doctorModel.Email ?? "";
                            command.Parameters.Add("@Qualification", SqlDbType.VarChar).Value = doctorModel.Qualification ?? "";
                            command.Parameters.Add("@Specialization", SqlDbType.VarChar).Value = doctorModel.Specialization ?? "";
                            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = true;
                            command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = doctorModel.Modified;
                            command.Parameters.Add("@UserID", SqlDbType.Int).Value = doctorModel.UserID;

                            int rows = command.ExecuteNonQuery();

                            TempData["DoctorInsertUpdateMessage"] = rows > 0
                                ? (doctorModel.DoctorID == 0 ? "Doctor added successfully!" : "Doctor updated successfully!")
                                : "Doctor Add/update Failed";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["DoctorInsertUpdateMessage"] = "Error: " + ex.Message;
                }
            }
            else
            {
                return View("DoctorAddEdit", doctorModel);
            }

            return RedirectToAction("DoctorList");
        }
        #endregion

        #region GET method for Edit - This was missing and causing 404
        [HttpGet]
        public IActionResult DoctorUpdate(int? DoctorID)
        {
            if (DoctorID == null)
            {
                ViewBag.ErrorMessage = "Doctor ID is required";
                return RedirectToAction("DoctorList");
            }

            string connectionString = this.configuration.GetConnectionString("myConnection");
            DoctorModel doctorModel = new DoctorModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Doctor_SelectByPK";
                        command.Parameters.Add("@DOCTORID", SqlDbType.Int).Value = DoctorID;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                doctorModel.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                                doctorModel.Name = reader["Name"].ToString() ?? "";
                                doctorModel.Phone = reader["Phone"].ToString() ?? "";
                                doctorModel.Email = reader["Email"].ToString() ?? "";
                                doctorModel.Qualification = reader["Qualification"].ToString() ?? "";
                                doctorModel.Specialization = reader["Specialization"].ToString() ?? "";


                                doctorModel.Modified = Convert.ToDateTime(reader["Modified"]);


                                doctorModel.Created = Convert.ToDateTime(reader["Created"]);

                                doctorModel.UserID = Convert.ToInt32(reader["UserID"]);

                                doctorModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "Doctor not found for edit";
                                return RedirectToAction("DoctorList");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading doctor: " + ex.Message;
                return RedirectToAction("DoctorList");
            }

            return View("DoctorAddEdit", doctorModel);
        }
        #endregion

        public IActionResult DoctorList()
        {
            DataTable table = DList("PR_Doctor_SelectAll");
            return View(table);
        }

        [HttpGet]
        public IActionResult DoctorAddEdit()
        {
            return View(new DoctorModel());
        }
    }
}