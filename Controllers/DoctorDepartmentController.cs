using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DoctorDepartmentController : Controller
    {

        public IConfiguration configuration;

        public DoctorDepartmentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #region List From DB
        public DataTable DDlist(string SP)
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
                        TempData["Successfully"] = "Deleted Successfully.";
                        command.ExecuteNonQuery();

                        return RedirectToAction("DoctorDepartmentList");
                    }
                }
            }
            catch (Exception e)
            {
                TempData["Unsuccessfully"] = "Delete Failed: " + e.Message;
                return RedirectToAction("DoctorDepartmentList");
            }
        }
        #endregion

        #region Add/Edit DB
        [HttpPost]
        public IActionResult DoctorDepartmentSave(DoctorDepartmentModel doctorDepartmentModel)
        {
            ModelState.Remove("DoctorName");
            ModelState.Remove("DepartmentName");
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
                            if (doctorDepartmentModel.DoctorDepartmentID != 0)
                            {
                                command.CommandText = "PR_DoctorDepartment_UpdateByPK";
                                command.Parameters.AddWithValue("@DOCTORDEPARTMENTID", doctorDepartmentModel.DoctorDepartmentID);
                                doctorDepartmentModel.Modified = DateTime.Now;
                            }
                            else
                            {
                                command.CommandText = "PR_DoctorDepartment_Insert";
                                doctorDepartmentModel.Created = DateTime.Now;
                                doctorDepartmentModel.Modified = DateTime.Now;
                            }

                            UserDropDown();
                            DoctorDropDown();
                            DepartmentDropDown();

                            command.Parameters.AddWithValue("@DOCTORID", doctorDepartmentModel.DoctorID);
                            command.Parameters.AddWithValue("@DEPARTMENTID", doctorDepartmentModel.DepartmentID);
                            command.Parameters.AddWithValue("@MODIFIED", doctorDepartmentModel.Modified);
                            command.Parameters.AddWithValue("@USERID", doctorDepartmentModel.UserID);

                            int rows = command.ExecuteNonQuery();

                            TempData["DoctorInsertUpdateMessage"] = rows > 0
                                ? (doctorDepartmentModel.DoctorDepartmentID == 0 ? "Doctor Department added successfully!" : "Doctor Department updated successfully!")
                                : "Doctor Department Add/update Failed";
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["DoctorDepartmentInsertUpdateMessage"] = "Error: " + ex.Message;
                }
            }
            else
            {
                return View("DoctorDepartmentAddEdit", doctorDepartmentModel);
            }

            return RedirectToAction("DoctorDepartmentList");
        }
        #endregion

        #region GET method for Edit - This was missing and causing 404
        public IActionResult DoctorDepartmentUpdate(int? DoctorDepartmentID)
        {
            UserDropDown();
            DoctorDropDown();
            DepartmentDropDown();
            if (DoctorDepartmentID == null)
            {
                ViewBag.ErrorMessage = "DoctorDepartmentID ID is required";
                return View("DoctorDepartmentList");
            }

            string connectionString = this.configuration.GetConnectionString("myConnection");
            DoctorDepartmentModel doctorDepartmentModel = new DoctorDepartmentModel();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_DoctorDepartment_SelectByPK";

                        command.Parameters.AddWithValue("@DOCTORDEPARTMENTID", DoctorDepartmentID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                doctorDepartmentModel.DoctorDepartmentID = Convert.ToInt32(reader["DoctorDepartmentID"]);
                                doctorDepartmentModel.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                                doctorDepartmentModel.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                doctorDepartmentModel.Created = Convert.ToDateTime(reader["Created"]);
                                doctorDepartmentModel.Modified = Convert.ToDateTime(reader["Modified"]);
                                doctorDepartmentModel.UserID = Convert.ToInt32(reader["UserID"]);
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "DoctorDepartment not found for edit";
                                return RedirectToAction("DoctorDepartmentList");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading doctor: " + ex.Message;
                return RedirectToAction("DoctorDepartmentList");
            }

            return View("DoctorDepartmentAddEdit", doctorDepartmentModel);
        }
        #endregion

        #region User Drop-Down 
        public void UserDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_SelectForDropDown";


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);

                        List<User_DropDownModel> userList = new List<User_DropDownModel>();
                        foreach (DataRow data in table.Rows)
                        {
                            User_DropDownModel model = new User_DropDownModel();
                            model.UserID = Convert.ToInt32(data["UserID"]);
                            model.UserName = data["UserName"].ToString();

                            userList.Add(model);
                        }

                        ViewBag.UserList = userList;
                    }
                }
            }
        }
        #endregion

        #region Doctor Drop-Down
        public void DoctorDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Doctor_SelectForDropDown";


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);

                        List<Doctor_DropDownModel> doctorList = new List<Doctor_DropDownModel>();
                        foreach (DataRow data in table.Rows)
                        {
                            Doctor_DropDownModel model = new Doctor_DropDownModel();
                            
                            model.DoctorID = Convert.ToInt32(data["DoctorID"]);
                            model.DoctorName = data["Name"].ToString();

                            doctorList.Add(model);
                        }

                        ViewBag.DoctorList = doctorList;
                    }
                }
            }
        }
        #endregion

        #region Department Drop-Down
        public void DepartmentDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Department_SelectForDropDown";


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);

                        List<Department_DropDown> departmentList = new List<Department_DropDown>();
                        foreach (DataRow data in table.Rows)
                        {
                            Department_DropDown model = new Department_DropDown();
                            model.DepartmentID = Convert.ToInt32(data["DepartmentID"]);
                            model.DepartmentName = data["DepartmentName"].ToString();

                            departmentList.Add(model);
                        }

                        ViewBag.DepartmentList = departmentList;
                    }
                }
            }
        }
        #endregion

        [HttpGet]
        public IActionResult DoctorDepartmentAddEdit()
        {
            UserDropDown();
            DoctorDropDown();
            DepartmentDropDown();
            return View(new DoctorDepartmentModel());
        }

        public IActionResult DoctorDepartmentList()
        {
            DataTable table = DDlist("PR_DoctorDepartment_SelectAll");

            return View(table);
        }
    }
}
