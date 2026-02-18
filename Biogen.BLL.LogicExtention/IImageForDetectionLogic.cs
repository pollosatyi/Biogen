using Biogen.Common.Entities;

namespace Biogen.BLL.LogicExtention;

public interface IImageForDetectionLogic
{
    Task<ImageDecectionOutcomeDTO?> CreateImageModelForDetection(string url);
    Task<ImageDetectionOutcome> CreateImageDetectionOutcome(ImageModelForDetection  imageModelForDetection);
}