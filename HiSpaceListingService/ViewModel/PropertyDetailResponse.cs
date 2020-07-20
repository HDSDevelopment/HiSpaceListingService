using HiSpaceListingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PropertyDetailResponse
	{
		public PropertyDetailResponse()
		{
			ListingImagesList = new List<ListingImages>();
		}
		public Listing SpaceListing { get; set; }
		public User SpaceUser { get; set; }
		public int AvailableAmenities { get; set; }
		public int AvailableFacilities { get; set; }
		public int AvailableProjects { get; set; }
		public int AvailableHealthCheck { get; set; }
		public int AvailableGreenBuildingCheck { get; set; }
		public List<ListingImages> ListingImagesList {get; set;}

	}
}
