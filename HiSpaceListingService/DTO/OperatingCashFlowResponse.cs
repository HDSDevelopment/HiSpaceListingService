using System.Collections.Generic;

namespace HiSpaceListingService.DTO
{
    public class OperatingCashFlowResponse
    {
        public List<ValueItemResponse> InflationRateList {get; set;}
        public List<ValueItemResponse> NetOperatingIncomeList {get; set;}
        public List<ValueItemResponse> CapitalExpenseList {get; set;}
        public List<ValueItemResponse> NetCashFlowList {get; set;}
    }
}