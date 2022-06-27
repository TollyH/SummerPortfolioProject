using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class StaffLogoutModel : PageBase
    {
        public StaffLogoutModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
