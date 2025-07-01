using Org.OpenAPITools.Model;

namespace MediaNetServer.Services.Interfaces
{
    public interface IHealthCheckService
    {
        Task<HealthResponse> GetHealthStatusAsync();
    }
}
