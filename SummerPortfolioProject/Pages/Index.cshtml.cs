using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class IndexModel : PageBase
    {
        public IndexModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}