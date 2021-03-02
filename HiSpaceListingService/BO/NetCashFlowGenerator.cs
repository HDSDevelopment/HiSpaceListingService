using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class NetCashFlowGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> netOperatingIncomeList,
                                                    List<ValueItemResponse> capitalExpenseList,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> netCashFlowList = new List<ValueItemResponse>();
            ValueItemResponse netCashFlowItem;
            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;
            
            for(int forYear = startYear; forYear <= endYear; forYear++)
            {
                netCashFlowItem = GetNetCashFlowItem(forYear, 
                                    netOperatingIncomeList[forYear].ItemValue, 
                                    capitalExpenseList[forYear].ItemValue);
                netCashFlowList.Add(netCashFlowItem);
            }
            return netCashFlowList;
        }

        ValueItemResponse GetNetCashFlowItem(int currentYear, 
                                            decimal netOperatingIncome, 
                                            decimal capitalExpense)
        {
            ValueItemResponse netCashFlowItem = new ValueItemResponse();
            netCashFlowItem.ForYear = currentYear;
            netCashFlowItem.ItemValue = netOperatingIncome + capitalExpense;
            return netCashFlowItem;
        }        
    }
}