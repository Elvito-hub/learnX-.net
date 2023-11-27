using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using final2.Pages.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages.community
{
    [Authorize(Policy = "AdminOrBloggerOnly")] // This page will be accessible by only Admin
    public class createModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;

        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";

        public Community cm = new Community();
        public string message = "";

        public createModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public void OnGet()
        {
        }
        public void OnPost()
        {


        var userIdClaim = User.FindFirst("blogUserId");
            System.Console.WriteLine("here abut to post");
            //    message = userIdClaim.Value;
            if (userIdClaim != null)
            {

                cm.name = Request.Form["title"];
                cm.description = Request.Form["goals"];
                cm.CreatorId = int.Parse(userIdClaim.Value);
                var coverPhoto = Request.Form.Files["coverPhoto"];
                cm.datecreated = DateTime.Now.ToString("o");
                System.Console.WriteLine(cm.datecreated);
                if (coverPhoto != null && coverPhoto.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "communityCover");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverPhoto.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        coverPhoto.CopyTo(fileStream);
                    }

                    cm.CoverPhotoPath = filePath; // Store the file path in the database
                }

                try
            {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                    {
                        string qry = "INSERT INTO community VALUES(DEFAULT, @name,@dateCreated,false, @description, @coverPhoto, @creatorId)";

                        conn.Open();
                        using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                        {
                            cmd.Parameters.AddWithValue("@name", cm.name);
                            cmd.Parameters.AddWithValue("@dateCreated", cm.datecreated);
                            cmd.Parameters.AddWithValue("@description", cm.description);
                            cmd.Parameters.AddWithValue("@creatorId", cm.CreatorId);
                            cmd.Parameters.AddWithValue("@coverPhoto", cm.CoverPhotoPath);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                message = "community requested, you will be notified.";
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
            else
            {
                message = "no user found";
            }
        }
    }

    public class Community
    {
    public string? communityid { get; set; }
    public string? name { get; set; }
    public string? datecreated { get; set; }
    public Boolean? isapproved { get; set; }
    public string? description { get; set; }
        // Reference to the User (assuming the User class exists)
    public int CreatorId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
    public User Creator { get; set; } // If you want to directly reference the User object
    public string? CoverPhotoPath { get; set; }

    public IFormFile coverPhoto { get; set; }
    }
}
