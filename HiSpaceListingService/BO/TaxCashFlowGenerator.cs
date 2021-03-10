using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class TaxCashFlowGenerator
    {
        TaxGenerator _taxGenerator;
        PostTaxCashFlowGenerator _postTaxCashFlowGenerator;

        public TaxCashFlowGenerator()
        {
            _taxGenerator = new TaxGenerator();
            _postTaxCashFlowGenerator = new PostTaxCashFlowGenerator();
        }
        public TaxCashFlowResponse Generate(List<ValueItemResponse> netOperatingIncomeList,
                                                    List<ValueItemResponse> interestList,
                                            List<ValueItemResponse> leveredCashFlowList,
                                            List<ValueItemResponse> leveredCashFlowList2,
                                                    decimal taxRatePercent,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            TaxCashFlowResponse taxCashFlowResponse = new TaxCashFlowResponse();
            taxCashFlowResponse.Tax = _taxGenerator.GenerateList(netOperatingIncomeList,
                                                                    interestList,
                                                                    taxRatePercent,
                                                                    holdingPeriodInYears,
                                                                    additionalYears);

            taxCashFlowResponse.PostTaxCashFlow = _postTaxCashFlowGenerator.GenerateList(
                                                                    leveredCashFlowList,
                                                                    taxCashFlowResponse.Tax,
                                                                    holdingPeriodInYears,
                                                                    additionalYears);

            taxCashFlowResponse.PostTaxCashFlow2 = _postTaxCashFlowGenerator.GenerateList(
                                                                    leveredCashFlowList2,
                                                                    taxCashFlowResponse.Tax,
                                                                    holdingPeriodInYears,
                                                                    additionalYears);
            return taxCashFlowResponse;
        }
    }
}