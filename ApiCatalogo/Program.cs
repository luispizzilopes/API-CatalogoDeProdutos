using ApiCatalogo.Context;
using ApiCatalogo.DTOs.Mappings;
using ApiCatalogo.Filters;
using ApiCatalogo.Repository.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ApiCatalogo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //Adicionando o servi�o do Unit of Work 
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Adicionando o servi�o do Filtro personalizado
            builder.Services.AddScoped<ApiLogginFilter>();

            //Adicionando o servi�o do AutoMapper
            var mappingConfig = new MapperConfiguration(mp =>
            {
                mp.AddProfile(new MappingProfile()); 
            });

            IMapper mapper = mappingConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);


            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); 

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            //Inclus�o do servi�o do contexto do EF
            //Defininfo a string de conex�o com o banco de dados
            string mySqlConnetion = builder.Configuration.GetConnectionString("DefaultConnetion");
            //Registro do contexto da EF
            builder.Services.AddDbContext<AppDbContext>(options =>
              options.UseMySql(mySqlConnetion, ServerVersion.AutoDetect(mySqlConnetion)));

            //Adicionando o servi�o de Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}