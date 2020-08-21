using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PTCwebApi.Interfaces;
using PTCwebApi.Security;

namespace PTCwebApi {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            SetDatabaseConnection ();
            services.AddControllers ();
            services.AddAutoMapper (typeof (Startup));
            services.AddCors (opt => {
                opt.AddPolicy ("CorsPolicy", policy => {
                    //policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000");
                    //policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200").AllowCredentials();
                    policy.AllowAnyOrigin ().AllowAnyHeader ().AllowAnyMethod ();
                    // policy.AllowAnyHeader ().AllowAnyMethod ().WithOrigins ("http://localhost:4200")
                    //     .WithExposedHeaders ("WWW-Authenticate").AllowCredentials ();
                });
            });
            services.AddScoped<IJwtGenerator, JwtGenerator> ();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors ("CorsPolicy");

            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }
        public void SetDatabaseConnection () {
            ConnectionFactory.LapConnectionString = Configuration.GetConnectionString ("LapConnection");
            ConnectionFactory.KppConnectionString = Configuration.GetConnectionString ("KppConnection");
            ConnectionFactory.KprConnectionString = Configuration.GetConnectionString ("KprConnection");

            DataContextConfiguration.DEFAULT_DATABASE = ConnectionFactory.GetDatabaseHostByName (Configuration.GetSection ("DefaultDatabase").Value);
        }
    }
}