using System.Collections.Generic;

namespace HiSpaceListingService.DTO
{
    public class TaxCashFlowResponse
    {
        public List<ValueItemResponse> Tax { get; set; }
        public List<ValueItemResponse> PostTaxCashFlow { get; set; }
        public List<ValueItemResponse> PostTaxCashFlow2 { get; set; }
    }
}