using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final2.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages.community
{
	public class detailsModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public List<Article> articles = new List<Article>();
        public List<Question> questions = new List<Question>();
        public string tabselected = "";
        public int selectedId;



        public void OnGet(int id)
        {
            selectedId = id;
            if (HttpContext.Request.Query.ContainsKey("sel"))
            {
                tabselected = HttpContext.Request.Query["sel"];
            }
            if (tabselected == null || tabselected == "")
            {


                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();
                    string qry = "SELECT a.articleid, a.title, a.content, a.dateCreated, a.creatorId, a.thumbnail, a.communityId, u.username FROM article a JOIN users u ON a.creatorId = u.userId WHERE a.communityid = @comid;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@comid", id);

                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Article cm = new Article();
                                cm.articleid = rdr.GetInt32(0).ToString();
                                cm.title = rdr.GetString(1);
                                cm.content = rdr.GetString(2);
                                cm.dateCreated = rdr.GetString(3);
                                cm.CreatorId = rdr.GetInt32(4);
                                cm.thumbnail = rdr.GetString(5);
                                cm.communityId = rdr.GetInt32(6);
                                cm.creatorUsername = rdr.GetString(7);
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
                                if (DateTimeOffset.TryParse(cm.dateCreated, out DateTimeOffset dateTimeOffset))
                                {
                                    DateTime normalDate = dateTimeOffset.UtcDateTime;
                                    string formattedDate = normalDate.ToString("dd/MM/yyyy HH:mm");
                                    Console.WriteLine(formattedDate); // Display or use the formatted date as needed
                                    cm.dateCreated = formattedDate;
                                }
                                articles.Add(cm);
                            }
                        }
                    }

                }
            }
            else if (tabselected == "questions")
            {

                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();
                    string qry = "SELECT a.articleid, a.title, a.content, a.dateCreated, a.creatorId, a.thumbnail, a.communityId,a.isThreadClosed, u.username FROM qa a JOIN users u ON a.creatorId = u.userId WHERE a.communityid = @comid;";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                    {
                        cmd.Parameters.AddWithValue("@comid", id);

                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Question cm = new Question();
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
                                questions.Add(cm);
                            }
                        }
                    }

                }

            }
            }
        //public async Task<IActionResult> OnGetAsync(int id)
        //{

        //    Console.WriteLine(id); // Use this extractedPath as needed

        //    return Page();
        //}
    }
    public class Article
    {
        public string? articleid { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? dateCreated { get; set; }
        public string? thumbnail { get; set; }
        public string creatorUsername { get; set; }
        // Reference to the User (assuming the User class exists)
        public int CreatorId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Creator { get; set; } // If you want to directly reference the User object
                                          // Reference to the User (assuming the User class exists)
        public int communityId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Community { get; set; } // If you want to directly reference the User object
    }
    public class Question
    {
        public string? articleid { get; set; }
        public string? title { get; set; }
        public string? content { get; set; }
        public string? dateCreated { get; set; }
        public string? thumbnail { get; set; }
        public string? creatorUsername { get; set; }
        public bool? isThreadClosed { get; set; }

        // Reference to the User (assuming the User class exists)
        public int CreatorId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Creator { get; set; } // If you want to directly reference the User object
                                          // Reference to the User (assuming the User class exists)
        public int communityId { get; set; } // This assumes the CreatorId is an int that matches the UserId in the User class
        public User Community { get; set; } // If you want to directly reference the User object
    }
}
