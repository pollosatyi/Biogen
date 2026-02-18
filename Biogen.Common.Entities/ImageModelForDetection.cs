namespace Biogen.Common.Entities;

public class ImageModelForDetection
{
    public string OriginalUrl { get; set; }
    
    /// <summary>
    /// Бинарные данные изображения 
    /// </summary>
    public byte[] ImageData { get; set; }
    
    
}