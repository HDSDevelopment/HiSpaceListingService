using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class BasicInfoCompletionResponse
    {
        public User User {get; set;}

        public int PercentageCompleted { get; set; }
    }
}