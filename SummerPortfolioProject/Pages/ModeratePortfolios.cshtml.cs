using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class ModeratePortfoliosModel : PageBase
    {
        public ModeratePortfoliosModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
