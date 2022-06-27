using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class StaffLoginModel : PageBase
    {
        public StaffLoginModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
