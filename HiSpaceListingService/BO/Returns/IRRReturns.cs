using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using HiSpaceListingService.DTO;
using HiSpaceListingService.Utilities;

namespace HiSpaceListingService.BO.Returns
{
    public class IRRReturns
    {
        public ReturnsItemResponse Generate(List<ValueItemResponse> unleveredCashFlowList, 
                                            List<ValueItemResponse> leveredCashFlowList,
                                            List<ValueItemResponse> postTaxCashFlowList,
                                            int holdingPeriodInYears)
        {
            ReturnsItemResponse returnsItemResponse = new ReturnsItemResponse();
            returnsItemResponse.Unlevered = GetIRR(unleveredCashFlowList, holdingPeriodInYears);
            returnsItemResponse.Levered = GetIRR(leveredCashFlowList, holdingPeriodInYears);
            returnsItemResponse.PostTax = GetIRR(postTaxCashFlowList, holdingPeriodInYears);
        return returnsItemResponse;
        }

        decimal GetIRR(List<ValueItemResponse> cashFlowList, 
                                    int holdingPeriodInYears)
        {
            double[] unleveredCashFlowListInDouble = Converter.ToDoubleArray(cashFlowList,                                                                         holdingPeriodInYears);
            decimal IRR = (decimal) Financial.IRR(ref unleveredCashFlowListInDouble);
            decimal IRRPercent = IRR * 100;
        return IRRPercent;
        }
    }
}