using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // accessible anonymously, to serve frontend from this endpoint
    [AllowAnonymous]
    public class FallbackController : Controller
    {
        // method name "Index" need to match startup UseEndpoints
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html"), "text/HTML");
        }
    }
}