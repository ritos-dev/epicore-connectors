namespace RTS.SharedKernel.Commons
{
    public record EventPublisherDto(int? CurrentUserId, DateTime DateOccurred, Dictionary<string, object> Data);
}