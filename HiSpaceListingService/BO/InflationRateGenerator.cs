using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class InflationRateGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal inflationPercent, 
                                                    int holdingPeriodInYears, 
                                                    int additionalYears)
        {
            List<ValueItemResponse> inflationRateList = new List<ValueItemResponse>();ValueItemResponse inflationRateItem;
            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;
            
            for(int forYear = startYear; forYear <= endYear; forYear++)
            {                
                inflationRateItem = GetInflationRateItem(forYear, 
                                                        inflationPercent, 
                                                        inflationRateList);
                inflationRateList.Add(inflationRateItem);
            }
            return inflationRateList;
        }

        ValueItemResponse GetInflationRateItem(int forYear, 
                                                decimal inflationPercent, 
                                                List<ValueItemResponse> inflationRateList)
        {
            ValueItemResponse inflationRateItem = new ValueItemResponse();
                inflationRateItem.ForYear = forYear;                
                inflationRateItem.ItemValue = GetInflationRate(forYear, 
                                            inflationPercent, 
                                            inflationRateList);
                return inflationRateItem;
        }

        decimal GetInflationRate(int forYear, decimal inflationPercent, List<ValueItemResponse> inflationRateList)
        {
            decimal inflationRate = 0;

            if(forYear == 0)
                inflationRate = 0;
            
            if(forYear == 1)
                inflationRate = 1;

            if(forYear > 1)
            {
                decimal previousYearInflationRate = GetPreviousYearInflationRate(forYear, inflationRateList);
                inflationRate = previousYearInflationRate * (1 + inflationPercent);
            }
        return inflationRate;
        }

        decimal GetPreviousYearInflationRate(int currentYear, List<ValueItemResponse> inflationRateList)
        {
            int previousYear = GetPreviousYear(currentYear);
            return inflationRateList[previousYear].ItemValue;
        }

        int GetPreviousYear(int currentYear)
        {            
            int previousYear = 0;                
                
                if(currentYear > 0)                
                    previousYear = currentYear - 1;

            return previousYear;                
        }
    }
}