using Biogen.Common.Entities;

namespace Biogen.BLL.LogicExtention;

public interface INeuralNetworkClient
{
    Task<ImageDetectionOutcome> DetectImageContentAsync(
        byte[] imageData, string prompt, CancellationToken cancellationToken = default);
}