using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HiSpaceListingService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace HiSpaceListingService
{
	public class WarmupHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public WarmupHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var hiSpaceContext = scope.ServiceProvider.GetRequiredService<HiSpaceListingContext>();
                var amenity = await hiSpaceContext.Amenitys.FirstOrDefaultAsync();
                var enquiry = await hiSpaceContext.Enquiries.FirstOrDefaultAsync();
                var facility = await hiSpaceContext.Facilitys.FirstOrDefaultAsync();
                var GBC = await hiSpaceContext.GreenBuildingChecks.FirstOrDefaultAsync();
                var healthCheck = await hiSpaceContext.HealthChecks.FirstOrDefaultAsync();
                //var investor = await hiSpaceContext.Investors.FirstOrDefaultAsync();
                var image = await hiSpaceContext.ListingImagess.FirstOrDefaultAsync();
                var listing = await hiSpaceContext.Listings.FirstOrDefaultAsync();
                var project = await hiSpaceContext.REProfessionalMasters.FirstOrDefaultAsync();
                var userListing = await hiSpaceContext.UserListings.FirstOrDefaultAsync();
                var user = await hiSpaceContext.Users.FirstOrDefaultAsync();
                var workHour = await hiSpaceContext.WorkingHourss.FirstOrDefaultAsync();
                var userOperator = await hiSpaceContext.UserOperators.FirstOrDefaultAsync();
            }            
        }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}