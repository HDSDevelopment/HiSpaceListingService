using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class NetOperatingIncomeGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal currentNOI, 
                                                    List<ValueItemResponse> inflationRateList,
                                                    int holdingPeriodYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> netOperatingIncomeList = new List<ValueItemResponse>();
            
            ValueItemResponse netOperatingIncomeItem;
            int startYear = 0;
            int endYear = holdingPeriodYears + additionalYears;

            for (int forYear = startYear; forYear <= endYear; forYear++)
            {
                netOperatingIncomeItem = GetNetOperatingIncomeItem(currentNOI, inflationRateList[forYear]);
                netOperatingIncomeList.Add(netOperatingIncomeItem);
            }
            return netOperatingIncomeList;
        }

        ValueItemResponse GetNetOperatingIncomeItem(decimal currentNOI, ValueItemResponse inflationRateItem)
        {
            ValueItemResponse netOperatingIncomeItem = new ValueItemResponse();
            netOperatingIncomeItem.ForYear = inflationRateItem.ForYear;

            if(netOperatingIncomeItem.ForYear == 0)
                netOperatingIncomeItem.ItemValue = 0;

            if(netOperatingIncomeItem.ForYear == 1)
                netOperatingIncomeItem.ItemValue = currentNOI;

            if(netOperatingIncomeItem.ForYear > 1)
                netOperatingIncomeItem.ItemValue = currentNOI * inflationRateItem.ItemValue;

            return netOperatingIncomeItem;
        }
    }
}