using ChampionsLeagueTickets.Domain.Data;
using ChampionsLeagueTickets.Models;
using ChampionsLeagueTickets.Repositories;
using ChampionsLeagueTickets.Repositories.Interfaces;
using ChampionsLeagueTickets.Services;
using ChampionsLeagueTickets.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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


// Register DAO
builder.Services.AddScoped<IClubDAO, ClubDAO>();
builder.Services.AddScoped<IMatchDAO, MatchDAO>();
builder.Services.AddScoped<IStadiumSectionDAO, StadiumSectionDAO>();
builder.Services.AddScoped<ITicketDAO, TicketDAO>();
builder.Services.AddScoped<IOrderDAO, OrderDAO>();

// Register Database Services
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IStadiumSectionService, StadiumSectionService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register Email Services
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

// Localization Registration
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddHttpClient<IHotelService, HotelService>();

builder.Services.AddControllersWithViews().AddViewLocalization();


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

var supportedCultures = new[] {
    new CultureInfo("en"),
    new CultureInfo("nl"),
    new CultureInfo("fr")
};

app.UseRequestLocalization(new RequestLocalizationOptions {
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});


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
