using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class RunningDirectCapValueGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> NOIList, 
                                            decimal exitCapRatePercent,
                                            int holdingPeriodInYears)
        {
            List<ValueItemResponse> runningValueList = new List<ValueItemResponse>();
            ValueItemResponse runningValueItem;
            
            int startFromNOIYear = 2;
            int endBeforeNOIYear = startFromNOIYear + holdingPeriodInYears;
            
            for(int forYear = startFromNOIYear; forYear < endBeforeNOIYear; forYear++)
            {
                runningValueItem = GetRunningValueItem(NOIList[forYear], exitCapRatePercent);
                runningValueList.Add(runningValueItem);
            }
            return runningValueList;
        }

        ValueItemResponse GetRunningValueItem(ValueItemResponse NOI, decimal exitCapRate)
        {
            ValueItemResponse runningValueItem = new ValueItemResponse();
            runningValueItem.ForYear = NOI.ForYear - 1;
            runningValueItem.ItemValue = exitCapRate != 0 ? NOI.ItemValue / exitCapRate : 0;
            return runningValueItem;
        }
    }
}