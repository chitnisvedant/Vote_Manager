using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Vote_Manager.DataClass;
using Vote_Manager.Pages.Voter;

namespace Vote_Manager.Pages.Voter
{
    public class CreateModel : PageModel
    {
        public VotersInfo votersInfo = new VotersInfo();
        public String errorMessage = "";


        private readonly IDatabaseConnection Dbc;
        public CreateModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }

        public async Task OnPostAsync() 
        {
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
                    String sql = "DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; INSERT INTO voters(voter_id, voter_name, district_id) VALUES (newID(), @name, @district_id_sqlVariable)"; //Our motive is to fill this string with sql query to update our database with the name, district and state entered.
                    using var connection = Dbc.GetConnection();
                    _ = await connection.ExecuteAsync(sql, new { name = votersInfo.Voter_Name, district = votersInfo.Voter_District, state = votersInfo.Voter_State });
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Voter/Index");
        }
    }
}


/* CODE WITHOUT DAPPER AND DEPENDENCY INJECTION
try
{
    String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
    using (SqlConnection connection= new SqlConnection(connectionString))
    {
        connection.Open();
        String sql = "DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; INSERT INTO voters(voter_id, voter_name, district_id) VALUES (newID(), @name, @district_id_sqlVariable)"; //Our motive is to fill this string with sql query to update our database with the name, district and state entered.

        using (SqlCommand command = new SqlCommand(sql, connection))
        {
            command.Parameters.AddWithValue("@name", votersInfo.Voter_Name);
            command.Parameters.AddWithValue("@district", votersInfo.Voter_District);
            command.Parameters.AddWithValue("@state", votersInfo.Voter_State);

            command.ExecuteNonQuery();
        }
    }
}
*/