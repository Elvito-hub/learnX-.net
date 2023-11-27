using final2.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages;

public class IndexModel : PageModel
{
    public List<Community> communities = new List<Community>();
    string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";


    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

        using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
        {
            conn.Open();
            string qry = "SELECT communityid, name,datecreated,isapproved,description,profilepic from community where isapproved=true";
            using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
            {

                using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Community cm = new Community();
                        cm.communityid = rdr.GetInt32(0).ToString();
                        cm.name = rdr.GetString(1);
                        cm.datecreated = rdr.GetString(2);
                        cm.isapproved = rdr.GetBoolean(3);
                        cm.description = rdr.GetString(4);
                        cm.CoverPhotoPath = rdr.GetString(5);
                        int index = cm.CoverPhotoPath.IndexOf("communityCover");

                        // Check if "communityCover" is found in the string
                        if (index != -1)
                        {
                            // Extract the substring after "communityCover" (inclusive)
                            string extractedPath = cm.CoverPhotoPath.Substring(index + "communityCover".Length);

                            // Add a leading '/' if needed
                            if (extractedPath.StartsWith("/"))
                            {
                                extractedPath = extractedPath.Substring(1); // Remove the leading '/'
                            }

                            // Now, extractedPath will contain the string after "communityCover"
                            Console.WriteLine(extractedPath); // Use this extractedPath as needed
                            cm.CoverPhotoPath = "/communityCover/" + extractedPath;
                        }
                        communities.Add(cm);
                    }
                }
            }
            conn.Close();
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

