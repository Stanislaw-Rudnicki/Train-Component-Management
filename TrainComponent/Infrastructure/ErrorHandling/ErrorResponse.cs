namespace TrainComponent.Infrastructure.ErrorHandling;

public class ErrorResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Details { get; set; } = null;
}
