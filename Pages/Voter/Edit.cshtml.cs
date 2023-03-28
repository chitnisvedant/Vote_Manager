using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Vote_Manager.DataClass;

namespace Vote_Manager.Pages.Voter
{
    public class EditModel : PageModel
    {
        public VotersInfo votersInfo = new VotersInfo();
        public String errorMessage = "";
        public String successMessage = "";

        private readonly IDatabaseConnection Dbc;
        public EditModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }

        public async Task OnGet()
        {
            Guid id = Guid.Parse(Request.Query["id"]);

            try
            {
                string sql = "SELECT voters.voter_id, voters.voter_name, districts.district_name Voter_District, states.state_name Voter_State FROM voters JOIN districts ON voters.district_id = districts.district_id JOIN states ON districts.state_id = states.state_id WHERE voters.voter_id = @id";
                using var connection = Dbc.GetConnection();
                votersInfo = (VotersInfo)await connection.QueryFirstOrDefaultAsync<VotersInfo>(sql, new { id = id });
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }
        }

        public async Task OnPostAsync()
        {
            //The information of id comes from the hidden input type, that receives it using @Model.votersInfo.Voter_Id in the Edit.cshtml
            votersInfo.Voter_Id = Guid.Parse(Request.Query["id"]);

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
                    _ = await connection.ExecuteAsync(sql, new { name = votersInfo.Voter_Name, district = votersInfo.Voter_District, state = votersInfo.Voter_State, id = votersInfo.Voter_Id});
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

/*CODE WITHOUT DAPPER AND DEPENDENCY INJECTION FOR ONGET -
 
                String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT voters.voter_id, voters.voter_name, districts.district_name Voter_District, states.state_name Voter_State FROM voters JOIN districts ON voters.district_id = districts.district_id JOIN states ON districts.state_id = states.state_id WHERE voters.voter_id = @id";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                votersInfo.Voter_Id = reader.GetGuid(0);
                                votersInfo.Voter_Name = reader.GetString(1);
                                votersInfo.Voter_District = reader.GetString(2);
                                votersInfo.Voter_State = reader.GetString(3);
                            }
                        }
                    }
                }
 */


/*CODE WITHOUT DAPPER AND DEPENDENCY INJECTION FOR ONPOST -

        public async Task OnPostAsync()
        {
            //The information of id comes from the hidden input type, that receives it using @Model.votersInfo.Voter_Id in the Edit.cshtml
            votersInfo.Voter_Id = Guid.Parse(Request.Query["id"]);

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
                String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                        connection.Open();
                        String sql = "DECLARE @state_id_sqlVariable AS UNIQUEIDENTIFIER; DECLARE @district_id_sqlVariable AS UNIQUEIDENTIFIER; INSERT INTO states (state_id, state_name) SELECT newID(), @state WHERE NOT EXISTS(SELECT 1 FROM states WHERE states.state_name = @state); SELECT @state_id_sqlVariable = states.state_id FROM states WHERE states.state_name = @state; INSERT INTO districts(district_id, district_name, state_id) SELECT newID(), @district, @state_id_sqlVariable WHERE NOT EXISTS(SELECT 1 FROM districts WHERE districts.district_name = @district); SELECT @district_id_sqlVariable = districts.district_id FROM districts WHERE districts.district_name = @district; UPDATE voters SET voter_name = @name, district_id = @district_id_sqlVariable WHERE voter_id = @id;"; //Our motive is to fill this string with sql query to update our database with the name, district and state entered.
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            //The code below assigns the value to @name, @districts, etc, using votersInfo.Voter_Name, votersInfo.Voter_District, etc, that has been received from the from.
                            command.Parameters.AddWithValue("@name", votersInfo.Voter_Name);
                            command.Parameters.AddWithValue("@district", votersInfo.Voter_District);
                            command.Parameters.AddWithValue("@state", votersInfo.Voter_State);


                            command.Parameters.AddWithValue("@id", votersInfo.Voter_Id);

                            //Once we assign the values to @name, @districts, etc, we execute the sql query written.
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