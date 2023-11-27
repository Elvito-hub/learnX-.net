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
using static System.Collections.Specialized.BitVector32;

namespace final2.Pages.Admin
{
    [Authorize(Policy = "AdminOnly")] // This page will be accessible by only Admin
    public class DashboardModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";
        public List<User> facultyList = new List<User>();
        public List<Community> communities = new List<Community>();

        public string tabselected = "";
        public void OnGet()
        {
            if (HttpContext.Request.Query.ContainsKey("sel"))
            {
                tabselected = HttpContext.Request.Query["sel"];
            }
            System.Console.WriteLine(tabselected);
            if (tabselected == null || tabselected == "")
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();
                    string qry = "SELECT email, username,phonenumber from users";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                    {
                        using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                User faculty = new User();
                                faculty.email = rdr.GetString(0);
                                faculty.userName = rdr.GetString(1);
                                faculty.phoneNumber = rdr.GetString(2);

                                facultyList.Add(faculty);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            else if (tabselected == "communities")
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
                {
                    conn.Open();
                    string qry = "SELECT communityid, name,datecreated,isapproved,description from community";
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
                                communities.Add(cm);
                            }
                        }
                    }

                }
            }
        }
        public async Task<IActionResult> OnPostApproveCommunityAsync(int communityId)
        {
            string cmid = Request.Form["communityId"];
            using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
            {
                conn.Open();
                string qry = "update community set isapproved=true where communityid=@commd";
                using (NpgsqlCommand cmd = new NpgsqlCommand(qry, conn))
                {
                    cmd.Parameters.AddWithValue("@commd",int.Parse(cmid));
                    int rowsAffected = cmd.ExecuteNonQuery();

                }
                conn.Close();
            }
            // Your logic to update the isapproved column in the database
            // Example:
            // await _communityService.ApproveCommunity(communityId);

            // After updating the database, you might want to refresh the page or perform other actions
            return RedirectToPage("/Admin/Dashboard"); // Redirect to the dashboard page after the update
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
    }
}
