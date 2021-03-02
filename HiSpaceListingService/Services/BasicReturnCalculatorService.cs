using System.Linq;
using System.Collections.Generic;
using HiSpaceListingService.BO;
using HiSpaceListingService.BO.Returns;
using HiSpaceListingService.DTO;
using HiSpaceListingService.Utilities;

namespace HiSpaceListingService.Services
{
    public class BasicReturnCalculatorService : IBasicReturnCalculatorService
    {
        BasicReturnCalculatorDTO _basicReturnCalculatorDTO;
        OperatingCashFlowGenerator _operatingCashFlowGenerator;
        Next10YearNCFGenerator _next10YearNCFGenerator;
        CappedValueGenerator _cappedValueGenerator;            
        RunningDirectCapValueGenerator _runningDirectCapValueGenerator;
        RunningDCFValueGenerator _runningDCFValueGenerator;
        InvestmentCashFlowGenerator _investmentCashFlowGenerator;
        FinancingCashFlowGenerator _financingCashFlowGenerator;
        TaxCashFlowGenerator _taxCashFlowGenerator;        
        IRRReturns _IRRReturns;
        ProfitReturns _profitReturns;
        MultipleReturns _multipleReturns;
        PeakEquityReturns _peakEquityReturns;

        public void Initialize(BasicReturnCalculatorDTO basicReturnCalculatorDTO)
        {
            _basicReturnCalculatorDTO = basicReturnCalculatorDTO;
            
            _operatingCashFlowGenerator = new OperatingCashFlowGenerator();
            _next10YearNCFGenerator = new Next10YearNCFGenerator();
            _cappedValueGenerator = new CappedValueGenerator();
            _runningDirectCapValueGenerator = new RunningDirectCapValueGenerator();
            _runningDCFValueGenerator = new RunningDCFValueGenerator();
            _investmentCashFlowGenerator = new InvestmentCashFlowGenerator();
            _financingCashFlowGenerator = new FinancingCashFlowGenerator();
            _taxCashFlowGenerator = new TaxCashFlowGenerator();
            
            _IRRReturns = new IRRReturns();
            _profitReturns = new ProfitReturns();
            _multipleReturns = new MultipleReturns();
            _peakEquityReturns = new PeakEquityReturns();
        }
        
        public BasicReturnCalculatorResponse GetResponse()
        {
            decimal inflationPercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.Inflation);
            decimal loanToValuePercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.LoanToValue);
            decimal interestRatePercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.InterestRate);
            decimal exitCapRatePercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.ExitCapRate);
            decimal discountRatePercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.DiscountRate);
            decimal taxRatePercent = Converter.FromPercentToDecimal(
                                                _basicReturnCalculatorDTO.TaxRate);
            int holdingPeriodInYears = (int) _basicReturnCalculatorDTO.HoldingPeriodInYears;
            BasicReturnCalculatorResponse BRCResponse = new BasicReturnCalculatorResponse();
            
            BRCResponse.OperatingCashFlow = _operatingCashFlowGenerator.GetOperatingCashFlow(
                                                _basicReturnCalculatorDTO.EstimatedCapExOrTIThroughHold,                       
                                                inflationPercent,
                                                _basicReturnCalculatorDTO.CurrentNOI,
                                                holdingPeriodInYears,
                                                _basicReturnCalculatorDTO.AdditionalYears);  
            
            List<ValueItemResponse> next10YearNCFList = _next10YearNCFGenerator.GenerateList(
                                                    BRCResponse.OperatingCashFlow.NetCashFlowList, 
                                                        discountRatePercent, 
                                                        holdingPeriodInYears);
            
            List<ValueItemResponse> cappedValueList = _cappedValueGenerator.GenerateList(
                                                BRCResponse.OperatingCashFlow.NetOperatingIncomeList,exitCapRatePercent,
                                                discountRatePercent,
                                                10, 
                                                holdingPeriodInYears);
                                                            
            List<ValueItemResponse> runningDirectCapValueList = _runningDirectCapValueGenerator
                                                                            .GenerateList(
                                            BRCResponse.OperatingCashFlow.NetOperatingIncomeList,
                                                                            exitCapRatePercent, 
                                                                            holdingPeriodInYears);

            List<ValueItemResponse> runningDCFValueList = _runningDCFValueGenerator.GenerateList(
                                                                            next10YearNCFList, 
                                                                            cappedValueList,
                                                                    holdingPeriodInYears);

            decimal finalYearExitValue = _basicReturnCalculatorDTO.ExitMethod == "DCF" 
                                            ? runningDCFValueList.LastOrDefault().ItemValue 
                                            : runningDirectCapValueList.LastOrDefault().ItemValue;

            BRCResponse.InvestmentCashFlow = _investmentCashFlowGenerator.Generate(
                                                    _basicReturnCalculatorDTO.Investment,
                                                    finalYearExitValue,
                                                    BRCResponse.OperatingCashFlow.NetCashFlowList,
                                                    holdingPeriodInYears,
                                                _basicReturnCalculatorDTO.AdditionalYears);      

            decimal acquisitionExitValueForZerothYear = BRCResponse.InvestmentCashFlow
                                                        .AcquisitionExitValue
                                                        .FirstOrDefault().ItemValue;            
            
            BRCResponse.FinancingCashFlow = _financingCashFlowGenerator.Generate(
                                                acquisitionExitValueForZerothYear, 
                                            BRCResponse.InvestmentCashFlow.UnleveredCashFlowValue,
                                                loanToValuePercent, 
                                                interestRatePercent, 
                                                holdingPeriodInYears,
                                                _basicReturnCalculatorDTO.AdditionalYears);
            
            BRCResponse.TaxCashFlow = _taxCashFlowGenerator.Generate(
                                            BRCResponse.OperatingCashFlow.NetOperatingIncomeList,
                                            BRCResponse.FinancingCashFlow.Interest,
                                            BRCResponse.FinancingCashFlow.LeveredCashFlow,
                                            taxRatePercent,
                                            holdingPeriodInYears,
                                         _basicReturnCalculatorDTO.AdditionalYears);            
            
            BRCResponse.IRR = _IRRReturns.Generate(
                                        BRCResponse.InvestmentCashFlow.UnleveredCashFlowValue,
                                                    BRCResponse.FinancingCashFlow.LeveredCashFlow,
                                                    BRCResponse.TaxCashFlow.PostTaxCashFlow,
                                                    holdingPeriodInYears);
            
            BRCResponse.Profit = _profitReturns.Generate(
                                        BRCResponse.InvestmentCashFlow.UnleveredCashFlowValue,
                                                    BRCResponse.FinancingCashFlow.LeveredCashFlow,
                                                    BRCResponse.TaxCashFlow.PostTaxCashFlow,
                                                    holdingPeriodInYears);
            
            BRCResponse.PeakEquity = _peakEquityReturns.Generate(
                                BRCResponse.InvestmentCashFlow.UnleveredCashFlowValue[0].ItemValue,
                                BRCResponse.FinancingCashFlow.LeveredCashFlow[0].ItemValue,
                                BRCResponse.TaxCashFlow.PostTaxCashFlow[0].ItemValue);
            
            BRCResponse.Multiple = _multipleReturns.Generate(BRCResponse.Profit, 
                                                            BRCResponse.PeakEquity);            
            return BRCResponse;
        }
    }
}
