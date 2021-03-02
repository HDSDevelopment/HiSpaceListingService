using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class OperatingCashFlowGenerator
    {
        InflationRateGenerator _inflationRateGenerator;
        NetOperatingIncomeGenerator _netOperatingIncomeGenerator;
        CapitalExpenseGenerator _capitalExpenseGenerator;
        NetCashFlowGenerator _netCashFlowGenerator;
        OperatingCashFlowResponse operatingCashFlowResponse;
        
    public OperatingCashFlowGenerator()
    {
        operatingCashFlowResponse = new OperatingCashFlowResponse();
        _inflationRateGenerator = new InflationRateGenerator();
        _netOperatingIncomeGenerator = new NetOperatingIncomeGenerator();
        _capitalExpenseGenerator = new CapitalExpenseGenerator();
        _netCashFlowGenerator = new NetCashFlowGenerator();
    }
    public OperatingCashFlowResponse GetOperatingCashFlow(decimal estimatedCapExOrThroughHold,
                                                        decimal inflationPercent,
                                                        decimal currentNOI,
                                                        int holdingPeriodInYears,
                                                        int additionalYears)
    {    
     operatingCashFlowResponse.InflationRateList = _inflationRateGenerator.GenerateList(
                                                                                inflationPercent,
                                                                                holdingPeriodInYears,
                                                                                additionalYears);

        operatingCashFlowResponse.NetOperatingIncomeList = _netOperatingIncomeGenerator.GenerateList(
                                                                currentNOI,
                                                        operatingCashFlowResponse.InflationRateList,
                                                                holdingPeriodInYears,
                                                                additionalYears);

        operatingCashFlowResponse.CapitalExpenseList = _capitalExpenseGenerator.GenerateList(
                                                                estimatedCapExOrThroughHold,
                                                                holdingPeriodInYears,
                                                                additionalYears);

        operatingCashFlowResponse.NetCashFlowList = _netCashFlowGenerator.GenerateList(
                                                            operatingCashFlowResponse.NetOperatingIncomeList,
                                                            operatingCashFlowResponse.CapitalExpenseList,
                                                            holdingPeriodInYears,
                                                            additionalYears); 
        return operatingCashFlowResponse;
        }
    }
}
