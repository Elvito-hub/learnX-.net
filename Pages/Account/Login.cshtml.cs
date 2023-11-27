using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages.Account
{
	public class LoginModel : PageModel
    {

        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public string message = "";


        public Credential credential = new Credential();

        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            credential.email = Request.Form["email"];
            credential.password = Request.Form["password"];

            if (VerifyCredentialFromDatabase(credential.email, credential.password))
            {
                // Get the user information from the database
                MyUser user = GetUserFromDatabase(credential.email);
                message = credential.email;
                System.Console.WriteLine(user!=null);


                if (user != null)
                {
                    // Creating the security context
                    var claims = new List<Claim>
                    {
                        new Claim("blogUserId", user.id + ""),
                        new Claim("blogUserEmail", user.email),
                        new Claim("blogUserRole", user.role)
                };
                    System.Console.WriteLine(user.id);

                    // Add claims to identity, also specify authentication name (anyname)
                    var identity = new ClaimsIdentity(claims, "RedBlogCookieAuth");
                    // Principal contains the security context, and can have many identities
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                    // Serialize claims principal into a string
                    // Then encrypt that string, save that as a cookie in the HttpContext
                    await HttpContext.SignInAsync("RedBlogCookieAuth", claimsPrincipal);

                    return RedirectToPage("/Index");
                }
                else
                {
                    return Page();
                }

            }
            message = "Wrong credentials, try again";
            return Page();
        }

        private bool VerifyCredentialFromDatabase(string userEmail, string password)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    // Open the database connection
                    conn.Open();

                    // Prepare the SQL query
                    string qry = "SELECT userid, password FROM USERS WHERE email=@v_email";

                    // Create the command and parameters
                    using (NpgsqlCommand command = new NpgsqlCommand(qry, conn))
                    {
                        command.Parameters.AddWithValue("@v_email", userEmail);

                        // Execute the query and retrieve the result
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if the user exists and the password is valid
                            if (reader.Read())
                            {
                                string storedHashedPassword = reader.GetString("password");

                                return BCrypt.Net.BCrypt.Verify(password, storedHashedPassword);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }

            return false;
        }
        private MyUser GetUserFromDatabase(string email)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    // Open the database connection
                    conn.Open();

                    // Prepare the SQL query
                    string qry = "SELECT userid, username, email,password,role FROM USERS WHERE email=@v_email";

                    // Create the command and parameters
                    using (NpgsqlCommand command = new NpgsqlCommand(qry, conn))
                    {
                        command.Parameters.AddWithValue("@v_email", email);

                        // Execute the query and retrieve the result
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Retrieve the user information from the reader
                                int bloggerUserId = reader.GetInt32("userid");
                                string bloggerFullname = reader.GetString("username");
                                string bloggerEmail = reader.GetString("email");
                                string bloggerPassword = reader.GetString("password");
                                string bloggerRole = reader.GetString("role");

                                // Create a User object with the retrieved information
                                MyUser blogUser = new MyUser
                                {
                                    id = bloggerUserId,
                                    userName = bloggerFullname,
                                    email = bloggerEmail,
                                    password = bloggerPassword,
                                    role = bloggerRole
                                };

                                return blogUser;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return null;
            }


            return null;
        }
    }
    public class Credential
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }
    public class MyUser
    {
        public int? id { get; set; }
        public string? email { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? phoneNumber { get; set; }
        public string? role { get; set; }

    }
}
