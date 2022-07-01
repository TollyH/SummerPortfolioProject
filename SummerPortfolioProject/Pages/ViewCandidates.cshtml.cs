using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class ViewCandidatesModel : PageBase
    {
        public class PublicCandidate
        {
            public int Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public List<KeyValuePair<string, int>> Skills { get; set; }

            public PublicCandidate()
            {
                // Default values to prevent null
                Id = 0;
                Forename = "";
                Surname = "";
                Skills = new();
            }
        }

        internal List<PublicCandidate> publicCandidates = new();

        public ViewCandidatesModel(MySqlConnection sqlConnection) : base(sqlConnection) { }

        public override void OnGet()
        {
            base.OnGet();
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT id, surname, forename FROM portfolios WHERE approved=b'1';", SqlConnection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                publicCandidates.Add(new()
                {
                    Id = reader.GetInt32("id"),
                    Forename = reader.GetString("forename"),
                    Surname = reader.GetString("surname")
                });
            }
            SqlConnection.Close();

            foreach (PublicCandidate candidate in publicCandidates)
            {
                SqlConnection.Open();
                using MySqlCommand skillsCommand = new("SELECT name, rating FROM portfolio_skills WHERE portfolio_id=@id;", SqlConnection);
                _ = skillsCommand.Parameters.AddWithValue("id", candidate.Id);
                MySqlDataReader skillsReader = skillsCommand.ExecuteReader();
                while (skillsReader.Read())
                {
                    candidate.Skills.Add(new(skillsReader.GetString("name"), skillsReader.GetInt32("rating")));
                }
                SqlConnection.Close();
            }
        }
    }
}
