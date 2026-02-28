namespace Home.Scout.Application.News.Dto;

public class CreateNewsItemDto
{
    public int RssChannelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
}