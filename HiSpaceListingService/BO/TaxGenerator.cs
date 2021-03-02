using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class TaxGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> netOperatingIncomeList, 
                                                    List<ValueItemResponse> interestList,
                                                    decimal taxRatePercent,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> taxList = new List<ValueItemResponse>();
            ValueItemResponse taxItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;

            for (int forYear = startYear; forYear <= endYear; forYear++)
            {
                taxItem = GetTaxItem(netOperatingIncomeList[forYear].ItemValue,
                                        interestList[forYear].ItemValue,
                                        taxRatePercent,
                                        forYear,
                                        holdingPeriodInYears);
                taxList.Add(taxItem);
            }
        return taxList;
        }

        ValueItemResponse GetTaxItem(decimal netOperatingIncomeValue,
                                    decimal interestValue,
                                    decimal taxRatePercent,
                                    int forYear,
                                    int holdingPeriodInYears)
        {
            ValueItemResponse taxItem = new ValueItemResponse();            
            taxItem.ForYear = forYear;

            if(forYear == 0 || forYear > holdingPeriodInYears)
                taxItem.ItemValue = 0;

            if(forYear > 0 && forYear <= holdingPeriodInYears)
            taxItem.ItemValue = (-1) * taxRatePercent * (netOperatingIncomeValue + interestValue);
            
        return taxItem;
        }
    }
}