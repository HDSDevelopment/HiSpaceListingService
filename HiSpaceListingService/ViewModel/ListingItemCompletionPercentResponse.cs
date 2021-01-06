using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class ListingItemCompletionPercentResponse
    {
        public Listing Listings { get; set; }
        public GreenBuildingCheck GBC { get; set; }
        public int TotalHealthCheck { get; set; }
        public int TotalGreenBuildingCheck { get; set; }
        public int TotalWorkingHours { get; set; }
        public int TotalListingImages { get; set; }
        public int TotalAmenities { get; set; }
        public int TotalFacilities { get; set; }
        public int TotalProjects { get; set; }
        public int PercentCompleted { get; set; }
    }
}