using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class PageBase : PageModel
    {
        public string? LoggedInUsername;

        public PageBase() : base()
        {
            LoggedInUsername = HttpContext.Session.GetString("LoggedInUsername");
        }

        public void RequireLoggedIn()
        {
            if (LoggedInUsername is null)
            {
                Response.Redirect("/Error/Forbidden");
            }
        }
    }
}
