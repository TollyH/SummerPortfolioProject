using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages.Error
{
    public class ForbiddenModel : PageBase
    {
        public ForbiddenModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
