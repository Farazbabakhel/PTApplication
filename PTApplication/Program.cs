using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Principal;

//using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;

using System.Text;
using PTApplication.Models.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PTApplication.Models.Global;
using PTApplication.Models.Interface;
using PTApplication.Models.Services;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ADD THIS FOR DATABASE INJECTION
builder.Services.AddDbContext<PTAppDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ReadWrite");
    options.UseSqlServer(connectionString);
});



builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<IMailService, MailService>();


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//builder.Services.AddControllers();
builder.Services.AddControllers();
    //.AddJsonOptions(options =>
//.AddNewtonsoftJson(options =>
//{
//    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
//});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["JWTIssuer"],
            ValidIssuer = builder.Configuration["JWTIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTKey"]))
        };
    });

builder.Services.AddSingleton<IPrincipal>(
provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//

builder.Services.AddAuthorization();
//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddCors(o => o.AddPolicy("PTApplication_Policy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));



var app = builder.Build();
builder.WebHost.UseUrls("http://*:8001");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("PTApplication_Policy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
