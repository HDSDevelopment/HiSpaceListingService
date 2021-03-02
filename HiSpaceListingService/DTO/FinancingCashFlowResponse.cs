using System.Collections.Generic;

namespace HiSpaceListingService.DTO
{
    public class FinancingCashFlowResponse
    {
        public List<ValueItemResponse> LoanDrawdownAndRepayment {get; set;}
        public List<ValueItemResponse> Interest {get; set;}
        public List<ValueItemResponse> LeveredCashFlow {get; set;}
    }
}