using System.Collections.Generic;

namespace HiSpaceListingService.DTO
{
    public class BasicReturnCalculatorResponse
    {
        public OperatingCashFlowResponse OperatingCashFlow { get; set; }

        public InvestmentCashFlowResponse InvestmentCashFlow {get; set;} 

        public FinancingCashFlowResponse FinancingCashFlow {get; set;}

        public TaxCashFlowResponse TaxCashFlow {get; set;}

        public ReturnsItemResponse IRR {get; set;}

        public ReturnsItemResponse Profit {get; set;}

        public ReturnsItemResponse Multiple {get; set;}

        public ReturnsItemResponse PeakEquity {get; set;}
    }
}
