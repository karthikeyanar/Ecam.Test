using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ecam.Framework {

	public class RouteConfiguration {
		private static bool isInitialized = false;
		public static bool IsInitialized {
			get { return isInitialized; }
		}
		private static Hashtable _routeNameToRoute = new Hashtable();
		public static Hashtable RouteNameToRoute {
			get {
				//if (_routeNameToRoute == null)
				//{
				//    RegisterRoutes(new RouteCollection());
				//}
				return _routeNameToRoute;
			}
		}

		public static void ConfigureWebApiRoutes(RouteCollection routes) {
			string defaultResourceName = RouteNames.DefaultResource;
			// Search
			string route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SEARCH);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/search", // URL with parameters
				new { action = "Search" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));

			// New route
			// GET  /Photo => Get()
			//route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.LIST);
			//_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
			//	string.Format(route_name), // Route name
			//	"{controller}", // URL with parameters
			//	new { action = "Get" }, // Parameter defaults
			//	new { httpMethod = new HttpMethodConstraint("GET") }
			//									  ));
			// GET  /Photo/1 => Get(1)	
			//string route_name = "get_default";
			//route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SHOW);
			//_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
			//    string.Format(route_name), // Route name
			//    "{controller}/{id}", // URL with parameters
			//    new {action = "Get", id = UrlParameter.Optional}, // Parameter defaults
			//    new {httpMethod = new HttpMethodConstraint("GET")}
			//                                      ));
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SHOW);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Get" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));

			// Create
			// POST /Photo => POST() or POST(Photo photo)
			route_name = "create_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.CREATE);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}", // URL with parameters
				new { action = "Post" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("POST") }
												  ));

			// UPDATE
			// POST or PUT /Photo/1 => Put(int id, Photo photo)
			route_name = "update_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.UPDATE, false);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Put" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("PUT") }));
			route_name = "update_default_low_rest";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.UPDATE);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Put" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("POST") }
												  ));

			// DELETE
			// GET /Photo/1/delete => delete(int id)
			route_name = "delete_default_lowrest";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.DELETE, true);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}/delete", // URL with parameters
				new { action = "Delete" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));
			// DELETE /Photo/id => delete(int id)
			route_name = "delete_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.DELETE, false);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Delete" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("DELETE") }
												  ));

			routes.MapRouteWithName(
			   "Default",
			   "{controller}/{action}",
			   new { controller = "Home", action = "Index" }, null
			   );

			isInitialized = true;
		}

		/// <summary>
		/// Use this method to configure default routes to MVC
		/// </summary>
		/// <param name="routes"></param>
		public static void ConfigureRoutes(RouteCollection routes) {
			string defaultResourceName = RouteNames.DefaultResource;
			string route_name = string.Empty;
			//// Search
			//string route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SEARCH);
			//_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
			//    string.Format(route_name), // Route name
			//    "{controller}/search", // URL with parameters
			//    new { action = "Search", id = UrlParameter.Optional }, // Parameter defaults
			//    new { httpMethod = new HttpMethodConstraint("GET") }
			//                                      ));

			// New route
			// GET  /Photo => Get()
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.LIST);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}", // URL with parameters
				new { action = "Get" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));
			// GET  /Photo/1 => Get(1)	
			//string route_name = "get_default";
			//route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SHOW);
			//_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
			//    string.Format(route_name), // Route name
			//    "{controller}/{id}", // URL with parameters
			//    new {action = "Get", id = UrlParameter.Optional}, // Parameter defaults
			//    new {httpMethod = new HttpMethodConstraint("GET")}
			//                                      ));
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.SHOW);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Get", id = UrlParameter.Optional }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));

			// Create
			// POST /Photo => POST() or POST(Photo photo)
			route_name = "create_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.CREATE);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}", // URL with parameters
				new { action = "Post" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("POST") }
												  ));

			// UPDATE
			// POST or PUT /Photo/1 => Put(int id, Photo photo)
			route_name = "update_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.UPDATE, false);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Put" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("PUT") }));
			route_name = "update_default_low_rest";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.UPDATE);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Put" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("POST") }
												  ));

			// DELETE
			// GET /Photo/1/delete => delete(int id)
			route_name = "delete_default_lowrest";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.DELETE, true);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}/delete", // URL with parameters
				new { action = "Delete" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("GET") }
												  ));
			// DELETE /Photo/id => delete(int id)
			route_name = "delete_default";
			route_name = RouteNames.GetRouteName(defaultResourceName, RouteAction.DELETE, false);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				string.Format(route_name), // Route name
				"{controller}/{id}", // URL with parameters
				new { action = "Delete" }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint("DELETE") }
												  ));

			routes.MapRouteWithName(
			   "Default",
			   "{controller}/{action}/{id}",
			   new { controller = "Home", action = "Index", id = UrlParameter.Optional }, null
			   );

			isInitialized = true;
		}
		public static void AddNestedRoute(RouteCollection routes, string parentCategory, string childCategory) {
			AddRoute(routes, parentCategory, childCategory, RouteAction.CREATE);
			AddRoute(routes, parentCategory, childCategory, RouteAction.DELETE);
			AddRoute(routes, parentCategory, childCategory, RouteAction.LIST);
			AddRoute(routes, parentCategory, childCategory, RouteAction.SEARCH);
			AddRoute(routes, parentCategory, childCategory, RouteAction.SHOW);
			AddRoute(routes, parentCategory, childCategory, RouteAction.UPDATE);
		}

		 

		public static Route AddRoute(RouteCollection routes,
											 string routeName, string routeUrl, object defaults, object constraints) {
			Route route = routes.MapRouteWithName(
				routeName, // Route name
				routeUrl, // URL with parameters
				defaults, // Parameter defaults
				constraints
				);
			_routeNameToRoute.Add(routeName, route);
			return route;
		}

		private static void AddRoute(RouteCollection routes, string parentCategory, string childCategory, RouteAction act, string pluralChildCategory = "", bool isLowRest = true) {
			if (!string.IsNullOrEmpty(pluralChildCategory)) {
				RouteNames.pluralResourceNames.Add(parentCategory, pluralChildCategory);
			}

			string childCategoryName = RouteNames.GetChildResourceName(childCategory, act);
			string action = string.Empty;
			string route = GetNestedRouteUrl(parentCategory, childCategory, act, out action, pluralChildCategory,
											 isLowRest);



			string route_name = RouteNames.GetNestedRouteName(parentCategory, childCategoryName, act, isLowRest);
			//AddLog(parentCategory + "/" + action + "->" + route + "/" + route_name);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				route_name, // Route name
				route, // URL with parameters
				new { controller = parentCategory, action = action }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint(GetHttpVerb(act)) }
												  ));
		}

		//private static void AddLog(string log) {
		//    try {
		//        string msg = string.Empty;
		//        string fileName = "";
		//        if(System.IO.File.Exists(fileName)){
		//            msg =  System.IO.File.ReadAllText(fileName);
		//        }
		//        msg = msg + log + Environment.NewLine + Environment.NewLine + Environment.NewLine;
		//        System.IO.File.WriteAllText(fileName, msg);
		//    } catch { }
		//}

		public static string GetNestedRouteUrl(string parentCategory, string childCategory, RouteAction act, out string action, string pluralChildCategory = "", bool isLowRest = true) {
			string childCategoryName = RouteNames.GetChildResourceName(childCategory, act);
			action = string.Empty;
			string route = string.Empty;
			//string parentCategoryIdName = parentCategory + "Id";
			string parentCategoryIdName = "id";
			// LIST
			//  GET 	Album/10/photos Get all the photos in the album
			//  get_photos_in_album
			if (act == RouteAction.LIST) {
				route = parentCategory + "/{" + parentCategoryIdName + "}/" + childCategoryName;
				action = childCategoryName;
			}


			//  Show
			//  GET     Album/10/photo/1	display a specific photo
			//  get_photo_in_album
			if (act == RouteAction.SHOW) {
				route = parentCategory + "/{" + parentCategoryIdName + "}/" + childCategoryName + "/{" +
						childCategoryName +
						"Id}";
				action = childCategoryName;
			}




			//POST 	Album/10/photo/1 	Photos 	update 	update a specific photo
			if (act == RouteAction.UPDATE) {
				route = parentCategory + "/{" + parentCategoryIdName + "}/" + childCategoryName + "/{" + childCategory +
						"Id}";
				action = "Update" + childCategoryName;
			}

			// POST 	Album/10/photo  create 	create a new photo
			if (act == RouteAction.CREATE) {
				route = parentCategory + "/{" + parentCategoryIdName + "}/" + childCategoryName;
				action = childCategoryName;
			}


			//GET 	Album/10/photo/1/delete  delete a photo
			if (act == RouteAction.DELETE) {
				route = parentCategory + "/{" + parentCategoryIdName + "}/" + childCategory + "/{" + childCategory +
						"Id}";
				action = "Delete" + childCategoryName;
			}
			return route;
		}

		/// <summary>
		/// Adds nested route with custom action name. Eg: to add parentCategory/{parentId}/actionName/{childCategoryId}
		/// eg: Fund/{fundId}/investorcommitments/{investorId} 
		/// </summary>
		/// <param name="routes"></param>
		/// <param name="actionName"></param>
		/// <param name="parentCategory"></param>
		/// <param name="childCategory"></param>
		/// <param name="act"></param>
		public static void AddNestedActionRoute(RouteCollection routes, string actionName, string parentCategory, string childCategory, RouteAction act) {
			// Fund/fundId/Action/childCategoryId
			string parentCategoryIdName = "id";
			string route = string.Empty;
			route = parentCategory + "/{" + parentCategoryIdName + "}/" + actionName + "/{" + childCategory + "Id}";

			string route_name = RouteNames.GetActionRouteName(actionName, parentCategory, childCategory, act);
			_routeNameToRoute.Add(route_name, routes.MapRouteWithName(
				route_name, // Route name
				route, // URL with parameters
				new { controller = parentCategory, action = actionName }, // Parameter defaults
				new { httpMethod = new HttpMethodConstraint(GetHttpVerb(act)) }
												  ));
		}

		private static string GetHttpVerb(RouteAction action) {
			HttpVerbs verb = HttpVerbs.Get;
			switch (action) {
				case RouteAction.CREATE:
					verb = HttpVerbs.Post;
					break;
				case RouteAction.UPDATE:
					verb = HttpVerbs.Put;
					break;
				case RouteAction.DELETE:
					verb = HttpVerbs.Delete;
					break;
				case RouteAction.LIST:
				case RouteAction.SHOW:
				case RouteAction.SEARCH:
					verb = HttpVerbs.Get;
					break;
				default:
					verb = HttpVerbs.Get;
					break;
			}
			return verb.ToString();
		}
	}

	public static class RouteNames {
		public static string DefaultResource = "default";
		internal static string GetRouteName(string resourceName, RouteAction action, bool isLowRest = true) {
			string resourceRouteName = "{0}_{1}{2}";
			string lowRest = string.Empty;
			string actionVerb = "get";
			GetRouteParameters(action, out actionVerb, out lowRest, isLowRest);
			if (action == RouteAction.LIST) {
				resourceName += "s";
			}
			return string.Format(resourceRouteName, actionVerb, resourceName, lowRest).ToLower();
		}

		public static string GetNestedRouteName(string parentResource, string childResource, RouteAction action, bool isLowRest = true) {
			string resourceRouteName = "{0}_{1}_in_{2}{3}";
			string lowRest = string.Empty;
			string actionVerb = "get";
			GetRouteParameters(action, out actionVerb, out lowRest, isLowRest);
			return string.Format(resourceRouteName, actionVerb, parentResource, childResource, lowRest).ToLower();
		}

		internal static string GetActionRouteName(string actionName, string parentResource, string childResource, RouteAction action, bool isLowRest = true) {
			string routeName = string.Empty;
			string resourceRouteName = "{0}_{1}_for_{2}_in_{3}";
			string lowRest = string.Empty;
			string actionVerb = "get";
			GetRouteParameters(action, out actionVerb, out lowRest, isLowRest);
			return string.Format(resourceRouteName, actionVerb, actionName, childResource, parentResource, lowRest).ToLower();
		}

		internal static void GetRouteParameters(RouteAction action, out string actionVerb, out string lowRest, bool isLowRest = true) {
			lowRest = string.Empty;
			actionVerb = "get";
			switch (action) {
				case RouteAction.SHOW:
				case RouteAction.LIST:
					break;
				case RouteAction.UPDATE:
					actionVerb = "update";
					if (isLowRest) {
						lowRest = "_lowrest";
					}
					break;
				case RouteAction.DELETE:
					actionVerb = "delete";
					if (isLowRest) {
						lowRest = "_lowrest";
					}
					break;
				case RouteAction.CREATE:
					actionVerb = "create";
					break;
				case RouteAction.SEARCH:
					actionVerb = "search";
					break;

			}
		}

		public static string FindRouteName(string resourceName, RouteAction action, bool isLowRest = true) {
			string resourceRouteName = GetRouteName(resourceName, action, isLowRest);
			if (!RouteConfiguration.RouteNameToRoute.ContainsKey(resourceRouteName)) {
				resourceRouteName = GetRouteName(RouteNames.DefaultResource, action, isLowRest);
			}
			return resourceRouteName;
		}

		//internal static string GetRoute(string resourceName, RouteAction action, bool isLowRest = true) {
		//    Route route = RouteConfig.RouteNameToRoute[FindRouteName(resourceName, action, isLowRest)] as Route;
		//    if (route != null) {
		//        return route.Url;
		//    }
		//    return null;
		//}

		public static string GetRoute<T>(T resource, RouteAction action, bool isLowRest = true) where T : BaseContract {
			Type resourceType = typeof(T);
			string resourceName = resourceType.Name;
			int index = resourceName.LastIndexOf(".");
			if (index > 0) {
				resourceName = resourceName.Substring(index + 1);
			}
			string routeName = FindRouteName(resourceName, action, isLowRest);
			string uri = null;
			Route route = RouteConfiguration.RouteNameToRoute[routeName] as Route;
			if (route != null) {
				uri = route.Url;
			} else {
				return null;
			}

			uri = uri.Replace("{controller}", resourceName);
			// Replace the {id} with the id of the resource
			// uri = ReplaceID(resource, uri, "{id}");
			//if (resource.id.HasValue && resource.id.Value > 0)
			//{
			//    uri = uri.Replace("{id}", resource.id.ToString());
			//}
			uri = ReplaceID(resource, uri, "{" + resourceName + "Id" + "}");
			return uri;
		}

		public static string GetRoute(string routeName) {
			Route route = RouteConfiguration.RouteNameToRoute[routeName] as Route;
			if (route != null) {
				return route.Url;
			} else {
				return null;
			}
		}

		private static string ReplaceID<T>(T resource, string uri, string parameterToReplace) {
			Type resourceType = typeof(T);
			var property = resourceType.GetProperty("id");
			if (property != null) {
				var value = property.GetValue(resource, null);
				if (value != null) {
					uri = uri.Replace(parameterToReplace, value.ToString());
				}
			}
			return uri;
		}

		internal static NameValueCollection pluralResourceNames = new NameValueCollection();

		public static string Pluralize(string name) {
			if (pluralResourceNames[name] != null) {
				return pluralResourceNames[name];
			}
			return name + "s";
		}

		public static string Singularize(string name) {
			foreach (string key in pluralResourceNames.AllKeys) {
				if (string.Compare(pluralResourceNames[key].ToLower(), name.ToLower(), StringComparison.OrdinalIgnoreCase) == 0) {
					return key;
				}
			}
			return name.Substring(0, name.Length - 1);
		}

		public static string GetChildResourceName(string childResourceName, RouteAction act) {
			if (act == RouteAction.LIST) {
				childResourceName = Pluralize(childResourceName);
			}
			return childResourceName;
		}
	}

	public enum RouteAction {
		/// <summary>
		/// Get a particular resource
		/// </summary>
		SHOW = 1,

		/// <summary>
		/// Get all the Resources
		/// </summary>
		LIST,

		/// <summary>
		/// Create a Resource
		/// </summary>
		CREATE,

		/// <summary>
		/// Update a Resource
		/// </summary>
		UPDATE,

		/// <summary>
		/// Delete a resource
		/// </summary>
		DELETE,

		/// <summary>
		/// Search a particular resource
		/// </summary>
		SEARCH
	}
}
