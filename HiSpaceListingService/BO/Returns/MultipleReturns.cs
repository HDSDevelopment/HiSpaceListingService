using System.Linq;
using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO.Returns
{
    public class MultipleReturns
    {
        public ReturnsItemResponse Generate(ReturnsItemResponse profitReturnsResponse,
                                            ReturnsItemResponse peakEquityReturnsResponse)
        {
            ReturnsItemResponse multipleResponse = new ReturnsItemResponse();
            multipleResponse.Unlevered = GetMultiple(profitReturnsResponse.Unlevered,                                                               peakEquityReturnsResponse.Unlevered);
            multipleResponse.Levered = GetMultiple(profitReturnsResponse.Levered,                                                               peakEquityReturnsResponse.Levered);
            multipleResponse.PostTax = GetMultiple(profitReturnsResponse.PostTax,                                                               peakEquityReturnsResponse.PostTax);
        return multipleResponse;
        }

        decimal GetMultiple(decimal profitReturnsValue, decimal peakEquityReturnsValue)
        {
            decimal multipleValue = 0;
            
            if(peakEquityReturnsValue != 0)
                multipleValue = (profitReturnsValue / peakEquityReturnsValue) + 1;

        return multipleValue;
        }

    }
}