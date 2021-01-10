using System.Linq;

namespace HiSpaceListingService.ViewModel
{
    public class PropertySearchCriteria
    {
        public int Id { get; set; }
        public string ListingType { get; set; }

        public string CMCW_PropertyFor { get; set; }

        public string CommercialType { get; set; }

        public string CoworkingType { get; set; }

        public string Locality { get; set; }

        public int? PriceMin { get; set; }

        public int? PriceMax { get; set; }

        public bool? IsPerformGBC { get; set; }

        public bool? IsPerformHealthCheck { get; set; }

        public bool? IsPerformHour { get; set; }

        public bool? IsPerformDay { get; set; }

        public bool? IsPerformMonth { get; set; }

        public bool IsValidCMCW_PropertyFor()
        {
        return CMCW_PropertyFor == "Rental" || CMCW_PropertyFor == "Sale" ? true : false;    
        }

        public bool IsValidListingType()
        {
        return ListingType=="Commercial" || ListingType == "Co-Working" ? true : false;
        }

        public bool IsValidCommercialType()
        {    
            return !string.IsNullOrEmpty(CommercialType);
        }

        public bool IsValidCoworkingType()
        {    
            return !string.IsNullOrEmpty(CoworkingType);
        }

        public bool IsValidPriceMin()
        {    
            return PriceMin != null;
        }

        public bool IsValidPriceMax()
        {    
            return PriceMax != null;
        }
        public bool IsValidPerformGBC()
        {
	        return IsPerformGBC != null && IsPerformGBC == true;
        }

        public bool IsValidPerformHealthCheck()
        {
	        return IsPerformHealthCheck != null && IsPerformHealthCheck == true;
        }
        public bool IsValidHour()
        {
            return IsPerformHour != null && IsPerformHour == true;
        }
        public bool IsValidDay()
        {
            return IsPerformDay != null && IsPerformDay == true;
        }
        public bool IsValidMonth()
        {
            return IsPerformMonth != null && IsPerformMonth == true;
        }
    }
}