using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO.Returns
{
    public class PeakEquityReturns
    {
        public ReturnsItemResponse Generate(decimal unleveredCashFlowForZerothYear,
                                            decimal leveredCashFlowForZerothYear,
                                            decimal postTaxCashFlowZerothYear)
        {
            ReturnsItemResponse peakEquityResponse = new ReturnsItemResponse();
            
            peakEquityResponse.Unlevered = GetPeakEquity(unleveredCashFlowForZerothYear);
            peakEquityResponse.Levered = GetPeakEquity(leveredCashFlowForZerothYear);
            peakEquityResponse.PostTax = GetPeakEquity(postTaxCashFlowZerothYear);
        return peakEquityResponse;
        }

        decimal GetPeakEquity(decimal item)
        {
            return (-1) * item;
        }
    }
}