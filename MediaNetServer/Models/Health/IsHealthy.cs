namespace MediaNetServer.Models.Health;

public class HealthResponseDto
{
    public bool IsHealthy { get; set; }
    public string? Message { get; set; }
}