using System.Web.Mvc;
using System.Web.Routing;

namespace CondutorInfrator.Util
{
    public class AuthorizeActionFilterAtribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                object usuario = filterContext.HttpContext.User.Identity.IsAuthenticated; // filterContext.HttpContext.Session["CPF_CNPJ_LOGADO"];// 

                if (usuario != null)
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "pessoa",
                        action = "sessaoexpirada"
                    }));

                }
            }
            catch (System.Exception)
            {

                throw;
            }
          

        }




    }
}