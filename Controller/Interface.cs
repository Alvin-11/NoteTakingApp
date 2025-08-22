using Microsoft.AspNetCore.Mvc;

namespace BlazorApp2.Controller
{
    public interface Interface
    {
        Task<IActionResult> CreateNote([FromBody] CreateNote request);
    }
}
