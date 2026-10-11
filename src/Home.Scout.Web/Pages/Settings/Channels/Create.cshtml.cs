using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Home.Scout.Application.RssChannels;
using Home.Scout.Application.Channels.Dto;

namespace Home.Scout.Web.Pages.Settings.Channels;

public class CreateModel : PageModel
{
    private readonly IChannelService _channelService;

    [BindProperty] public InputModel Input { get; set; } = new();

    public CreateModel(IChannelService channelService)
    {
        _channelService = channelService;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Nazwa kanału jest wymagana")]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL kanału jest wymagany")]
        [Url]
        public string Url { get; set; } = string.Empty;

        [StringLength(1000)] public string? Description { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var dto = new CreateRssChannelDto
        {
            Name = Input.Name,
            Url = Input.Url,
            Description = Input.Description
        };

        try
        {
            await _channelService.CreateAsync(dto);
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Błąd podczas dodawania kanału: {ex.Message}");
            return Page();
        }
    }
}