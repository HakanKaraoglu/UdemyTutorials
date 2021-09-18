using System.Text;
using DOTNET_RPG.Data;
using DOTNET_RPG.Services.CharacterService;
using DOTNET_RPG.Services.CharacterSkillService;
using DOTNET_RPG.Services.FightService;
using DOTNET_RPG.Services.WeaponService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace DOTNET_RPG
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x=>x.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));            
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<ICharacterService,CharacterService>();
            services.AddScoped<IAuthRepository,AuthRepository>();    
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(opt=>{
                        opt.TokenValidationParameters=new TokenValidationParameters{
                            ValidateIssuerSigningKey=true,
                            IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                            ValidateIssuer=false,
                            ValidateAudience=false
                        };
                    });    
            
            services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
            services.AddScoped<IWeaponService,WeaponService>();
            services.AddScoped<ICharacterSkillService,CharacterSkillService>();
            services.AddScoped<IFightService,FightService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
