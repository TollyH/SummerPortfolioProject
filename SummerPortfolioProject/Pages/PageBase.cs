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

        public void OnGet()
        {
            LoggedInUsername = HttpContext.Session.GetString("LoggedInUsername");
        }

        /// <summary>
        /// Redirect the user to the forbidden error page if they aren't logged in.
        /// </summary>
        public void RequireLoggedIn()
        {
            if (LoggedInUsername is null)
            {
                Response.Redirect("/Error/Forbidden");
            }
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
    }
}
