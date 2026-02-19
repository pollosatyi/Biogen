using Biogen.Common.Entities;

namespace Biogen.BLL.LogicExtention;

public interface IImageForDetectionLogic
{
    Task<ImageDecectionOutcomeDTOPost?> CreateImageModelForDetection(string url);
    Task<ImageDetectionOutcome> CreateImageDetectionOutcome(ImageModelForDetection imageModelForDetection);
    Task<ImageDecectionOutcomeDTOUpdate?> RefineMaterialsBySubjects(int id, string[] subjects);
}