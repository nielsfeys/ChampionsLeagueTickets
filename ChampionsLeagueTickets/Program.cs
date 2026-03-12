using ChampionsLeagueTickets.Data;
using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Models;
using ChampionsLeagueTickets.Repositories;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services;
using ChampionsLeagueTickets.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//Add Automapper
builder.Services.AddAutoMapper(typeof(Program));

// DbContext voor Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// DbContext voor ChampionsLeagueTickets
builder.Services.AddDbContext<ChampionsLeagueDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IClubDAO, ClubDAO>();
builder.Services.AddScoped<IMatchDAO, MatchDAO>();
builder.Services.AddScoped<IStadiumSectionDAO, StadiumSectionDAO>();
builder.Services.AddScoped<ITicketDAO, TicketDAO>();

builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IStadiumSectionService, StadiumSectionService>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register Email Service
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddControllersWithViews();


builder.Services.AddSession(options => {
    options.Cookie.Name = "ChampionsLeagueTickets.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
