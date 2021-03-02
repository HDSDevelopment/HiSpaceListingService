using System.Linq;
using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO.Returns
{
    public class ProfitReturns
    {
        public ReturnsItemResponse Generate(List<ValueItemResponse> unleveredCashFlowList, 
                                            List<ValueItemResponse> leveredCashFlowList,
                                            List<ValueItemResponse> postTaxCashFlowList,
                                            int holdingPeriodInYears)
        {
            ReturnsItemResponse profitResponse = new ReturnsItemResponse();
            profitResponse.Unlevered = GetProfit(unleveredCashFlowList, holdingPeriodInYears);
            profitResponse.Levered = GetProfit(leveredCashFlowList, holdingPeriodInYears);
            profitResponse.PostTax = GetProfit(postTaxCashFlowList, holdingPeriodInYears);
        return profitResponse;
        }

        decimal GetProfit(List<ValueItemResponse> cashFlowList, 
                                    int holdingPeriodInYears)
        {
            decimal sum = 0;

            if(cashFlowList != null)
            sum = (from cashFlowItem in cashFlowList
            where cashFlowItem.ForYear <= holdingPeriodInYears
            select cashFlowItem.ItemValue)
                                        .Sum();
        return sum;
        }
    }
}