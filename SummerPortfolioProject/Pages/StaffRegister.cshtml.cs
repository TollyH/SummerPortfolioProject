using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class StaffRegisterModel : PageBase
    {
        public StaffRegisterModel(MySqlConnection sqlConnection) : base(sqlConnection) { }

        internal bool matchError = false;

        public void OnPost()
        {
            string username = HttpContext.Request.Form["Username"];
            string email = HttpContext.Request.Form["Email"];
            string emailConfirm = HttpContext.Request.Form["EmailConfirm"];
            string password = HttpContext.Request.Form["Password"];
            string passwordConfirm = HttpContext.Request.Form["PasswordConfirm"];

            if (email != emailConfirm || password != passwordConfirm)
            {
                matchError = true;
            }
            else
            {
                SqlConnection.Open();
                using MySqlCommand command = new("INSERT INTO staff_accounts (`username`, `password`, `email`) VALUES (@username, @password, @email);", SqlConnection);
                _ = command.Parameters.AddWithValue("username", username);
                _ = command.Parameters.AddWithValue("password", HashPassword(password));
                _ = command.Parameters.AddWithValue("email", email);
                _ = command.ExecuteNonQuery();
                SqlConnection.Close();

                HttpContext.Session.SetString("LoggedInUsername", username);
                Response.Redirect("/?newauth=1");
            }
        }
    }
}
