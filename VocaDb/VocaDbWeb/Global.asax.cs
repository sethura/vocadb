﻿using System.Web.Mvc;
using System.Web.Routing;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web {

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication {

		private static ServiceModel serviceModel;

		public static LoginManager LoginManager {
			get {
				return new LoginManager();
			}
		}

		public static ServiceModel Services {
			get {
				
				if (serviceModel == null)
					serviceModel = new ServiceModel(DatabaseConfiguration.BuildSessionFactory());

				return serviceModel;

			}
		}

		public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes) {
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", // Route name
				"{controller}/{action}/{id}", // URL with parameters
				new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
			);

		}

		protected void Application_Start() {
			AreaRegistration.RegisterAllAreas();

			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}