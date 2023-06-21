using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shooping.Helpers;
using Shopping.Data;
using Shopping.Data.Entities;
using Shopping.Helpers;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder .Services.AddDbContext<DataContext>(o=>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
}
);
//todo: Make strongest password
builder.Services.AddIdentity<User, IdentityRole>(cfg =>
{
    cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    cfg.SignIn.RequireConfirmedEmail = true;

    cfg.User.RequireUniqueEmail = true;
    cfg.Password.RequireDigit = false;
    cfg.Password.RequiredUniqueChars = 0;
    cfg.Password.RequireLowercase = false;
    cfg.Password.RequireNonAlphanumeric = false;
    cfg.Password.RequireUppercase = false;
    cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    cfg.Lockout.MaxFailedAccessAttempts = 3;
    cfg.Lockout.AllowedForNewUsers = true;

    // cfg.Password.RequiredLength = 10;
})
      .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Account/NotAuthorized";
	options.AccessDeniedPath = "/Account/NotAuthorized";
});
builder.Services.AddFlashMessage();
builder.Services.AddTransient<SeedDb>();//se usa una sola vez y se destruye
builder.Services.AddScoped<IUserHelper, UserHelper>();
builder.Services.AddScoped<ICombosHelper, CombosHelper>();
builder.Services.AddScoped<IBlobHelper, BlobHelper>();
builder.Services.AddScoped<IMailHelper, MailHelper>();
builder.Services.AddScoped<IOrdersHelper, OrdersHelper>();
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
//builder.Services.AddScoped <SeedDb>(); //la mayoria usa esta se usa varias veces y se destruye al final
//builder.Services.AddSingleton<SeedDb>();//se usa una sola vez pero no se destruye
var app = builder.Build();
SeedData();

void SeedData()
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
        service.SeedAsync().Wait();
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
