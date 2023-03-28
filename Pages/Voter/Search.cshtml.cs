using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Vote_Manager.DataClass;

namespace Vote_Manager.Pages.Voter
{
    public class SearchModel : PageModel
    {
        public VotersInfo votersInfo = new VotersInfo();
        public String errorMessage = "";

        private readonly IDatabaseConnection Dbc;
        public SearchModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }

        public async Task OnPostAsync()
        {
            votersInfo.Voter_District = Request.Form["district_name"];
            votersInfo.Voter_State = Request.Form["state_name"];

            if (votersInfo.Voter_District.Length == 0 || votersInfo.Voter_State.Length == 0)
            {
                errorMessage = "All Fields Are Required";
                return;
            }

            //Add the data to the database;
            try
            {
                String sql = "DECLARE @district AS VARCHAR(100); DECLARE @state AS VARCHAR(100); SET @district = @district_name; SET @state = @state_name; DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state ); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state ; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; UPDATE states SET states.Active = 0; UPDATE districts SET districts.Active = 0; UPDATE states SET states.Active = 1 WHERE states.state_name = @state_name UPDATE districts SET districts.Active = 1 WHERE districts.district_name = @district_name";
                using var connection = Dbc.GetConnection();
                _ = await connection.ExecuteAsync(sql, new {district_name = votersInfo.Voter_District, state_name = votersInfo.Voter_State });
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


/* CODE WITHOUT DAPPER AND DEPENDENCY INJECTION

        public async Task OnPostAsync()
        {
            votersInfo.Voter_District = Request.Form["district_name"];
            votersInfo.Voter_State = Request.Form["state_name"];

            if (votersInfo.Voter_District.Length == 0 || votersInfo.Voter_State.Length == 0)
            {
                errorMessage = "All Fields Are Required";
                return;
            }

            //Add the data to the database;
            try
            {
                    String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        String sql = "DECLARE @district AS VARCHAR(100); DECLARE @state AS VARCHAR(100); SET @district = @district_name; SET @state = @state_name; DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state ); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state ; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; UPDATE states SET states.Active = 0; UPDATE districts SET districts.Active = 0; UPDATE states SET states.Active = 1 WHERE states.state_name = @state_name UPDATE districts SET districts.Active = 1 WHERE districts.district_name = @district_name";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@district_name", votersInfo.Voter_District);
                            command.Parameters.AddWithValue("@state_name", votersInfo.Voter_State);

                            command.ExecuteNonQuery();
                        }
                    }
            }
            catch (Exception ex)
            {
                    errorMessage = ex.Message;
                    return;
            }

            Response.Redirect("/Voter/Index");
        }

*/