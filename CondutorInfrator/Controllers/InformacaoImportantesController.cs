using System.Web.Mvc;

namespace CondutorInfrator.Controllers
{
    [AllowAnonymous]
    public class InformacaoImportantesController : Controller
    {
        // GET: InformacaoImportantes
        public ActionResult Infor_CondutorInfrator()
        {
            return View();
        }
        public ActionResult Infor_DefesaAutuacao()
        {
            return View();
        }
        public ActionResult Infor_JARI()
        {
            return View();
        }
    }
}