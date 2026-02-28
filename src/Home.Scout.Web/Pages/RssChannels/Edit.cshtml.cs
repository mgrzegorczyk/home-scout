using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Home.Scout.Application.Channels.Dto;
using Home.Scout.Application.RssChannels;

namespace Home.Scout.Web.Pages.RssChannels;

public class EditModel : PageModel
{
    private readonly IChannelService _channelService;

    public RssChannelDto? Channel { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public EditModel(IChannelService channelService)
    {
        _channelService = channelService;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Channel name is required")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Channel URL is required")]
        [Url]
        public string Url { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Channel = await _channelService.GetByIdAsync(id);

        if (Channel == null)
            return NotFound();

        Input = new InputModel
        {
            Name = Channel.Name,
            Url = Channel.Url,
            Description = Channel.Description
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            Channel = await _channelService.GetByIdAsync(id);
            return Page();
        }

        var dto = new UpdateRssChannelDto
        {
            Id = id,
            Name = Input.Name,
            Url = Input.Url,
            Description = Input.Description
        };

        try
        {
            await _channelService.UpdateAsync(dto);
            return RedirectToPage("Index");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Błąd podczas aktualizacji kanału: {ex.Message}");
            Channel = await _channelService.GetByIdAsync(id);
            return Page();
        }
    }
}