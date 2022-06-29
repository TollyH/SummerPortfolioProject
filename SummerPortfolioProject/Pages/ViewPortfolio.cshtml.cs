using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class ViewPortfolioModel : PageBase
    {
        internal string? surname;
        internal string? forename;
        internal DateTime dateOfBirth;
        internal string? location;
        internal string? email;
        internal string? imageUrl;
        internal List<KeyValuePair<string, string>> skills = new();
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
                dateOfBirth = reader.GetDateTime("dob");
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
        }
    }
}
