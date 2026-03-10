using apiAutenticacao.Data;
using apiAutenticacao.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace apiAutenticacao
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // registração do serviço AuthService para injeção de dependência
            // em modo scoped, ou seja, uma nova instância será criada para cada requisição HTTP
            // imbernado.
            builder.Services.AddScoped<AuthService>();

            builder.Services.AddScoped<TokenService>();
            builder.Services.AddScoped<UserServices>();
            //Configuramos  a autenticação usando JWT Bearer).
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(options =>
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //Valida o emissor do token
                        ValidateAudience = true, //Valida o destinatário do token
                        ValidateLifetime = true, //Valida o tempo expiração do token
                        ValidateIssuerSigningKey = true, //Valida a chave de assinatura do token

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    };
                });
                    builder.Services.AddAuthorization();
                    // Add services to the container.

                    builder.Services.AddControllers();
                    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                    builder.Services.AddOpenApi();

                    builder.Services.AddDbContext<AppDbContext>(
                        options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                        );

                    var app = builder.Build();

                    // Configure the HTTP request pipeline.
                    if (app.Environment.IsDevelopment())
                    {
                        app.MapOpenApi();
                        app.MapScalarApiReference();
                    }
                    
                    app.UseHttpsRedirection();

                    app.UseAuthentication();
                    app.UseAuthorization();


                    app.MapControllers();

                    app.Run();
                

        }
    } }