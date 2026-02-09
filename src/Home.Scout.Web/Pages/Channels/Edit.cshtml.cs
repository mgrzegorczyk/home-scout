using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Home.Scout.Web.Pages.Channels;

public class EditModel : PageModel
{
    private readonly IRssChannelRepository _repository;

    public RssChannel? Channel { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public EditModel(IRssChannelRepository repository)
    {
        _repository = repository;
    }

    public class InputModel
    {
        [Required(ErrorMessage = "Nazwa kanału jest wymagana")]
        [StringLength(255, ErrorMessage = "Nazwa nie może być dłuższa niż 255 znaków")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "URL kanału jest wymagany")]
        [Url(ErrorMessage = "URL musi być prawidłowym adresem")]
        public string Url { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Opis nie może być dłuższy niż 1000 znaków")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Channel = await _repository.GetByIdAsync(id);
        if (Channel == null)
        {
            return NotFound();
        }

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
            Channel = await _repository.GetByIdAsync(id);
            return Page();
        }

        var channel = await _repository.GetByIdAsync(id);
        if (channel == null)
        {
            return NotFound();
        }

        channel.Name = Input.Name;
        channel.Url = Input.Url;
        channel.Description = Input.Description ?? string.Empty;
        channel.UpdatedAt = DateTime.UtcNow;

        try
        {
            var result = await _repository.UpdateAsync(channel);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Błąd podczas aktualizacji kanału: {ex.Message}");
            Channel = channel;
            return Page();
        }
    }
}


