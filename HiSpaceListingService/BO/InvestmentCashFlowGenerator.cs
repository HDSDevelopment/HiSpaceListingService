using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
public class InvestmentCashFlowGenerator
    {
        AcquisitionExitValueGenerator _acquisitionExitValueGenerator;
        UnleveredCashFlowValueGenerator _unleveredCashFlowValueGenerator;
        
        public InvestmentCashFlowGenerator()
        {
            _acquisitionExitValueGenerator = new AcquisitionExitValueGenerator();
            _unleveredCashFlowValueGenerator = new UnleveredCashFlowValueGenerator();
        }
        
        public InvestmentCashFlowResponse Generate(decimal investment, 
                                            decimal finalYearExitValue, 
                                            List<ValueItemResponse> netCashFlowList,
                                            int holdingPeriodInYears,
                                            int additionalYears)
        {
            InvestmentCashFlowResponse investmentCashFlowResponse = new InvestmentCashFlowResponse();
            investmentCashFlowResponse.AcquisitionExitValue = _acquisitionExitValueGenerator
                                                                        .GenerateList(investment, 
                                                                        finalYearExitValue, 
                                                                        holdingPeriodInYears,
                                                                        additionalYears);
            investmentCashFlowResponse.UnleveredCashFlowValue = _unleveredCashFlowValueGenerator
                                                                    .GenerateList(netCashFlowList,
                                                        investmentCashFlowResponse.AcquisitionExitValue,
                                                                    holdingPeriodInYears,
                                                                    additionalYears); 
        return investmentCashFlowResponse;
        }
    }
}