using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class DeleteModel : PageBase
    {
        public DeleteModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
