using Vote_Manager.DataClass;
using Vote_Manager.Pages.Voter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();



//We have added this service here and 'defaultConnection' has been set to our serverName and dataBase name to be used inside appsettings.json
//Task - Understand what exactly is the logic and meaning of code below!!
builder.Services.AddTransient<IDatabaseConnection>(e => new DatabaseConnection(builder.Configuration.GetConnectionString("defaultConnection")));



// Register the VotersInfo service
builder.Services.AddScoped<VotersInfo>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

