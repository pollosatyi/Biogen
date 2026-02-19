namespace Biogen.Common.Entities;

public class ImageDecectionOutcomeDTOPost
{
    public int Id { get; set; }
    
    public List<DetectedItem> DetectedItems { get; set; } = new List<DetectedItem>();


}

