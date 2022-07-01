using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class ModeratePortfoliosModel : PageBase
    {
        public class FullCandidate
        {
            public int Id { get; set; }
            public string Forename { get; set; }
            public string Surname { get; set; }
            public DateOnly DateOfBirth { get; set; }
            public string Location { get; set; }
            public string Email { get; set; }
            public bool Approved { get; set; }
            public List<KeyValuePair<string, int>> Skills { get; set; }

            public FullCandidate()
            {
                // Default values to prevent null
                Id = 0;
                Forename = "";
                Surname = "";
                DateOfBirth = new();
                Location = "";
                Email = "";
                Approved = false;
                Skills = new();
            }
        }

        internal List<FullCandidate> fullCandidates = new();

        public ModeratePortfoliosModel(MySqlConnection sqlConnection) : base(sqlConnection) { }

        public override void OnGet()
        {
            base.OnGet();
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT id, surname, forename, dob, location, email, approved FROM portfolios;", SqlConnection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                fullCandidates.Add(new()
                {
                    Id = reader.GetInt32("id"),
                    Forename = reader.GetString("forename"),
                    Surname = reader.GetString("surname"),
                    DateOfBirth = reader.GetDateOnly("dob"),
                    Location = reader.GetString("location"),
                    Email = reader.GetString("email"),
                    Approved = reader.GetBoolean("approved")
                });
            }
            SqlConnection.Close();

            foreach (FullCandidate candidate in fullCandidates)
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
