using Hotel_Booking.Constants;
using Hotel_Booking.Data;
using Hotel_Booking.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.Identity.Web;

using Microsoft.OpenApi.Models;


namespace Hotel_Booking
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
            services.AddTransient<IStorageService, StorageService>();
            // Upload File 
            services.AddAzureClients(builder => {
                builder.AddBlobServiceClient(Configuration.GetSection("Storage:ConnectionString").Value);
            });
            
            // Adding Controllers
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel_Booking", Version = "v1" });
            });

            // Graph API Config
            services.Configure<GraphAPISettings>(Configuration.GetSection("GraphAPI"));

            // Microsoft Authentication
            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

            // Database Connection
            services.AddDbContext<HotelBookingDBContext>(options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel_Booking v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Using Microsoft Authentication and Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
