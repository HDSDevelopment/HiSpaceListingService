using System.Collections.Generic;

namespace HiSpaceListingService.ViewModel
{
    public class ListingCompletionPercentResponse
    {
    public List<ListingItemCompletionPercentResponse> ListingsWithCompletionPercent { get; set; }

        public int OverallPercentCompleted { get; set; }
    }
}
