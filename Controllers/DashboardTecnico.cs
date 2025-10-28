using Microsoft.AspNetCore.Mvc;

namespace ProjetoTi.Controllers
{
    public class DashboardTecnicoController : Controller
    {
        // GET: /DashboardTecnico
        public IActionResult Index()
        {
            return View();
        }
    }
}
