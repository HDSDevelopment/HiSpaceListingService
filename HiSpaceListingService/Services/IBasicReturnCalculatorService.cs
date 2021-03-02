using System.Collections.Generic;
using HiSpaceListingService.BO;
using HiSpaceListingService.DTO;

namespace HiSpaceListingService.Services
{
    public interface IBasicReturnCalculatorService
    {
        void Initialize(BasicReturnCalculatorDTO basicReturnCalculatorDTO);

        BasicReturnCalculatorResponse GetResponse();
    }
}
