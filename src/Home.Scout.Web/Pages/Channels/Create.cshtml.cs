using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Home.Scout.Web.Pages.Channels;

public class CreateModel : PageModel
{
    private readonly IRssChannelRepository _repository;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public CreateModel(IRssChannelRepository repository)
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

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var channel = new RssChannel
        {
            Name = Input.Name,
            Url = Input.Url,
            Description = Input.Description ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            await _repository.AddAsync(channel);
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Błąd podczas dodawania kanału: {ex.Message}");
            return Page();
        }
    }
}


