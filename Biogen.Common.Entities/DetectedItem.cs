namespace Biogen.Common.Entities;

public class DetectedItem
{
    public string Name { get; set; }

    public List<string> materials { get; set; } = new List<string>();
}