using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class PostTaxCashFlowGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> leveredCashFlowList,
                                                    List<ValueItemResponse> taxList,
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> postTaxCashFlowList = new List<ValueItemResponse>();
            ValueItemResponse postTaxCashFlowItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;

            for (int forYear = startYear; forYear <= endYear; forYear++)
            {
                postTaxCashFlowItem = GetPostTaxCashFlowItem(leveredCashFlowList[forYear].ItemValue,
                                                            taxList[forYear].ItemValue,
                                                            forYear,
                                                            holdingPeriodInYears);
                postTaxCashFlowList.Add(postTaxCashFlowItem);
            }
            return postTaxCashFlowList;
        }

        ValueItemResponse GetPostTaxCashFlowItem(decimal leveredCashFlowValue,
                                                decimal taxValue,
                                                int forYear,
                                                int holdingPeriodInYears)
        {
            ValueItemResponse postTaxCashFlowItem = new ValueItemResponse();
            postTaxCashFlowItem.ForYear = forYear;            

            if(forYear >= 0 && forYear <= holdingPeriodInYears)
                postTaxCashFlowItem.ItemValue = leveredCashFlowValue + taxValue;

            if(forYear > holdingPeriodInYears)
            postTaxCashFlowItem.ItemValue = 0;

        return postTaxCashFlowItem;
        } 
    }
}