using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class ViewPortfolioModel : PageBase
    {
        internal string? surname;
        internal string? forename;
        internal DateOnly dateOfBirth;
        internal string? location;
        internal string? email;
        internal string? imageUrl;
        internal List<KeyValuePair<string, int>> skills = new();
        internal bool approved = false;

        internal bool notFound = false;

        public ViewPortfolioModel(MySqlConnection sqlConnection) : base(sqlConnection) { }

        public override void OnGet()
        {
            base.OnGet();
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT surname, forename, dob, location, email, image_url, approved FROM portfolios WHERE id=@id;", SqlConnection);
            _ = command.Parameters.AddWithValue("id", Request.Query["id"].ToString());
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                surname = reader.GetString("surname");
                forename = reader.GetString("forename");
                dateOfBirth = reader.GetDateOnly("dob");
                location = reader.GetString("location");
                email = reader.GetString("email");
                imageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString("image_url");
                approved = reader.GetBoolean("approved");
            }
            else
            {
                notFound = true;
            }
            SqlConnection.Close();

            SqlConnection.Open();
            using MySqlCommand skillsCommand = new("SELECT name, rating FROM portfolio_skills WHERE portfolio_id=@id;", SqlConnection);
            _ = skillsCommand.Parameters.AddWithValue("id", Request.Query["id"].ToString());
            MySqlDataReader skillsReader = skillsCommand.ExecuteReader();
            while (skillsReader.Read())
            {
                skills.Add(new(skillsReader.GetString("name"), skillsReader.GetInt32("rating")));
            }
            SqlConnection.Close();
        }
    }
}
