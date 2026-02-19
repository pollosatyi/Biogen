namespace Biogen.Common.Entities;

public class ImageDetectionOutcome
{
    public int Id { get; set; }
    
    public string OriginalUrl { get; set; }

    public List<DetectedItem> DetectedItems { get; set; } = new List<DetectedItem>();


}