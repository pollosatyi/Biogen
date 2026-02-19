using Biogen.Common.Entities;

namespace Biogen.BLL.LogicExtention;

public interface IImageDetectionOutcomeLogic
{
    Task<ImageDetectionOutcome> CreateImageDetectionOutcome(ImageModelForDetection  imageModelForDetection);
}