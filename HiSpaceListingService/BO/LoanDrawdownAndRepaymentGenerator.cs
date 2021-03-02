using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class LoanDrawdownAndRepaymentGenerator
    {
        public List<ValueItemResponse> GenerateList(decimal acquisitionExitValueForZerothYear, 
                                                    decimal loanToValuePercent, 
                                                    int holdingPeriodInYears,
                                                    int additionalYears)
        {
            List<ValueItemResponse> LDRList = new List<ValueItemResponse>();
            ValueItemResponse LDRItem;

            int startYear = 0;
            int endYear = holdingPeriodInYears + additionalYears;

            for(int forYear = startYear; forYear <= endYear; forYear++)
            {
                LDRItem = GetLDRItem(acquisitionExitValueForZerothYear, 
                                    loanToValuePercent, 
                                    forYear, 
                                    holdingPeriodInYears);
                LDRList.Add(LDRItem);    
            }
        return LDRList;
        }

        public ValueItemResponse GetLDRItem(decimal acquisitionExitValueForZerothYear, 
                                            decimal loanToValuePercent,
                                            int forYear,
                                            int holdingPeriodInYears)
        {
            ValueItemResponse loanDrawdownAndRepaymentItem = new ValueItemResponse();
            loanDrawdownAndRepaymentItem.ForYear = forYear;

            if(forYear == 0)
        loanDrawdownAndRepaymentItem.ItemValue = (-1) * acquisitionExitValueForZerothYear * loanToValuePercent;

            if(forYear > 0 && forYear < holdingPeriodInYears)
            loanDrawdownAndRepaymentItem.ItemValue = 0;

            if(forYear == holdingPeriodInYears)
        loanDrawdownAndRepaymentItem.ItemValue = acquisitionExitValueForZerothYear * 
                                                    loanToValuePercent;

            if(forYear > holdingPeriodInYears)
            loanDrawdownAndRepaymentItem.ItemValue = 0;       
        
        return loanDrawdownAndRepaymentItem;
        }
    }
}