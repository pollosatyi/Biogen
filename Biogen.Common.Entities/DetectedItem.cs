namespace Biogen.Common.Entities;

public class DetectedItem
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public List<string> materials { get; set; } = new List<string>();

    public int ImageDetectionOutcomeId { get; set; }
}