using HiSpaceListingService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
//using StackExchange.Profiling;
using System.Threading.Tasks;

namespace HiSpaceListingService
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


			//Miniprofiler start
			//services.AddMemoryCache();
			//services.AddMiniProfiler(options => options.RouteBasePath = "/profiler").AddEntityFramework();
			//Miniprofiler end	

			//services.AddDbContext<HiSpaceListingContext>(opt => opt.UseSqlServer(Configuration["ConnectionString:HiSpaceListingDB"]));

			services.AddDbContextPool<HiSpaceListingContext>(opt => opt.UseSqlServer(Configuration["ConnectionString:HiSpaceListingDB"]));

			Task.Run(() =>
			{
				var optionsBuilder = new DbContextOptionsBuilder<HiSpaceListingContext>();
				optionsBuilder.UseSqlServer(Configuration["ConnectionString:HiSpaceListingDB"]);

				using (HiSpaceListingContext dbContext = new HiSpaceListingContext(optionsBuilder.Options))
				{
					var model = dbContext.Model;
				}
			});

			//services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

			// Register the Swagger generator, defining 1 or more Swagger documents
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});

			services.AddControllers();

			//services.AddCors(CorsHandler);	
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			//Miniprofiler start
			//app.UseMiniProfiler();
			//Miniprofiler end

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();

			app.UseRouting();

			//app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			app.UseSwagger();

			// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
			// specifying the Swagger JSON endpoint.
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});
		}
	}

	//}
}