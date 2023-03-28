using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Xml;
using Vote_Manager.DataClass;

namespace Vote_Manager.Pages.Voter 
{
    //Create a class votersInfo, that stores the data of 1 Voter to be displayed on the page.
    public class VotersInfo
    {
        //We are injecting it from the Program.cs where we have stored the services, and in appsettings.json, we have stored our connectionString...
        public IDatabaseConnection Dbc { get; set; }

        public Guid Voter_Id { get; set; }
        public string Voter_Name { get; set; }
        public string Voter_District { get; set; }
        public string Voter_State { get; set; }
    }

    public class IndexModel : PageModel
    {
        //Created a list (Array), based on the class votersInfo, to store data of multiple voters.
        public List<VotersInfo> listVoters = new List<VotersInfo>();

        public VotersInfo votersInfo = new VotersInfo();
        public String errorMessage = "";
        public String successMessage = "";

        private readonly IDatabaseConnection Dbc;
        public IndexModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }
        
        public async Task OnGetAsync()
        { 
            try
            {
                    string sql = @"SELECT voters.voter_id, voters.voter_name, districts.district_name Voter_District, states.state_name Voter_State FROM voters 
                        JOIN districts ON voters.district_id = districts.district_id 
                        JOIN states ON districts.state_id = states.state_id
                        WHERE voters.Active = 1 AND states.Active = 1 AND districts.Active = 1";

                    using var connection = Dbc.GetConnection();
                    listVoters = ((List<VotersInfo>)await connection.QueryAsync<VotersInfo>(sql));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
        }

        //Using OnPostDelete to apply onPost on the submitted delete form.
        //Name changed from OnPost to OnPostDelete using asp-page-handler="Delete" attribute of form tag.
        public async Task OnPostDelete()
        {
            //The information of id comes from the hidden input type, that receives it using @Model.votersInfo.Voter_Id in the Edit.cshtml
            Guid id = Guid.Parse(Request.Form["id"]);

            try
            {
                string sql = "UPDATE voters SET voters.Active = 0 WHERE voters.voter_id = @id";
                using var connection = Dbc.GetConnection();
                _ = await connection.ExecuteAsync(sql, new { id });
            }
            catch (Exception ex)
            {

            }

            Response.Redirect("/Voter/Index");
        }

        //The OnPost below is to read the form filled for Edit portion.
        public async Task OnPostAsync()
        {
            //The information of id comes from the hidden input type, that receives it using @Model.votersInfo.Voter_Id in the Edit.cshtml
            votersInfo.Voter_Id = Guid.Parse(Request.Form["id"]);

            votersInfo.Voter_Name = Request.Form["name"];
            votersInfo.Voter_District = Request.Form["district"];
            votersInfo.Voter_State = Request.Form["state"];

            if (votersInfo.Voter_Name.Length == 0 || votersInfo.Voter_District.Length == 0 || votersInfo.Voter_State.Length == 0)
            {
                errorMessage = "All Fields Are Required";
                return;
            }

            //Add the data to the database;
            try
            {
                String sql = "DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; UPDATE voters SET voter_name = @name, district_id = @district_id_sqlVariable WHERE voter_id = @id;"; //Our motive is to fill this string with sql query to update our database with the name, district and state entered.
                using var connection = Dbc.GetConnection();
                //Dapper stuff to update
                _ = await connection.ExecuteAsync(sql, new { name = votersInfo.Voter_Name, district = votersInfo.Voter_District, state = votersInfo.Voter_State, id = votersInfo.Voter_Id });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Voter/Index");
        }
    }
}

/* WITHOUT DAPPER AND DEPENDENCY INJECTION
                String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                { 
                    connection.Open();

                    //In front of districts.district_name, we need to write the Voter_District, so that it allows it to read that districts.district_name is placed in for Voter_District.
                    //And similarly states.state_name Voter_State is written that makes sure that Voter_State has to contain states.state_name;

                    string sql = @"SELECT voters.voter_id, voters.voter_name, districts.district_name Voter_District, states.state_name Voter_State FROM voters 
                        JOIN districts ON voters.district_id = districts.district_id 
                        JOIN states ON districts.state_id = states.state_id
                        WHERE voters.Active = 1 AND states.Active = 1 AND districts.Active = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader()) 
                        {
                            while(reader.Read())
                            {
                                VotersInfo votersInfo = new VotersInfo();

                                votersInfo.Voter_Id = reader.GetGuid(0);
                                votersInfo.Voter_Name = reader.GetString(1);
                                votersInfo.Voter_District = reader.GetString(2);
                                votersInfo.Voter_State = reader.GetString(3);

                                listVoters.Add(votersInfo);
                            }
                        }
                    }
                }
*/ 