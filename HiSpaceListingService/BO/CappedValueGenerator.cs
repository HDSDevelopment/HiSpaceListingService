using System;
using System.Collections.Generic;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.BO
{
    public class CappedValueGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> NetOperatingIncomeList, 
                                                    decimal exitCapRatePercent, 
                                                    decimal discountRatePercent, 
                                                    int noiAfterYear,
                                                    int holdingPeriodInYears)
        {       
            List<ValueItemResponse> cappedValueList = new List<ValueItemResponse>();
            ValueItemResponse cappedValueItem;
            
            decimal denominatorPart = GetDenominatorPart(discountRatePercent);
            int startFromNOIYear = noiAfterYear + 1;
            int endAtNOIYear = holdingPeriodInYears + noiAfterYear;
            
            for(int NOIYear = startFromNOIYear; NOIYear <= endAtNOIYear; NOIYear++)
            {
                cappedValueItem = GetCappedValueItem(NetOperatingIncomeList[NOIYear], 
                                                    exitCapRatePercent, 
                                                    denominatorPart);
                cappedValueList.Add(cappedValueItem);
            }
            return cappedValueList;                                 
        }

        ValueItemResponse GetCappedValueItem(ValueItemResponse netOperatingIncomeItem, 
                                            decimal exitCapRatePercent, 
                                            decimal denominatorPart)
        {
            ValueItemResponse cappedValueItem = new ValueItemResponse();
            cappedValueItem.ForYear = netOperatingIncomeItem.ForYear - 10;
            
            decimal numeratorPart = GetNumeratorPart(netOperatingIncomeItem.ItemValue, 
                                                    exitCapRatePercent);
            cappedValueItem.ItemValue = denominatorPart != 0 ? numeratorPart / denominatorPart : 0;
            return cappedValueItem;            
        }

        decimal GetNumeratorPart(decimal netOperatingIncomeValue, decimal exitCapRatePercent)
        {            
            return exitCapRatePercent != 0 ? netOperatingIncomeValue/exitCapRatePercent : 0;
        }

        decimal GetDenominatorPart(decimal discountRatePercent)
        {
                double discountRate = (double) (1 + discountRatePercent);                 
                double compoundedFor10Years = Math.Pow(discountRate, 10);
                decimal resultInDecimal = Convert.ToDecimal(compoundedFor10Years);
            return resultInDecimal;
        }
    }
}