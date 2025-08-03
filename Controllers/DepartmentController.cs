using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DepartmentController : Controller
    {

        public IConfiguration configuration;

        public DepartmentController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #region List from DB
        public DataTable DeList(string SP)
        {
            string connectionString = this.configuration.GetConnectionString("myConnection");

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using(SqlCommand  command = connection.CreateCommand())
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

        #region Delete DB
        public IActionResult DepartmentDelete(int departmentID)
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
                        command.CommandText = "PR_Department_DeleteByPK";

                        TempData["Successfully"] = "Deleted Successfully.";
                        command.Parameters.AddWithValue("@DEPARTMENTID", departmentID);

                        command.ExecuteNonQuery();
                        return RedirectToAction("DepartmentList");
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["Unsuccessfully"] = "Delete Failed: " + ex.Message;
                return RedirectToAction("DepartmentList");
            }
        }
        #endregion
       
        #region Add/Edit DB 
        public IActionResult DepartmentSave(DepartmentModel departmentModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = this.configuration.GetConnectionString("myConnection");

                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using(SqlCommand  command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            if(departmentModel.DepartmentID != 0)
                            {
                                command.CommandText = "PR_Department_UpdateByPK";
                                command.Parameters.AddWithValue("@DEPARTMENTID", departmentModel.DepartmentID);
                                departmentModel.Modified = DateTime.Now; 
                            }
                            else
                            {
                                command.CommandText = "PR_Department_Insert";
                                departmentModel.IsActive = true;
                                departmentModel.Created = DateTime.Now;
                                departmentModel.Modified = DateTime.Now;
                                departmentModel.UserID = 6;
                            }

                            command.Parameters.AddWithValue("@DEPARTMENTNAME", departmentModel.DepartmentName);
                            command.Parameters.AddWithValue("@DESCRIPTION", departmentModel.Description);
                            command.Parameters.AddWithValue("@ISACTIVE", departmentModel.IsActive);
                            command.Parameters.AddWithValue("@MODIFIED", departmentModel.Modified);
                            command.Parameters.AddWithValue("@USERID", departmentModel.UserID);
                            
                            int rows = command.ExecuteNonQuery();

                            TempData["DepartmentInsertUpdateMessage"] = rows > 0
                                ? (departmentModel.DepartmentID == 0 ? "Department added successfully!" : "Department updated successfully!")
                                : "Department Add/update Failed";
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["DepartmentInsertUpdateMessage"] = "Error: " + e.Message;
                }
            }
            else
            {
                return View("DepartmentAddEdit", departmentModel);
            }
            return RedirectToAction("DepartmentList");
        }
        #endregion

        #region GET method for Edit - This was missing and causing 404
        public IActionResult DepartmentUpdate(int departmentID)
        {
            if(departmentID == 0)
            {
                ViewBag.ErrorMessage = "DepartmentID  is required";
                return RedirectToAction("DepartmentList");
            }

            string connectionString = this.configuration.GetConnectionString("myConnection");
            DepartmentModel departmentModel = new DepartmentModel();

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using(SqlCommand  command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_Department_SelectByPK";
                        command.Parameters.AddWithValue("@DEPARTMENTID", departmentID);

                        SqlDataReader reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            departmentModel.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                            departmentModel.DepartmentName = reader["DepartmentName"].ToString();
                            departmentModel.Description = reader["Description"].ToString();
                            departmentModel.Created = Convert.ToDateTime(reader["Created"]);
                            departmentModel.Modified = Convert.ToDateTime(reader["Modified"]);
                            departmentModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Department not found for edit";
                            return RedirectToAction("DepartmentList");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                TempData["ErrorMessage"] = "Error loading Department: " + e.Message;
                return RedirectToAction("DepartmentList");
            }

            return View("DepartmentAddEdit", departmentModel);
        }
        #endregion

        public IActionResult DepartmentList()
        {
            DataTable table = DeList("PR_Department_SelectAll");
            return View(table);
        }

        public IActionResult DepartmentAddEdit()
        {
            return View(new DepartmentModel());
        }
    }
}
