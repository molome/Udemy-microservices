using Duende.IdentityServer.Services;
using Mango.Services.Identity;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Initailizer;
using Mango.Services.Identity.Models;
using Mango.Services.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();



var builder_C = builder.Services.AddIdentityServer(option =>
{
    option.Events.RaiseErrorEvents = true;
    option.Events.RaiseInformationEvents = true;
    option.Events.RaiseFailureEvents = true;
    option.Events.RaiseSuccessEvents = true;
    option.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(SD.identityResources)
.AddInMemoryApiScopes(SD.apiScopes)
.AddInMemoryClients(SD.clients)
.AddAspNetIdentity<ApplicationUser>();

builder.Services.AddScoped<IDbInitailizer, DbInitailizer>();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder_C.AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthorization();

//IDbInitailizer dbInitailizer = app.Services.GetRequiredService<IDbInitailizer>();
//dbInitailizer.Initailize();

using (var serviceScope = app.Services.CreateScope())
{
    var service = serviceScope.ServiceProvider.GetService<IDbInitailizer>();
    service.Initailize();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
