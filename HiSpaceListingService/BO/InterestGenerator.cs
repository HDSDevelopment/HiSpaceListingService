using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class InterestGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal loanDrawdownAndRepaymentForZerothYear, 
                                                    decimal interestRatePercent,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> interestList = new List<ValueItemResponse>();
            ValueItemResponse interestItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;
            decimal interestItemValue = (-1) * loanDrawdownAndRepaymentForZerothYear 
                                        * interestRatePercent;

            for(int forYear = startYear; forYear <= endYear; forYear++)
            {
                interestItem = GetInterestItem(interestItemValue, forYear, holdingPeriodInYears);
                interestList.Add(interestItem);
            }
        return interestList;
        }

        ValueItemResponse GetInterestItem(decimal interestItemValue, 
                                            int forYear, 
                                            int holdingPeriodInYears)
        {
            ValueItemResponse interestItem = new ValueItemResponse();
            interestItem.ForYear = forYear;

            if(forYear == 0 || forYear > holdingPeriodInYears)
                interestItem.ItemValue = 0;

            if(forYear > 0 && forYear <= holdingPeriodInYears)
                interestItem.ItemValue = interestItemValue;
                
        return interestItem;
        }
    }
}