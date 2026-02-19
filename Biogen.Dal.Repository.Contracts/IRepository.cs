using Biogen.Common.Entities;

namespace Biogen.Dal.Repository.Contracts;

public interface IRepository
{
    Task<bool> SaveDetectImage(ImageDetectionOutcome image);
    Task<ImageDetectionOutcome?> GetDetectionOutcomeById(int id);
    Task<bool> UpdateDetectionOutcome(ImageDetectionOutcome outcome);
}