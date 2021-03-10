using System.Linq;
using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class FinancingCashFlowGenerator
    {
        LoanDrawdownAndRepaymentGenerator _LDRGenerator;
        InterestGenerator _interestGenerator;
        LeveredCashFlowValueGenerator _leveredCashFlowValueGenerator;
        public FinancingCashFlowGenerator()
        {
            _LDRGenerator = new LoanDrawdownAndRepaymentGenerator();
            _interestGenerator = new InterestGenerator();
            _leveredCashFlowValueGenerator = new LeveredCashFlowValueGenerator();
        }

        public FinancingCashFlowResponse Generate(decimal acquisitionExitValueForZerothYear,
                                                    List<ValueItemResponse> unleveredCashFlowList,
                                                    List<ValueItemResponse> unleveredCashFlowList2,
                                                    decimal loanToValuePercent,
                                                    decimal interestRatePercent,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            FinancingCashFlowResponse financingCashFlowResponse = new FinancingCashFlowResponse();
            financingCashFlowResponse.LoanDrawdownAndRepayment = _LDRGenerator.GenerateList(
                                                                acquisitionExitValueForZerothYear, 
                                                                    loanToValuePercent, 
                                                                    holdingPeriodInYears,
                                                                    additionalYears);
            
            financingCashFlowResponse.Interest = _interestGenerator.GenerateList(
                                        financingCashFlowResponse.LoanDrawdownAndRepayment[0].ItemValue,
                                                                interestRatePercent,
                                                                holdingPeriodInYears,
                                                                additionalYears);
            
            financingCashFlowResponse.LeveredCashFlow = _leveredCashFlowValueGenerator.GenerateList(
                                                                            unleveredCashFlowList,
                                                financingCashFlowResponse.LoanDrawdownAndRepayment,
                                                                financingCashFlowResponse.Interest,
                                                                                holdingPeriodInYears,
                                                                            additionalYears);

            List<ValueItemResponse> loanDrawdownAndRepayment2 = financingCashFlowResponse
                            .LoanDrawdownAndRepayment
                                            .Select(n => new ValueItemResponse{ForYear = n.ForYear, 
                                                                            ItemValue = n.ItemValue})
                                            .ToList();
            
            loanDrawdownAndRepayment2[holdingPeriodInYears].ItemValue = 0;

            financingCashFlowResponse.LeveredCashFlow2 = _leveredCashFlowValueGenerator.GenerateList(unleveredCashFlowList2,
            loanDrawdownAndRepayment2,
            financingCashFlowResponse.Interest,
            holdingPeriodInYears,
            additionalYears);   

        return financingCashFlowResponse;
        }
    }
}
