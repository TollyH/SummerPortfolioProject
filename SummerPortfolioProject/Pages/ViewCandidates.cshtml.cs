using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class ViewCandidatesModel : PageBase
    {
        public ViewCandidatesModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
