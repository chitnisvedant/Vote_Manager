using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using Vote_Manager.DataClass;

namespace Vote_Manager.Pages.Voter
{
    public class ShowAllModel : PageModel
    {
        private readonly IDatabaseConnection Dbc;
        public ShowAllModel(IDatabaseConnection dbConnection)
        {
            Dbc = dbConnection;
        }

        public async Task OnGetAsync()
        {
            try
            {
                string sql = "UPDATE states SET states.Active = 1 UPDATE districts SET districts.Active = 1";
                using var connection = Dbc.GetConnection();
                _ = await connection.ExecuteAsync(sql);
            }

            catch (Exception ex)
            {
                return;
            }

            Response.Redirect("/Voter/Index");

        }
    }
}




/*CODE WITHOUT USING DAPPER AND DEPENDENCY INJECTION

        public async Task OnGetAsync()
        {
            try
            {
                String connectionString = "Data Source=LAPTOP-BH4KDSCJ;Initial Catalog=\"Voters Database\";Integrated Security=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "UPDATE states SET states.Active = 1 UPDATE districts SET districts.Active = 1";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                return;
            }

            Response.Redirect("/Voter/Index");

        }
 
*/