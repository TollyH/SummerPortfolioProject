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

        internal string surname = "";
        internal string forename = "";
        internal string dateOfBirth = "";
        internal string location = "";
        internal string email = "";
        internal string? imageUrl = "";
        internal List<KeyValuePair<string, int>> skills = new();

        public override void OnGet()
        {
            base.OnGet();
            // Prefill values if modifying instead of adding
            if (Request.Query["edit_id"] != StringValues.Empty && RequirePermissions(modifyPortfolios: true))
            {
                HttpContext.Session.SetInt32("EditId", int.Parse(Request.Query["edit_id"]));
                SqlConnection.Open();
                using MySqlCommand command = new("SELECT surname, forename, dob, location, email, image_url, approved FROM portfolios WHERE id=@id;", SqlConnection);
                _ = command.Parameters.AddWithValue("id", Request.Query["edit_id"].ToString());
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    surname = reader.GetString("surname");
                    forename = reader.GetString("forename");
                    dateOfBirth = reader.GetDateOnly("dob").ToShortDateString();
                    location = reader.GetString("location");
                    email = reader.GetString("email");
                    imageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) ? "" : reader.GetString("image_url");
                }
                else
                {
                    databaseError = true;
                }
                SqlConnection.Close();

                SqlConnection.Open();
                using MySqlCommand skillsCommand = new("SELECT name, rating FROM portfolio_skills WHERE portfolio_id=@id;", SqlConnection);
                _ = skillsCommand.Parameters.AddWithValue("id", Request.Query["edit_id"].ToString());
                MySqlDataReader skillsReader = skillsCommand.ExecuteReader();
                while (skillsReader.Read())
                {
                    skills.Add(new(skillsReader.GetString("name"), skillsReader.GetInt32("rating")));
                }
                SqlConnection.Close();
            }
        }

        public void OnPost()
        {
            if (HttpContext.Session.GetInt32("NewId") is null)
            {
                // Will be set if the user is editing instead of adding
                int? editId = HttpContext.Session.GetInt32("EditId");
                // User has submitted first part of the form
                stage = 1;
                surname = HttpContext.Request.Form["Surname"];
                forename = HttpContext.Request.Form["Forename"];
                dateOfBirth = HttpContext.Request.Form["DateOfBirth"];
                location = HttpContext.Request.Form["Location"];
                email = HttpContext.Request.Form["Email"];
                imageUrl = HttpContext.Request.Form["ImageUrl"];
                if (imageUrl == "")
                {
                    imageUrl = null;
                }

                string commandText = editId is null
                    ? "INSERT INTO portfolios (`surname`, `forename`, `dob`, `location`, `email`, `image_url`) VALUES " +
                        "(@surname, @forename, @dob, @location, @email, @imageUrl);"
                    : "UPDATE portfolios SET surname=@surname, forename=@forename, dob=@dob, location=@location, email=@email, image_url=@imageUrl WHERE id=@id;";
                SqlConnection.Open();
                using MySqlCommand command = new(commandText, SqlConnection);
                _ = command.Parameters.AddWithValue("surname", surname);
                _ = command.Parameters.AddWithValue("forename", forename);
                _ = command.Parameters.AddWithValue("dob", dateOfBirth);
                _ = command.Parameters.AddWithValue("location", location);
                _ = command.Parameters.AddWithValue("email", email);
                _ = command.Parameters.AddWithValue("imageUrl", imageUrl);
                if (editId is not null)
                {
                    _ = command.Parameters.AddWithValue("id", Request.Query["edit_id"].ToString());
                }
                _ = command.ExecuteNonQuery();

                if (editId is null)
                {
                    using MySqlCommand autoIncrementCommand = new("SELECT LAST_INSERT_ID();", SqlConnection);
                    MySqlDataReader reader = autoIncrementCommand.ExecuteReader();
                    _ = reader.Read();
                    HttpContext.Session.SetInt32("NewId", reader.GetInt32(0));
                }
                else
                {
                    HttpContext.Session.SetInt32("NewId", editId.Value);
                }
                SqlConnection.Close();

                Dictionary<string, int> existingSkills = new();
                if (editId is not null)
                {
                    SqlConnection.Open();
                    using MySqlCommand skillsCommand = new("SELECT name, rating FROM portfolio_skills WHERE portfolio_id=@id;", SqlConnection);
                    _ = skillsCommand.Parameters.AddWithValue("id", editId.ToString());
                    MySqlDataReader skillsReader = skillsCommand.ExecuteReader();
                    while (skillsReader.Read())
                    {
                        existingSkills[skillsReader.GetString("name")] = skillsReader.GetInt32("rating");
                    }
                    SqlConnection.Close();
                }

                string[] skillNames = HttpContext.Request.Form["Skills"].ToString().ReplaceLineEndings().Split(Environment.NewLine);
                foreach (string skillName in skillNames)
                {
                    skills.Add(new(skillName, existingSkills.GetValueOrDefault(skillName, 0)));
                }
                if (skills.Count == 0 || (skills.Count == 1 && skills[0].Key == ""))
                {
                    HttpContext.Session.Remove("NewId");
                    HttpContext.Session.Remove("EditId");
                    Response.Redirect("/");
                }
            }
            else
            {
                // User has submitted skill rankings
                int? editId = HttpContext.Session.GetInt32("EditId");
                if (editId is not null)
                {
                    SqlConnection.Open();
                    using MySqlCommand skillsDeleteCommand = new("DELETE FROM portfolio_skills WHERE portfolio_id=@id;", SqlConnection);
                    _ = skillsDeleteCommand.Parameters.AddWithValue("id", editId);
                    _ = skillsDeleteCommand.ExecuteNonQuery();
                    SqlConnection.Close();
                }
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
                HttpContext.Session.Remove("EditId");
                Response.Redirect("/");
            }
        }
    }
}
