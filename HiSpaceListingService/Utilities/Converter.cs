using System;
using System.Linq;
using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.Utilities
{
    public static class Converter
    {
        public static double[] ToDoubleArray(List<ValueItemResponse> valueItemList, int upToYear)
        {
            if(valueItemList.Count > 0)
        return (from cashFlowItem in valueItemList
                where cashFlowItem.ForYear <= upToYear
                select (double) cashFlowItem.ItemValue)
                                                    .ToArray();
        return null;
        }

        public static double[] ToDoubleArray(List<ValueItemResponse> valueItemList, 
                                            int fromYear, 
                                            int upToYear)
        {
            if(valueItemList.Count > 0)
        return (from cashFlowItem in valueItemList
                where cashFlowItem.ForYear >= fromYear && cashFlowItem.ForYear <= upToYear
                select (double) cashFlowItem.ItemValue)
                                                    .ToArray();
        return null;
        }

        public static decimal FromPercentToDecimal(decimal percent)
        {
            return percent / 100;
        }
    }
}