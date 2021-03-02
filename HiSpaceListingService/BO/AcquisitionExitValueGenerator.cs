using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class AcquisitionExitValueGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal investment, 
                                                    decimal finalYearExitValue,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> acquisitionExitValueList = new List<ValueItemResponse>();
            ValueItemResponse acquisitionExitValueItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;
            
            for(int forYear = startYear; forYear <= endYear; forYear++)
            {
                acquisitionExitValueItem = GetAcquisitionExitValueItem(investment, finalYearExitValue, 
                                                                    forYear, holdingPeriodInYears);
                acquisitionExitValueList.Add(acquisitionExitValueItem);    
            }
        return acquisitionExitValueList;
        }   

        ValueItemResponse GetAcquisitionExitValueItem(decimal investment, 
                                                decimal finalYearExitValue,
                                                int forYear,
                                                int holdingPeriodInYears)
        {
            ValueItemResponse acquisitionExitValueItem = new ValueItemResponse();
            acquisitionExitValueItem.ForYear = forYear;

            if(forYear == 0)
                acquisitionExitValueItem.ItemValue = (-1) * investment;
            
            if(forYear > 0 && forYear < holdingPeriodInYears)
                acquisitionExitValueItem.ItemValue = 0;

            if(forYear == holdingPeriodInYears)
                acquisitionExitValueItem.ItemValue = finalYearExitValue;

            if(forYear > holdingPeriodInYears)
                acquisitionExitValueItem.ItemValue = 0;

        return acquisitionExitValueItem;
        }
    }
}