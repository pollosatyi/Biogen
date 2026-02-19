using AutoMapper;
using Biogen.Common.Entities;

namespace Biogen.BLL.Logic;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ImageDetectionOutcome, ImageDecectionOutcomeDTOPost>();
        CreateMap<DetectedItem, DetectedItemDTO>();
        CreateMap<ImageDetectionOutcome, ImageDecectionOutcomeDTOUpdate>();
    }
}
