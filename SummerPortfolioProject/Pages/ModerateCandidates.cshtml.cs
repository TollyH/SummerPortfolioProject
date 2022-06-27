using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SummerPortfolioProject.Pages
{
    public class ModerateCandidatesModel : PageBase
    {
        public ModerateCandidatesModel(MySqlConnector.MySqlConnection sqlConnection) : base(sqlConnection) { }
    }
}
