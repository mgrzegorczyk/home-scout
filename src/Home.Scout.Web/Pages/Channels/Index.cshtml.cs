using Home.Scout.Web.Data;
using Home.Scout.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Home.Scout.Web.Pages.Channels;

public class IndexModel : PageModel
{
    private readonly IRssChannelRepository _repository;

    public List<RssChannel>? Channels { get; set; }

    public IndexModel(IRssChannelRepository repository)
    {
        _repository = repository;
    }

    public async Task OnGetAsync()
    {
        Channels = await _repository.GetAllAsync();
    }
}



