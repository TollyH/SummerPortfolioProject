using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class SubmitPortfolioModel : PageBase
    {
        public SubmitPortfolioModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
