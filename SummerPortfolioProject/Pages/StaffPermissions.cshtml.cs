using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Primitives;
using MySqlConnector;

namespace SummerPortfolioProject.Pages
{
    public class StaffPermissionsModel : PageBase
    {
        public StaffPermissionsModel(MySqlConnection sqlConnection) : base(sqlConnection) { }
        public class StaffMember
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public Permissions Permissions { get; set; }

            public StaffMember()
            {
                // Set default values to avoid null
                Id = 0;
                Username = "";
                Email = "";
                Permissions = new();
            }
        }

        internal bool notFound = false;
        internal List<StaffMember> staffMembers = new();

        public override void OnGet()
        {
            base.OnGet();
            SqlConnection.Open();
            using MySqlCommand command = new("SELECT id, username, email, can_moderate_portfolios, can_modify_portfolios, can_modify_staff_perms " +
                "FROM staff_accounts;", SqlConnection);
            using MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                staffMembers.Add(new()
                {
                    Id = reader.GetInt32("id"),
                    Username = reader.GetString("username"),
                    Email = reader.GetString("email"),
                    Permissions = new()
                    {
                        ModeratePortfolios = reader.GetBoolean("can_moderate_portfolios"),
                        ModifyPortfolios = reader.GetBoolean("can_modify_portfolios"),
                        ModifyStaffPerms = reader.GetBoolean("can_modify_staff_perms")
                    }
                });
            }
            SqlConnection.Close();
        }

        public void OnPost()
        {
            // Ensure username has been correctly retrieved before checking permissions
            base.OnGet();
            if (RequirePermissions(modifyStaffPerms: true))
            {
                string username = Request.Form["Username"];
                int moderatePortfolios = Request.Form["ModeratePortfolios"] == StringValues.Empty ? 0 : 1;
                int modifyPortfolios = Request.Form["ModifyPortfolios"] == StringValues.Empty ? 0 : 1;
                int modifyStaffPermissions = Request.Form["ModifyStaffPermissions"] == StringValues.Empty ? 0 : 1;

                SqlConnection.Open();
                using MySqlCommand command = new("UPDATE staff_accounts " +
                    "SET can_moderate_portfolios=@moderatePortfolios, can_modify_portfolios=@modifyPortfolios, can_modify_staff_perms=@modifyStaffPerms " +
                    "WHERE username=@username;", SqlConnection);
                _ = command.Parameters.AddWithValue("username", username);
                _ = command.Parameters.AddWithValue("moderatePortfolios", moderatePortfolios);
                _ = command.Parameters.AddWithValue("modifyPortfolios", modifyPortfolios);
                _ = command.Parameters.AddWithValue("modifyStaffPerms", modifyStaffPermissions);
                int affectedRows = command.ExecuteNonQuery();
                if (affectedRows == 0)
                {
                    notFound = true;
                }
                SqlConnection.Close();
                OnGet();
            }
        }
    }
}
