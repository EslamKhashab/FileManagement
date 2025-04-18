using FileManagement.Business.Options;
using FileManagement.Business.Services.AttachmentService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileManagement.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusiness(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ImageSettingsOption>(configuration.GetSection("ImageSettings"));
            services.Configure<AttachmentSettingsOption>(configuration.GetSection("AttachmentSettings"));

            services.AddTransient<IAttachmentService, AttachmentService>();
            return services;

        }
    }
}