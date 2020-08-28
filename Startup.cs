using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using PTCwebApi.Interfaces;
using PTCwebApi.Models.PTCModels;
using PTCwebApi.Security;
using PTCwebApi.Security.Requirement;

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

            // services.AddControllers (opt => { /*หาก request ไหนที่ส่งมาไม่ตรงตาม policy ในที่นี้คือ JWT ไม่ผ่านจะ return 401*/
            //     var policy = new AuthorizationPolicyBuilder ().RequireAuthenticatedUser ().Build ();
            //     opt.Filters.Add (new AuthorizeFilter (policy));
            // });

            // services.Configure<ConnectionStringList>(Configuration.GetSection("ConnectString"));
            // services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor> ();
            // services.AddAuthorization (opt => {
            //     opt.AddPolicy ("UserInternal", policy => {
            //         policy.Requirements.Add (new UserInternalRequirement ());
            //     });
            //     opt.AddPolicy ("UserAdmin", policy => {
            //         policy.Requirements.Add (new UserAdminRequirement ());
            //     });
            // });
            // services.AddTransient<IAuthorizationHandler, UserInternalRequirementHandler> ();

            // var key = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (Configuration.GetSection ("AppSettings:Secret").Value));
            // services.AddAuthentication (JwtBearerDefaults.AuthenticationScheme)
            //     .AddJwtBearer (opt => {
            //         opt.TokenValidationParameters = new TokenValidationParameters {
            //         ValidateIssuerSigningKey = true,
            //         IssuerSigningKey = key,
            //         ValidateAudience = false,
            //         ValidateIssuer = false
            //         };
            //     });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors ("CorsPolicy");

            // app.UseAuthorization ();

            app.UseMiddleware<JwtMiddleware> ();

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