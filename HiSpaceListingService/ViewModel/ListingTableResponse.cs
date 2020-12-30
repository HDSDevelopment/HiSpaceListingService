using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class ListingTableResponse
    {
        public ListingTableResponse()
        {
            Listings = new Listing();
        }

        public Listing Listings { get; set; }
        public GreenBuildingCheck GBC { get; set; }
        public int TotalHealthCheck { get; set; }
        public int TotalGreenBuildingCheck { get; set; }
        public int TotalWorkingHours { get; set; }
        public int TotalListingImages { get; set; }
        public int TotalAmenities { get; set; }
        public int TotalFacilities { get; set; }
        public int TotalProjects { get; set; }
        public int ListProgress { get; set; } = 63;
    }
}
