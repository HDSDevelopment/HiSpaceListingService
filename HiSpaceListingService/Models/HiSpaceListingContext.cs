using HiSpaceListingModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.Models
{
	public class HiSpaceListingContext : DbContext
	{
		public HiSpaceListingContext(DbContextOptions<HiSpaceListingContext> options)
		   : base(options)
		{ }

		public DbSet<User> Users { get; set; }
		public DbSet<Listing> Listings { get; set; }
		public DbSet<Amenity> Amenitys { get; set; }
		public DbSet<Facility> Facilitys { get; set; }
		public DbSet<ListingImages> ListingImagess { get; set; }
		public DbSet<REProfessionalMaster> REProfessionalMasters { get; set; }
		public DbSet<WorkingHours> WorkingHourss { get; set; }
		public DbSet<AmenityMaster> AmenityMasters { get; set; }
		public DbSet<FacilityMaster> FacilityMasters { get; set; }
		public DbSet<GreenBuildingCheck> GreenBuildingChecks { get; set; }
		public DbSet<HealthCheck> HealthChecks { get; set; }
	}
}