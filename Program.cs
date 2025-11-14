using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyTasks.API.Data;
using MyTasks.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyTasks API", Version = "v1" });

	// Add JWT Authentication to Swagger
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

// Configure MongoDB
builder.Services.AddSingleton<MongoDbContext>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrEmpty(jwtSecret))
{
	// Fallback secret for development
	jwtSecret = "this-is-a-very-long-secret-key-for-development-only-32-chars";
}

var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero // Remove delay of token when expire
	};

	// For debugging - you can remove this in production
	options.Events = new JwtBearerEvents
	{
		OnAuthenticationFailed = context =>
		{
			Console.WriteLine($"Authentication failed: {context.Exception.Message}");
			return Task.CompletedTask;
		},
		OnTokenValidated = context =>
		{
			Console.WriteLine("Token validated successfully");
			return Task.CompletedTask;
		}
	};
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
	app.UseCors(policy => policy
	   .WithOrigins("http://localhost:3000") // React default port
	   .AllowAnyMethod()
	   .AllowAnyHeader()
	   .AllowCredentials());

	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Make sure these are in the correct order
app.UseAuthentication(); // This must come first
app.UseAuthorization();  // This comes after

app.MapControllers();

app.Run();