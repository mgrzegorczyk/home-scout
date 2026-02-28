namespace Home.Scout.Application.Channels.Dto;

public class CreateRssChannelDto
{
    public string Name { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}