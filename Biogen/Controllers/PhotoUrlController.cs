using Biogen.BLL.LogicExtention;
using Biogen.Common.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Biogen.Controllers;

[ApiController]
[Route("[controller]")]
public class PhotoUrlController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PhotoUrlController> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly IImageForDetectionLogic  _imageForDetectionLogic;

    public PhotoUrlController(
        IHttpClientFactory httpClientFactory,
        ILogger<PhotoUrlController> logger,
        IWebHostEnvironment env,
        IImageForDetectionLogic imageForDetectionLogic)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _env = env;
        _imageForDetectionLogic = imageForDetectionLogic;
    }

    [HttpPost]
    public async Task<ImageDecectionOutcomeDTOPost?> Post(string url)
    {
        var imageDecectionOutcomeDto=  await _imageForDetectionLogic.CreateImageModelForDetection(url);
        
        return imageDecectionOutcomeDto; 
    }
    

    [HttpPut("{id}")]
    public async Task<ImageDecectionOutcomeDTOUpdate?> Update(int id, [FromBody] string[] subjects)
    {
        return await _imageForDetectionLogic.RefineMaterialsBySubjects(id, subjects);
    }
}

