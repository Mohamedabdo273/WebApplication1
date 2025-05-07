using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using Stripe;
using WebApplication1.Data;
using WebApplication1.Extentions;
using WebApplication1.Models;
using WebApplication1.Repository;
using WebApplication1.Repository.IRepository;
using WebApplication1.Services;
using WebApplication1.Services.Iservices;
using WebApplication1.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(
    option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
});

// Stripe Configuration
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Repository Services
builder.Services.AddScoped<IProduct, ProductRepository>();
builder.Services.AddScoped<ICart, CartRepository>();
builder.Services.AddScoped<IOrder, OrderRepository>();
builder.Services.AddScoped<ICategory, CategoryRepository>();
builder.Services.AddScoped<IContactUs, ContactUsRepository>();

// Business Services
builder.Services.AddScoped<IAccountService, WebApplication1.Services.AccountService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IProductService, WebApplication1.Services.ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IContactUsService, ContactUsService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// JWT Authentication
builder.Services.AddCustomJwtAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware Configuration
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ProductImages")),
    RequestPath = "/ProductImages"
});
// CORS must come before UseAuthorization
app.UseAuthentication(); // Authentication must come before Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();