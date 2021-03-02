using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class UnleveredCashFlowValueGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> netCashFlowList, 
                                                    List<ValueItemResponse> acquisitionExitValueList,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> UnleveredCashFlowValueList = new List<ValueItemResponse>();
            ValueItemResponse UnleveredCashFlowValueItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;
            
            for(int forYear = startYear; forYear <= endYear; forYear++)
            {
                UnleveredCashFlowValueItem = GetUnleveredCashFlowValueItem(
                                                                netCashFlowList[forYear].ItemValue, 
                                                        acquisitionExitValueList[forYear].ItemValue, 
                                                                    forYear, holdingPeriodInYears);
                UnleveredCashFlowValueList.Add(UnleveredCashFlowValueItem);    
            }
        return UnleveredCashFlowValueList;
        }

        ValueItemResponse GetUnleveredCashFlowValueItem(decimal netCashFlowValue, 
                                                decimal acquisitionExitValue,
                                                int forYear,
                                                int holdingPeriodInYears)
        {
            ValueItemResponse UnleveredCashFlowValueItem = new ValueItemResponse();
            UnleveredCashFlowValueItem.ForYear = forYear;
            
            UnleveredCashFlowValueItem.ItemValue = forYear <= holdingPeriodInYears ?
                                                    netCashFlowValue + acquisitionExitValue : 0;
        return UnleveredCashFlowValueItem;
        }
    }
}