using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages.Account
{
    public class RegisterModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public string message = "";
        public User us = new User();

        public void OnGet()
        {
        }
        public void OnPost()
        {


                us.userName = Request.Form["userName"];
                us.email = Request.Form["email"];
                us.password = Request.Form["password"];
                us.phoneNumber = Request.Form["phoneNumber"];
                us.dateJoined = DateTime.Now.ToString("o");

            us.role = "user";

                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                    {
                        string qry = "INSERT INTO USERS VALUES(DEFAULT, @email, @userName, @password, @phoneNumber,@datejoined,false,@role)";

                        conn.Open();
                        using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                        {
                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(us.password); // hash password using encrypt
                            cmd.Parameters.AddWithValue("@email", us.email);
                            cmd.Parameters.AddWithValue("@userName", us.userName);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);
                            cmd.Parameters.AddWithValue("@phoneNumber", us.phoneNumber);
                            cmd.Parameters.AddWithValue("@datejoined", us.dateJoined);


                        cmd.Parameters.AddWithValue("@role", us.role);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                message = "User Created";
                                // Response.Redirect("/Student");
                            }
                            else
                            {
                                message = "user not created";
                            }
                        }
                        conn.Close();
                    }

                }
                catch (Exception ex)
                {
                    message = ex.Message;

                }

        }

    }
    public class User
    {
        public string? email { get; set; }
        public string? userName { get; set; }
        public string? password { get; set; }
        public string? dateJoined { get; set; }

        public string? phoneNumber { get; set; }
        public string? role { get; set; }

    }
}
