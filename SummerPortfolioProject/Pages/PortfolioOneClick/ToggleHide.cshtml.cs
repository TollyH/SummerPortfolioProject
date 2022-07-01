using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class ToggleHideModel : PageBase
    {
        public ToggleHideModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
