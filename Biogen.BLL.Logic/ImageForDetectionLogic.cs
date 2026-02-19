using AutoMapper;
using Biogen.BLL.LogicExtention;
using Biogen.Common.Entities;
using Biogen.Dal.Repository.Contracts;
using Microsoft.Extensions.Logging;

namespace Biogen.BLL.Logic;

public class ImageForDetectionLogic : IImageForDetectionLogic
{
    private readonly HttpClient _httpClient;
    private readonly INeuralNetworkClient _neuralNetworkClient;
    private readonly string _prompt =
        "Выбери один или несколько предметов на изображении и укажи из чего они сделаны. " +
        "Верни ответ строго в формате JSON без пояснений: " +
        "{\"DetectedItems\":[{\"Name\":\"название объекта\",\"materials\":[\"материал1\",\"материал2\"]}]}";
    private readonly IRepository _repository;
    private readonly IMapper _mapper;
    private ILogger<ImageForDetectionLogic> _logger;

    public ImageForDetectionLogic(
        HttpClient httpClient,
        INeuralNetworkClient neuralNetworkClient,
        IMapper mapper,
        IRepository repository,
        ILogger<ImageForDetectionLogic> logger)
    {
        _httpClient = httpClient;
        _neuralNetworkClient = neuralNetworkClient;
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<ImageDecectionOutcomeDTOPost?> CreateImageModelForDetection(string url)
    {
        ImageModelForDetection imageModel;
        try
        {
             imageModel = await CreateImageModel(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, "Failed to create image model for detection");
            throw;
        }

        ImageDetectionOutcome imageDetectionOutcome;
        try
        {
            imageDetectionOutcome = await CreateImageDetectionOutcome(imageModel);
            imageDetectionOutcome.OriginalUrl = imageModel.OriginalUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, "CreateImageDetectionOutcome  failed");
            throw;
        }

        bool IsSaveImage;
        try
        {
            IsSaveImage = await _repository.SaveDetectImage(imageDetectionOutcome);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, "Failed to save image in DB");
            throw;
        }

        if (IsSaveImage)
        {
            return _mapper.Map<ImageDecectionOutcomeDTOPost>(imageDetectionOutcome);
        }
        return null;
        
    }


    public async Task<ImageDetectionOutcome> CreateImageDetectionOutcome(ImageModelForDetection imageModelForDetection)
    {
        ImageDetectionOutcome outcome;
        try
        {
             outcome = await _neuralNetworkClient.DetectImageContentAsync(
                imageModelForDetection.ImageData, _prompt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, "Failed to create image detection outcome");
            throw;
        }

        return outcome;
    }

    private async Task<ImageModelForDetection> CreateImageModel(string url)
    {
        ImageModelForDetection imageModel = new ImageModelForDetection();
        imageModel.OriginalUrl = url;
        imageModel.ImageData = await GetByteOfImage(url);
        return imageModel;
    }

    public async Task<ImageDecectionOutcomeDTOUpdate?> RefineMaterialsBySubjects(int id, string[] subjects)
    {
        var outcome = await _repository.GetDetectionOutcomeById(id);
        if (outcome == null) return null;

        var imageBytes = await GetByteOfImage(outcome.OriginalUrl);

        var prompt = BuildSubjectPrompt(subjects);
        var refinedOutcome = await _neuralNetworkClient.DetectImageContentAsync(imageBytes, prompt);

        foreach (var refinedItem in refinedOutcome.DetectedItems)
        {
            var existing = outcome.DetectedItems
                .FirstOrDefault(d => d.Name.Equals(refinedItem.Name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
                existing.materials = refinedItem.materials;
        }

        await _repository.UpdateDetectionOutcome(outcome);

        return _mapper.Map<ImageDecectionOutcomeDTOUpdate>(outcome);
    }

    private string BuildSubjectPrompt(string[] subjects) =>
        $"Для следующих предметов на изображении: {string.Join(", ", subjects)} — " +
        "укажи из чего они сделаны. " +
        "Верни ответ строго в формате JSON без пояснений: " +
        "{\"DetectedItems\":[{\"Name\":\"название объекта\",\"materials\":[\"материал1\",\"материал2\"]}]}";

    private async Task<byte[]> GetByteOfImage(string url)
    {
        try
        {
            using var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message, "Failed to create image");
            throw;
        }
    }
}