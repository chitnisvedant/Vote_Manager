using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Vote_Manager.DataClass;

namespace Vote_Manager.Pages.Voter
{
    public class DeleteModel : PageModel
    {
        private readonly IDatabaseConnection Dbc;
        public DeleteModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }

        public async Task OnGetAsync()
        {
            //The information of id comes from the hidden input type, that receives it using @Model.votersInfo.Voter_Id in the Edit.cshtml
            Guid id = Guid.Parse(Request.Query["id"]);

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
    }
}








/* CODE WITHOUT DAPPER AND DEPENDENCY INJECTION
            Guid id = Guid.Parse(Request.Query["id"]);
            try
            {
                String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE voters SET voters.Active = 0 WHERE voters.voter_id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                       command.Parameters.AddWithValue("@id", id);
                       command.ExecuteNonQuery();
                    }
                }
            }
 */