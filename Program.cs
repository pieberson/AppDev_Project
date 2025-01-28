var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files (CSS, JS, images)

app.UseRouting();

app.UseAuthorization();

// Redirect root-level requests to /TRACKXD/AboutUs
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/TRACKXD/AboutUs");
        return;
    }
    await next();
});

// Define custom route prefix 'TRACKXD' without the 'Home' controller prefix
app.MapControllerRoute(
    name: "default",
    pattern: "TRACKXD/{action=AboutUs}/{id?}",
    defaults: new { controller = "Home" }); // Default controller is 'Home' with AboutUs as the default action

app.Run();
