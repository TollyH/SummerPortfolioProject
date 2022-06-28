using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class StaffLoginModel : PageBase
    {
        public StaffLoginModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }

        internal bool showAuthError = false;

        public void OnPost()
        {
            string? username = HttpContext.Request.Form["Username"];
            string? password = HttpContext.Request.Form["Password"];

            if (username is not null && password is not null)
            {
                if (Authenticate(username, password))
                {
                    HttpContext.Session.SetString("LoggedInUsername", username);
                    Response.Redirect("/?newauth=1");
                }
            }
            showAuthError = true;
        }
    }
}
