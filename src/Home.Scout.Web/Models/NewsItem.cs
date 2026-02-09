namespace Home.Scout.Web.Models;

/// <summary>
/// Reprezentuje element newsa z kanału RSS
/// </summary>
public class NewsItem
{
    public int Id { get; set; }
    public int RssChannelId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

