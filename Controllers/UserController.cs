using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class UserController : Controller
    {

        public IConfiguration configuration;

        public UserController (IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #region List from DB

        public DataTable USList(string SP)
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
        public IActionResult UserDelete(int USERID)
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
                        command.CommandText = "PR_User_DeleteByPK";

                        command.Parameters.Add("@USERID", SqlDbType.Int).Value = USERID;
                        TempData["Successfully"] = "Deleted Successfully.";
                        command.ExecuteNonQuery();

                        return RedirectToAction("UserList");
                    }
                }
            }
            catch(Exception e)
            {
                TempData["Unsuccessfully"] = "Delete Failed: " + e.Message;
                return RedirectToAction("UserList");
            }
            
        }
        #endregion

        #region Add/Edit DB 
        public IActionResult UserSave(UserModel userModel)
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

                            if (userModel.UserID != 0)
                            {
                                command.CommandText = "PR_User_UpdateByPK";
                                command.Parameters.AddWithValue("@USERID", userModel.UserID);
                                userModel.Modified = DateTime.Now; 
                            }
                            else
                            {
                                command.CommandText = "PR_User_Insert";
                                userModel.IsActive = true;
                                userModel.Created = DateTime.Now;
                                userModel.Modified = DateTime.Now;
                            }

                            command.Parameters.AddWithValue("@USRENAME", userModel.UserName);
                            command.Parameters.AddWithValue("@PASSWORD", userModel.Password);
                            command.Parameters.AddWithValue("@EMAIL", userModel.Email);
                            command.Parameters.AddWithValue("@MOBILENO", userModel.MobileNo);
                            command.Parameters.AddWithValue("@ISACTIVE", userModel.IsActive);
                            command.Parameters.AddWithValue("@MODIFIED", userModel.Modified);

                            int rows = command.ExecuteNonQuery();

                            TempData["UserInsertUpdateMessage"] = rows > 0
                                ? (userModel.UserID == 0 ? "User added successfully!" : "User updated successfully!")
                                : "User Add/update Failed";
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["UserInsertUpdateMessage"] = "Error: " + e.Message;
                }
            }
            else
            {
                return View("UserAddEdit", userModel);
            }

            return RedirectToAction("UserList");
        }
        #endregion

        #region GET method for Edit - This was missing and causing 404
        public IActionResult UserUpdate(int UserID)
        {
            if(UserID == null)
            {
                ViewBag.ErrorMessage = "UserID ID is required";
                return RedirectToAction("UserList");
            }

            string connectionString = this.configuration.GetConnectionString("myConnection");
            UserModel userModel = new UserModel();

            try
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using(SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_User_SelectByPK";

                        command.Parameters.AddWithValue("@USERID", UserID);

                        SqlDataReader reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            userModel.UserID = Convert.ToInt32(reader["UserID"]);
                            userModel.UserName = reader["UserName"].ToString();
                            userModel.Password = reader["Password"].ToString();
                            userModel.Email = reader["Email"].ToString();
                            userModel.MobileNo = reader["MobileNo"].ToString();
                            userModel.Created = Convert.ToDateTime(reader["Created"]);
                            userModel.Modified = Convert.ToDateTime(reader["Modified"]);
                            userModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "User not found for edit";
                            return RedirectToAction("UserList");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = "Error loading patient: " + e.Message;
                return RedirectToAction("UserList");
            }

            return View("UserAddEdit", userModel);
        }
        #endregion


        public IActionResult UserAddEdit()
        {
            return View(new UserModel());   
        }

        public IActionResult UserList()
        {

            DataTable table = USList("PR_User_SelectAll");
            return View(table);
        }
    }
}
