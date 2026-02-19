using Biogen.BLL.LogicExtention;
using Microsoft.Extensions.DependencyInjection;

namespace Biogen.BLL.Logic;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddHttpClient<IImageForDetectionLogic, ImageForDetectionLogic>();
        return services;
    }
}
