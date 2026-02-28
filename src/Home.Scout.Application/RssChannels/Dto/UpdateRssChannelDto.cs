namespace Home.Scout.Application.Channels.Dto;

public class UpdateRssChannelDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}