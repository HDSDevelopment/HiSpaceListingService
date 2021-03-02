using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class LeveredCashFlowValueGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> unleveredCashFlowList, 
                                                List<ValueItemResponse> loanDrawndownAndRepaymentList,
                                                List<ValueItemResponse> interestList,
                                                int holdingPeriodInYears,
                                                int additionalYears)
        {
            List<ValueItemResponse> leveredCashFlowValueList = new List<ValueItemResponse>();
            ValueItemResponse leveredCashFlowValueItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;

            for (int forYear = startYear; forYear <= endYear; forYear++)
            {
                leveredCashFlowValueItem = GetLeveredCashFlowValueItem(
                                                    unleveredCashFlowList[forYear].ItemValue,
                                                    loanDrawndownAndRepaymentList[forYear].ItemValue,
                                                    interestList[forYear].ItemValue,
                                                    forYear,
                                                    holdingPeriodInYears);
                leveredCashFlowValueList.Add(leveredCashFlowValueItem);
            }            
        return leveredCashFlowValueList;
        }

        ValueItemResponse GetLeveredCashFlowValueItem(decimal unleveredCashValue, 
                                                            decimal loanDrawndownAndRepaymentValue,
                                                            decimal interestValue,
                                                            int forYear,
                                                            int holdingPeriodInYears)
        {
            ValueItemResponse leveredCashFlowValueItem = new ValueItemResponse();
            leveredCashFlowValueItem.ForYear = forYear;
            
            leveredCashFlowValueItem.ItemValue = unleveredCashValue + 
                                                loanDrawndownAndRepaymentValue + 
                                                interestValue;            
        return leveredCashFlowValueItem;
        }
    }
}