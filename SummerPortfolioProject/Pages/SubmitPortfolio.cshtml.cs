using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class SubmitPortfolioModel : PageBase
    {
        public SubmitPortfolioModel(MySqlConnection sqlConnection) : base(sqlConnection) { }

        internal int stage = 0;
        internal bool databaseError = false;
        internal string[] skills = Array.Empty<string>();

        public void OnPost()
        {
            if (HttpContext.Session.GetInt32("NewId") is null)
            {
                // User has submitted first part of the form
                stage = 1;
                string surname = HttpContext.Request.Form["Surname"];
                string forename = HttpContext.Request.Form["Forename"];
                string dateOfBirth = HttpContext.Request.Form["DateOfBirth"];
                string location = HttpContext.Request.Form["Location"];
                string email = HttpContext.Request.Form["Email"];
                string? imageUrl = HttpContext.Request.Form["ImageUrl"];
                if (imageUrl == "")
                {
                    imageUrl = null;
                }

                SqlConnection.Open();
                using MySqlCommand command = new("INSERT INTO portfolios (`surname`, `forename`, `dob`, `location`, `email`, `image_url`) VALUES " +
                    "(@surname, @forename, @dob, @location, @email, @imageUrl);", SqlConnection);
                _ = command.Parameters.AddWithValue("surname", surname);
                _ = command.Parameters.AddWithValue("forename", forename);
                _ = command.Parameters.AddWithValue("dob", dateOfBirth);
                _ = command.Parameters.AddWithValue("location", location);
                _ = command.Parameters.AddWithValue("email", email);
                _ = command.Parameters.AddWithValue("imageUrl", imageUrl);
                _ = command.ExecuteNonQuery();

                using MySqlCommand autoIncrementCommand = new("SELECT LAST_INSERT_ID();", SqlConnection);
                MySqlDataReader reader = autoIncrementCommand.ExecuteReader();
                _ = reader.Read();
                HttpContext.Session.SetInt32("NewId", reader.GetInt32(0));
                SqlConnection.Close();

                skills = HttpContext.Request.Form["Skills"].ToString().ReplaceLineEndings().Split(Environment.NewLine);
                if (skills.Length == 0 || (skills.Length == 1 && skills[0] == ""))
                {
                    HttpContext.Session.Remove("NewId");
                    Response.Redirect("/");
                }
            }
            else
            {
                // User has submitted skill rankings
                foreach (KeyValuePair<string, StringValues> skill in HttpContext.Request.Form)
                {
                    if (skill.Key == "__RequestVerificationToken")
                    {
                        continue;
                    }
                    SqlConnection.Open();
                    using MySqlCommand command = new("INSERT INTO portfolio_skills (`portfolio_id`, `name`, `rating`) VALUES "
                        + "(@portfolioId, @name, @rating);", SqlConnection);
                    _ = command.Parameters.AddWithValue("portfolioId", HttpContext.Session.GetInt32("NewId"));
                    _ = command.Parameters.AddWithValue("name", skill.Key);
                    _ = command.Parameters.AddWithValue("rating", skill.Value.ToString());
                    _ = command.ExecuteNonQuery();
                    SqlConnection.Close();
                }
                HttpContext.Session.Remove("NewId");
                Response.Redirect("/");
            }
        }
    }
}
