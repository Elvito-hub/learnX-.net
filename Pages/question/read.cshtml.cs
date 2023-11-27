using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final2.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace final2.Pages.question
{
	public class readModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public Question cm = new Question();
        public List<Comment> comments = new List<Comment>();

        public string message = "";

        public void OnGet(int id)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
            {
                conn.Open();
                string qry = "SELECT a.articleid, a.title, a.content, a.dateCreated, a.creatorId, a.thumbnail, a.communityId,a.isThreadClosed, u.username FROM qa a JOIN users u ON a.creatorId = u.userId WHERE a.articleid = @comid;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                {
                    cmd.Parameters.AddWithValue("@comid", id);

                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            cm.articleid = rdr.GetInt32(0).ToString();
                            cm.title = rdr.GetString(1);
                            cm.content = rdr.GetString(2);
                            cm.dateCreated = rdr.GetString(3);
                            cm.CreatorId = rdr.GetInt32(4);
                            if (!rdr.IsDBNull(5))
                            {
                                cm.thumbnail = rdr.GetString(5);
                                int index = cm.thumbnail.IndexOf("communityCover");

                                // Check if "communityCover" is found in the string
                                if (index != -1)
                                {
                                    // Extract the substring after "communityCover" (inclusive)
                                    string extractedPath = cm.thumbnail.Substring(index + "communityCover".Length);

                                    // Add a leading '/' if needed
                                    if (extractedPath.StartsWith("/"))
                                    {
                                        extractedPath = extractedPath.Substring(1); // Remove the leading '/'
                                    }

                                    // Now, extractedPath will contain the string after "communityCover"
                                    Console.WriteLine(extractedPath); // Use this extractedPath as needed
                                    cm.thumbnail = "/communityCover/" + extractedPath;
                                }
                            }
                            cm.communityId = rdr.GetInt32(6);
                            cm.isThreadClosed = rdr.GetBoolean(7);
                            cm.creatorUsername = rdr.GetString(8);
                            if (DateTimeOffset.TryParse(cm.dateCreated, out DateTimeOffset dateTimeOffset))
                            {
                                DateTime normalDate = dateTimeOffset.UtcDateTime;
                                string formattedDate = normalDate.ToString("dd/MM/yyyy HH:mm");
                                Console.WriteLine(formattedDate); // Display or use the formatted date as needed
                                cm.dateCreated = formattedDate;
                            }
                        }
                    }
                }
                conn.Close();

            }
            using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
            {
                conn.Open();
                string qry = "SELECT a.commentid, a.content, a.dateCreated, a.creatorId,a.iscorrect, u.username FROM comments a JOIN users u ON a.creatorId = u.userId WHERE a.qaid = @comid;";
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                {
                    cmd.Parameters.AddWithValue("@comid", id);

                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Comment cm = new Comment();
                            cm.commentId = rdr.GetInt32(0).ToString();
                            cm.content = rdr.GetString(1);
                            cm.dateCreated = rdr.GetString(2);
                            cm.CreatorId = rdr.GetInt32(3);
                            cm.iscorrect = rdr.GetBoolean(4);
                            cm.creatorUsername = rdr.GetString(5);
                            if (DateTimeOffset.TryParse(cm.dateCreated, out DateTimeOffset dateTimeOffset))
                            {
                                DateTime normalDate = dateTimeOffset.UtcDateTime;
                                string formattedDate = normalDate.ToString("dd/MM/yyyy HH:mm");
                                Console.WriteLine(formattedDate); // Display or use the formatted date as needed
                                cm.dateCreated = formattedDate;
                            }
                            comments.Add(cm);
                        }
                    }
                }

            }

        }
        public void OnPost(int id)
        {
            var userIdClaim = User.FindFirst("blogUserId");
            System.Console.WriteLine("here abut to post");
            //    message = userIdClaim.Value;
            if (userIdClaim != null)
            {

                Comment cm = new Comment();

                cm.content = Request.Form["article"];
                cm.dateCreated = DateTime.Today.ToString("o");
                cm.CreatorId = int.Parse(userIdClaim.Value);
                cm.qaId = id;



                try
                {
                    using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                    {
                        string qry = "INSERT INTO comments (content, dateCreated, creatorId, qaId,iscorrect) VALUES ( @content, @datecreated, @creatorId,  @qaId,false);";

                        conn.Open();
                        using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                        {
                            cmd.Parameters.AddWithValue("@dateCreated", cm.dateCreated);
                            cmd.Parameters.AddWithValue("@content", cm.content);
                            cmd.Parameters.AddWithValue("@creatorId", cm.CreatorId);
                            cmd.Parameters.AddWithValue("@qaId", cm.qaId);


                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                message = "comment created";
                                Response.Redirect("/question/read/"+id);
                            }
                            else
                            {
                                message = "comment not created";
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
    public class Comment
    {
        public string? commentId { get; set; }
        public string? content { get; set; }
        public string? dateCreated { get; set; }
        public string? creatorUsername { get; set; }
        public int? qaId { get; set; }
        public bool? iscorrect { get; set; }

        // Reference to the User (assuming the User class exists)
        public int CreatorId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Creator { get; set; } // If you want to directly reference the User object
                                          // Reference to the User (assuming the User class exists)
        public int communityId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Community { get; set; } // If you want to directly reference the User object
    }
}
