using Res.Business;
using System.Web.Mvc;
using System.Web.Routing;

namespace Res
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				 name: "Default",
				 url: ThisApp.ProjectKey + "/{controller}/{action}/{id}",
				 defaults: new { controller = "CroHome", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
