namespace Biogen.Common.Entities;

public class ImageDecectionOutcomeDTO
{
    public int Id { get; set; }
    
    public List<DetectedItem> DetectedItems { get; set; } = new List<DetectedItem>();


}

