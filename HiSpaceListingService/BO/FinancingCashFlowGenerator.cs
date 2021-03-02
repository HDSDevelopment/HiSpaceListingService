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
        return financingCashFlowResponse;
        }
    }
}
