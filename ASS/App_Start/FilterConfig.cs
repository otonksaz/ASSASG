using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace ASS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new AuthorizeAttribute());
            filters.Add(new CustomAuthorizeAttribute());
        }
    }

    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(
            typeof(AllowAnonymousAttribute), inherit: true)
            || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(
            typeof(AllowAnonymousAttribute), inherit: true);

            if (skipAuthorization)
            {
                return;
            }

            if (filterContext.HttpContext.User.Identity == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
            {                
                filterContext.Result = new RedirectResult("/AssUser/Login");
            }
        }
    }

    public class CstmIdentity : IIdentity
    {
        public string Name { get; private set; }

        public CstmIdentity(string name)
        {
            this.Name = name;
        }

        public string AuthenticationType
        {
            get { return "custom"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
    }
}
