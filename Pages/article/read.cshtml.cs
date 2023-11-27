using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final2.Pages.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Npgsql;

namespace final2.Pages.article
{
	public class readModel : PageModel
    {
        string connstring = "Host=localhost;Port=5432;Username=elvisgasana;Database=finalasp;";

        public Article cm = new Article();
        public void OnGet(int id)
        {

            Console.WriteLine(id+"id of article"); // Use this extractedPath as needed

            using (NpgsqlConnection conn = new NpgsqlConnection(connstring))
            {
                conn.Open();
                string qry = "SELECT a.articleid, a.title, a.content, a.dateCreated, a.creatorId, a.thumbnail, a.communityId, u.username FROM article a JOIN users u ON a.creatorId = u.userId WHERE a.articleid = @comid;";
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
                            cm.thumbnail = rdr.GetString(5);
                            cm.communityId = rdr.GetInt32(6);
                            cm.creatorUsername = rdr.GetString(7);
                            int index = cm.thumbnail.IndexOf("communityCover");
                            Console.WriteLine(cm.content); // Display or use the formatted date as needed



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
                        }
                    }
                }

            }
        }
    }
}
