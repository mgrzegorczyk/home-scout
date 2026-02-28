namespace Home.Scout.Application.News.Dto;

public class NewsItemDto
{
    public int Id { get; set; }
    public int RssChannelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}