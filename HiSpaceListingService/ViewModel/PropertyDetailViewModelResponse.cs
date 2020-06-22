using HiSpaceListingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PropertyDetailViewModelResponse
	{
		public PropertyDetailViewModelResponse()
		{
			Amenities = new List<Amenity>();
			Facilities = new List<Facility>();
			ListingImages = new List<ListingImages>();
			REProfessionalMasters = new List<REProfessionalMaster>();
			Listing = new Listing();
			User = new User();
			WorkingHours = new WorkingHours();
			HealthCheck = new HealthCheck();
			GreenBuildingCheck = new GreenBuildingCheck();
		}
		public List<Amenity> Amenities { set; get; }
		public List<Facility> Facilities { set; get; }
		public List<ListingImages> ListingImages { set; get; }
		public List<REProfessionalMaster> REProfessionalMasters { set; get; }
		public Listing Listing { set; get; }
		public User User { set; get; }
		public WorkingHours WorkingHours { set; get; }
		public HealthCheck HealthCheck { set; get; }
		public GreenBuildingCheck GreenBuildingCheck { set; get; }
		public int ListerPropertyCount { set; get; }
	}
}
