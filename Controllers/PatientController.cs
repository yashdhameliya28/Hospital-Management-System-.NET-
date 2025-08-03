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

        public IActionResult PatientDelete(int PATIENTID)
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
                        command.CommandText = "PR_Patient_DeleteByPK";

                        command.Parameters.Add("PATIENTID", SqlDbType.Int).Value = PATIENTID;
                        TempData["Successfully"] = "Deleted Successfully.";
                        command.ExecuteNonQuery();

                        return RedirectToAction("PatientList");
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Unsuccessfully"] = "Delete Failed: " + e.Message;
                return RedirectToAction("Plist");
            }
        }

        #endregion

        
        #region Add/Edit DB 
        [HttpPost]
        public IActionResult PatientSave(PatientModel patientModel)
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

                            if (patientModel.PatientID != 0)
                            {
                                command.CommandText = "PR_Patient_UpdateByPK";
                                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = patientModel.PatientID;
                                patientModel.Modified = DateTime.Now;
                            }
                            else
                            {
                                command.CommandText = "PR_Patient_Insert";
                                patientModel.Created = DateTime.Now;
                                patientModel.UserID = 6;
                                patientModel.IsActive = true;
                                patientModel.Modified = DateTime.Now;
                            }

                            command.Parameters.Add("@PATIENTNAME", SqlDbType.VarChar).Value = patientModel.PatientName;
                            command.Parameters.Add("@DATEOFBIRTH", SqlDbType.DateTime).Value = patientModel.DateOfBirth;
                            command.Parameters.Add("@GENDER", SqlDbType.VarChar).Value = patientModel.Gender;
                            command.Parameters.Add("@EMAIL", SqlDbType.VarChar).Value = patientModel.Email;
                            command.Parameters.Add("@PHONE", SqlDbType.VarChar).Value = patientModel.Phone;
                            command.Parameters.Add("@ADDRESS", SqlDbType.VarChar).Value = patientModel.Address;
                            command.Parameters.Add("@CITY", SqlDbType.VarChar).Value = patientModel.City;
                            command.Parameters.Add("@STATE", SqlDbType.VarChar).Value = patientModel.State;
                            command.Parameters.Add("@ISACTIVE", SqlDbType.Bit).Value = patientModel.IsActive;
                            command.Parameters.Add("@MODIFIED", SqlDbType.DateTime).Value = patientModel.Modified;
                            command.Parameters.Add("@USERID", SqlDbType.Int).Value = patientModel.UserID;

                            int rows = command.ExecuteNonQuery();

                            TempData["PatientInsertUpdateMessage"] = rows > 0
                                ? (patientModel.PatientID == 0 ? "Patient added successfully!" : "Patient updated successfully!")
                                : "Patient Add/update Failed";
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
                return View("PatientAddEdit", patientModel);
            }

            return RedirectToAction("PatientList");
        }
        #endregion


        #region GET method for Edit - This was missing and causing 404
        public IActionResult PatientUpdate(int? PatientID)
        {
            if (PatientID == null)
            {
                ViewBag.ErrorMessage = "PatientID ID is required";
                return RedirectToAction("PatientList");
            }

            string connectionString = this.configuration.GetConnectionString("myConnection");
            PatientModel patientModel = new PatientModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Patient_SelectByPK";

                        command.Parameters.Add("@PATIENTID", SqlDbType.Int).Value = PatientID;

                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            patientModel.PatientID = Convert.ToInt32(reader["PatientID"]);
                            patientModel.PatientName = reader["PatientName"].ToString();
                            patientModel.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                            patientModel.Gender = reader["Gender"].ToString();
                            patientModel.Email = reader["Email"].ToString();
                            patientModel.Phone = reader["Phone"].ToString();
                            patientModel.City = reader["City"].ToString();
                            patientModel.Address = reader["Address"].ToString();
                            patientModel.State = reader["State"].ToString();
                            patientModel.Modified = Convert.ToDateTime(reader["Modified"]);
                            patientModel.Created = Convert.ToDateTime(reader["Created"]);
                            patientModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                            patientModel.UserID = Convert.ToInt32(reader["UserID"]);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Patient not found for edit";
                            return RedirectToAction("PatientList");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading patient: " + ex.Message;
                return RedirectToAction("PatientList");
            }

            return View("PatientAddEdit", patientModel);
        }
        #endregion


        public IActionResult PatientList()
        {
            DataTable table = Plist("PR_Patient_SelectAll");

            return View(table);
        }

        public IActionResult PatientAddEdit()
        {
            return View(new PatientModel());
        }


    }
}
