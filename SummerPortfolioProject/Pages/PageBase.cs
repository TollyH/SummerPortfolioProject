using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace SummerPortfolioProject.Pages
{
    public class PageBase : PageModel
    {
        public MySqlConnection SqlConnection;

        public string? LoggedInUsername;

        public PageBase(MySqlConnection sqlConnection)
        {
            SqlConnection = sqlConnection;
        }

        public struct Permissions
        {
            public bool ModeratePortfolios { get; set; }
            public bool ModifyPortfolios { get; set; }
            public bool ModifyStaffPerms { get; set; }
        }

        public virtual void OnGet()
        {
            LoggedInUsername = HttpContext.Session.GetString("LoggedInUsername");
        }

        /// <summary>
        /// Redirect the user to the forbidden error page if they aren't logged in.
        /// </summary>
        public bool RequireLoggedIn()
        {
            if (LoggedInUsername is null)
            {
                Response.Redirect("/Error/Forbidden");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get a <see cref="Permissions"/> struct representing what permissions the current user has.
        /// </summary>
        public Permissions GetPermissions()
        {
            if (LoggedInUsername is null)
            {
                return new Permissions() { ModeratePortfolios = false, ModifyPortfolios = false, ModifyStaffPerms = false };
            }
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT can_moderate_portfolios, can_modify_portfolios, can_modify_staff_perms FROM staff_accounts " +
                "WHERE username = (@username);", SqlConnection);
            _ = command.Parameters.AddWithValue("username", LoggedInUsername);
            using MySqlDataReader reader = command.ExecuteReader();
            _ = reader.Read();
            Permissions toReturn = new()
            {
                ModeratePortfolios = reader.GetBoolean("can_moderate_portfolios"),
                ModifyPortfolios = reader.GetBoolean("can_modify_portfolios"),
                ModifyStaffPerms = reader.GetBoolean("can_modify_staff_perms")
            };
            SqlConnection.Close();
            return toReturn;
        }

        /// <summary>
        /// Redirect the user to the forbidden error page if they aren't logged in or don't have the correct permissions.
        /// </summary>
        public bool RequirePermissions(bool moderatePortfolios = false, bool modifyPortfolios = false, bool modifyStaffPerms = false)
        {
            if (!RequireLoggedIn())
            {
                return false;
            }
            Permissions currentPermissions = GetPermissions();
            if ((moderatePortfolios && !currentPermissions.ModeratePortfolios)
                || (modifyPortfolios && !currentPermissions.ModifyPortfolios)
                || (modifyStaffPerms && !currentPermissions.ModifyStaffPerms))
            {
                Response.Redirect("/Error/Forbidden");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generate a salted SHA512 hash for a password.
        /// </summary>
        /// <param name="password">The plaintext password to hash</param>
        /// <param name="salt">The optional salt to use. If null a random one will be generated.</param>
        /// <returns>The resulting salted hash in the format "{hash}:{salt}"</returns>
        public string HashPassword(string password, string? salt = null)
        {
            if (salt is null)
            {
                byte[] saltBytes = RandomNumberGenerator.GetBytes(32);
                salt = Convert.ToBase64String(saltBytes);
            }
            SHA512 sha512 = SHA512.Create();
            byte[] computedHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            return $"{Convert.ToHexString(computedHash)}:{salt}";
        }

        /// <summary>
        /// Check the staff accounts database to determine if the provided username and password match
        /// </summary>
        /// <returns>True if the provided username and password match the database, otherwise false</returns>
        public bool Authenticate(string username, string password)
        {
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT password FROM staff_accounts WHERE username = (@username);", SqlConnection);
            _ = command.Parameters.AddWithValue("username", username);
            using MySqlDataReader reader = command.ExecuteReader();
            bool authSuccess = false;
            if (reader.Read())
            {
                // If a matching row was found
                string checkPasswordCombo = reader.GetString("password");
                string storedSalt = checkPasswordCombo.Split(":")[1];
                authSuccess = HashPassword(password, storedSalt) == checkPasswordCombo;
            }
            SqlConnection.Close();
            return authSuccess;
        }
    }
}
