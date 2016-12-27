using Accenture.Security.Eso.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace LCMS.Common.WebApi
{
    public class ESOActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            //Get ESO info and store it in http context
            var principal = (IEnterprisePrincipal)Thread.CurrentPrincipal;

            if (principal != null && !string.IsNullOrEmpty(principal.EnterpriseIdentity.EnterpriseId))
            {
                actionContext.RequestContext.Principal = principal;
            }
            else
            {
                actionContext.RequestContext.Principal = new BaseService.DummyEnterprisePrincipal();
            }
        }
    }
}
