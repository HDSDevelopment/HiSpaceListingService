using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class RunningDCFValueGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> next10YearNCFList, 
                                                        List<ValueItemResponse> cappedValueList, 
                                                        decimal holdingPeriodInYears)
        {
            List<ValueItemResponse> runningDCFValueList = new List<ValueItemResponse>();
            int startYear = 0;
            ValueItemResponse runningDCFValueItem;

            for(int forYear = startYear ; forYear < holdingPeriodInYears; forYear++)
            {
                runningDCFValueItem = GetRunningDCFValueItem(next10YearNCFList[forYear], 
                                                            cappedValueList[forYear],
                                                            forYear);
                runningDCFValueList.Add(runningDCFValueItem);
            }
            return runningDCFValueList;
        }

        public ValueItemResponse GetRunningDCFValueItem(ValueItemResponse next10YearNCFItem, 
                                                        ValueItemResponse cappedValueItem,
                                                        int forYear)
        {
            ValueItemResponse runningDCFValueItem = new ValueItemResponse();
            runningDCFValueItem.ForYear = forYear + 1;
            runningDCFValueItem.ItemValue = next10YearNCFItem.ItemValue + cappedValueItem.ItemValue;
            return runningDCFValueItem;
        }
    }
}