using System.Collections.Generic;

namespace HiSpaceListingService.DTO
{
    public class InvestmentCashFlowResponse
    {
        public List<ValueItemResponse> AcquisitionExitValue {get; set;}
        public List<ValueItemResponse> UnleveredCashFlowValue {get; set;}
    }
}