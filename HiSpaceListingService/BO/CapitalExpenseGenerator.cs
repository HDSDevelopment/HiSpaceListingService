using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class CapitalExpenseGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal estimatedCapExOrThroughHold,
                                    int holdingPeriodInYears,
                                    int additionalYears)
        {
        List<ValueItemResponse> capitalExpenseList = new List<ValueItemResponse>();
         decimal capitalExpenseDuringHoldingPeriod = GetCapitalExpenseDuringHoldingPeriod(
                                                        estimatedCapExOrThroughHold, 
                                                        holdingPeriodInYears);
        ValueItemResponse capitalExpenseItem;
        int startYear = 0;
        int endYear = holdingPeriodInYears + additionalYears;
        
        for(int forYear = startYear; forYear <= endYear; forYear++)
        {
            capitalExpenseItem = new ValueItemResponse();
            capitalExpenseItem = GetCapitalExpenseItem(forYear, 
                                            holdingPeriodInYears, 
                                            capitalExpenseDuringHoldingPeriod);
            capitalExpenseList.Add(capitalExpenseItem);
        }
        return capitalExpenseList;
        }

        ValueItemResponse GetCapitalExpenseItem(int forYear, 
                                    decimal holdingPeriodInYears, 
                                    decimal capitalExpenseDuringHoldingPeriod)
        {
            ValueItemResponse capitalExpenseItem = new ValueItemResponse();
            capitalExpenseItem.ForYear = forYear;
            capitalExpenseItem.ItemValue = forYear == 0 || forYear > holdingPeriodInYears ? 0 : capitalExpenseDuringHoldingPeriod;

            return capitalExpenseItem;
        }

        decimal GetCapitalExpenseDuringHoldingPeriod(decimal EstimatedCapExOrTIThroughHold, 
                                                    decimal holdingPeriodInYears)
        {
                return (-1) * (EstimatedCapExOrTIThroughHold / holdingPeriodInYears);
        }
    }
}