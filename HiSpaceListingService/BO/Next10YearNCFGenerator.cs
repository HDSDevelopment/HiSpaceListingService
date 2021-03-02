using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using HiSpaceListingService.DTO;
using HiSpaceListingService.Utilities;

namespace HiSpaceListingService.BO
{
    public class Next10YearNCFGenerator
    {
        public List<ValueItemResponse> GenerateList(List<ValueItemResponse> netCashFlowList,
                                                                decimal discountRatePercent,
                                                                int holdingPeriodInYears)
        {
            List<ValueItemResponse> next10YearNCFList = new List<ValueItemResponse>();
            ValueItemResponse next10YearNCFItem;            

            for(int forYear = 1; forYear <= holdingPeriodInYears; forYear++)
            {
                next10YearNCFItem = GetNext10YearNCFItem((double) discountRatePercent, netCashFlowList, forYear);
                next10YearNCFList.Add(next10YearNCFItem);
            }
            return next10YearNCFList;
        }

        ValueItemResponse GetNext10YearNCFItem(double discountRatePercent, 
                                    List<ValueItemResponse> netCashFlowList, 
                                    int forYear)
        {
            ValueItemResponse next10YearNCFItem = new ValueItemResponse();
            next10YearNCFItem.ForYear = forYear;
            double[] netCashFLowListFor10Years = Converter.ToDoubleArray(netCashFlowList, 
                                                                        forYear, forYear + 9);
            next10YearNCFItem.ItemValue = (decimal) Financial.NPV(discountRatePercent, 
                                                    ref netCashFLowListFor10Years);
            return next10YearNCFItem;
        }
    }
}