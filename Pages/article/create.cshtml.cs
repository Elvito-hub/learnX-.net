using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final2.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace final2.Pages.article
{
	public class createModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public string message = "";
        private readonly IWebHostEnvironment _environment;


        public Article cm = new Article();
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

                cm.title = Request.Form["title"];
                cm.content = Request.Form["article"];
                cm.communityId = int.Parse("4");
                cm.dateCreated = DateTime.Now.ToString("o");
                cm.CreatorId = int.Parse(userIdClaim.Value);

                var coverPhoto = Request.Form.Files["coverPhoto"];
                if (coverPhoto != null && coverPhoto.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "communityCover");
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + coverPhoto.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        coverPhoto.CopyTo(fileStream);
                    }

                    cm.thumbnail = filePath; // Store the file path in the database
                }



                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                    {
                        string qry = "INSERT INTO article (title, content, dateCreated, creatorId,thumbnail, communityId) VALUES (@title, @content, @datecreated, @creatorId,@thumbnail,  @commid);";

                        conn.Open();
                        using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                        {
                            cmd.Parameters.AddWithValue("@title", cm.title);
                            cmd.Parameters.AddWithValue("@dateCreated", cm.dateCreated);
                            cmd.Parameters.AddWithValue("@content", cm.content);
                            cmd.Parameters.AddWithValue("@creatorId", cm.CreatorId);
                            cmd.Parameters.AddWithValue("@thumbnail", cm.thumbnail);
                            cmd.Parameters.AddWithValue("@commid", cm.communityId);


                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                message = "article created";
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
    public class Article
    {
        public string? articleid { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? dateCreated { get; set; }
        public string? thumbnail { get; set; }
        public string? creatorUsername { get; set; }

        // Reference to the User (assuming the User class exists)
        public int CreatorId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Creator { get; set; } // If you want to directly reference the User object
                                          // Reference to the User (assuming the User class exists)
        public int communityId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Community { get; set; } // If you want to directly reference the User object
    }
}
