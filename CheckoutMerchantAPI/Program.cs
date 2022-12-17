using CheckoutMerchant.Infrastructure;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using FluentValidation;
using CheckoutMerchant.Models;
using CheckoutMerchant.Models.Validators;
using CheckoutMerchant.Models.Merchant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IValidator<ProcessPaymentRequest>, ProcessPaymentRequestValidator>();
builder.Services.AddScoped<IValidator<MerchantPaymentDetails>, MerchantPaymentDetailsValidator>();
builder.Services.AddScoped<IValidator<MerchantDetails>, MerchantDetailsValidator>();

builder.Services.AddTransient<IMockBank, CheckoutMerchant.Infrastructure.MockBankClient>();
builder.Services.AddTransient<IMockBankAuth, MockBankAuthClient>();
builder.Services.AddTransient<IMerchantService, MerchantService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Unauthorized/";
        options.AccessDeniedPath = "/Account/Forbidden/";
    })
    .AddJwtBearer(options =>
    {
        options.Audience = "https://localhost:7291/";
        options.Authority = "https://localhost:7240/";
        options.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "CheckoutMerchant API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
