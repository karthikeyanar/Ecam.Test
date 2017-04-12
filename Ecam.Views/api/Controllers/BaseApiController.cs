using Ecam.Views.Repository;
using Ecam.Framework;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Ecam.Views.Controllers {

	
	public class BaseApiController : ApiController {

		protected T SingleOr404<T>(T instance) where T : class {
			if (instance == null) {
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
			}
			return instance;
		}

		//protected bool IsNestedResource(int nestedId) {
		//    // Get the RouteName
		//    string currentRouteName = this.ControllerContext.RouteData.Values["RouteName"] as string;
		//    if (currentRouteName != null) {
		//        // We have to get the child category name
		//        string url = RouteNames.GetRoute(currentRouteName);
		//        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

		//        string[] urlParts = url.Split('/');
		//        if (urlParts.Length == 3) {
		//            string childResourceName = urlParts[2];
		//            string nestedRouteName = RouteNames.GetNestedRouteName(controllerName, childResourceName, Action.SHOW);
		//            Url.Route(nestedRouteName, new {id = this.BaseRepository.ID});
		//        }
		//        string actiona = this.ControllerContext.RouteData.Values["action"].ToString();
		//        // Get the route from the current route name
		//        RouteNames.GetRoute()
		//    }
		//}

		protected string GenerateNestedLink<T>(T instance, bool usePKForChildId = true) {
			string locationUri = string.Empty;
			string parent = this.ControllerContext.RouteData.Values["controller"].ToString();
			string parentId = this.ControllerContext.RouteData.Values["id"].ToString();
			string child = this.ControllerContext.RouteData.Values["action"].ToString();
			string childIdName = string.Format("{0}Id", child);
			//string currentAction = this.ControllerContext.RouteData.Values["action"].ToString();
			// Get the id of the newly created instance
			var type = typeof(T);
			PropertyInfo property = null;
			if (usePKForChildId) {
				property = type.GetProperty("id");
			} else {
				property = type.GetProperty(childIdName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
			}
			if (property != null) {
				var id = property.GetValue(instance, null);
				if (id != null) {
					Dictionary<string, object> routeValues = new Dictionary<string, object>();
					routeValues.Add("controller", parent);
					routeValues.Add("id", parentId);
					routeValues.Add("action", child);
					routeValues.Add(childIdName, id);
					try {
						locationUri = Url.Route(RouteNames.GetNestedRouteName(parent, child, RouteAction.SHOW),
												routeValues);
					} catch {
						// Probably the route is not configured explicitly
						string action = string.Empty;
						locationUri = RouteConfiguration.GetNestedRouteUrl(parent, child, RouteAction.SHOW, out action);
						locationUri = locationUri.Replace("{id}", parentId).Replace("{" + childIdName + "}", id.ToString());
					}
				}
			}

			return locationUri;
		}

		protected string GenerateLink<T>(T instance) {
			string locationUri = string.Empty;
			string currentController = this.ControllerContext.RouteData.Values["controller"].ToString();
			//string currentAction = this.ControllerContext.RouteData.Values["action"].ToString();
			// Get the id of the newly created instance
			var type = typeof(T);
			var property = type.GetProperty("id");
			if (property != null) {
				var id = property.GetValue(instance, null);
				if (id != null) {
					locationUri = Url.Route(RouteNames.FindRouteName(RouteNames.DefaultResource, RouteAction.SHOW),
											new { controller = currentController, id = id.ToString() });
				}
			}

			return locationUri;
		}

		protected string GenerateLink(int id) {
			string locationUri = string.Empty;
			string currentController = this.ControllerContext.RouteData.Values["controller"].ToString();
			string action = this.ControllerContext.RouteData.Values["action"].ToString();
			string parentId = this.ControllerContext.RouteData.Values["id"].ToString();
			//action = RouteNames.Singularize(action);
			string route_name = RouteNames.GetNestedRouteName(currentController, action, RouteAction.SHOW);
			//string parentResourceIdName = currentController + "Id";
			string parentResourceIdName = "id";
			string childResourceIdName = action + "Id";
			Dictionary<string, object> dict = new Dictionary<string, object>();
			dict.Add(parentResourceIdName, parentId);
			dict.Add(childResourceIdName, id);
			//locationUri = Url.Route(route_name,
			//                                new { controller = currentController, id = id.ToString()});
			locationUri = Url.Route(route_name,
											dict);

			return locationUri;
		}

		protected HttpResponseMessage SuccessfullyCreatedResponse<T>(T instance) where T : class {
			// Make sure that the instance is valid
			instance = SingleOr404(instance);
			// var response = Request.CreateResponse(HttpStatusCode.Created);
			var response = CreateDynamicResponse(this.Request, HttpStatusCode.Created, instance);
			response.Headers.Add(Constants.LocationHeader, GenerateLink(instance));
			return response;
		}

		protected HttpResponseMessage SuccessfullyCreatedNestedResponse<T>(T instance, bool usePKForChildId = true) where T : class {
			// Make sure that the instance is valid
			instance = SingleOr404(instance);
			// var response = Request.CreateResponse(HttpStatusCode.Created);
			var response = CreateDynamicResponse(this.Request, HttpStatusCode.Created, instance);
			response.Headers.Add(Constants.LocationHeader, GenerateNestedLink(instance, usePKForChildId));
			return response;
		}

		protected HttpResponseMessage CreateDynamicResponse(HttpRequestMessage request, HttpStatusCode statusCode, object content) {
			var configuration = request.GetConfiguration();
			var negotiator = configuration.Services.GetContentNegotiator();
			var formatters = configuration.Formatters;

			var contentType = (content ?? new object()).GetType();
			var result = negotiator.Negotiate(contentType, request, formatters);

			//if (result == null) {
			//    return
			//        new HttpResponseMessage {
			//            StatusCode = HttpStatusCode.NotAcceptable,
			//            RequestMessage = request
			//        };
			//} else {
			//make sure we are using an xmlserializer for the base class so that the resulting output
			//has an xsi:type attribute
			// SetSerializerForType(contentType);

			return
				new HttpResponseMessage {
					StatusCode = statusCode,
					RequestMessage = request,
					Content = new ObjectContent(contentType, content, result.Formatter, result.MediaType.MediaType)
				};
			//}
		}


		protected HttpResponseMessage SuccessfullyCreatedResponse(int id) {
			// Make sure that the instance is valid
			var response = Request.CreateResponse(HttpStatusCode.Created);
			response.Headers.Add(Constants.LocationHeader, GenerateLink(id));
			return response;
		}

		//protected HttpStatusCode GetStatusCode(CRUD crud) {
		//    HttpStatusCode statusCode = HttpStatusCode.OK;
		//    switch (crud) {
		//        case CRUD.Create:
		//            statusCode = HttpStatusCode.Created;
		//            break;
		//            ;
		//        case CRUD.Delete:
		//        case CRUD.Read:
		//        case CRUD.Update:
		//            statusCode = HttpStatusCode.OK;
		//            break;
		//    }
		//    return statusCode;
		//}

		private IRepository _baseRepository = null;
		public IRepository BaseRepository {
			get { return _baseRepository; }
			protected set { _baseRepository = value; }
		}

	}

	public class BaseApiController<Contract, Model> : BaseApiController
		where Contract : BaseContract
		where Model : BaseEntity<Model> {
		private IRepository<Contract> _repository = null;

		public IRepository<Contract> Repository {
			get {

				if (_repository == null) {
					Type repositoryType = typeof(BaseRepository<,>).MakeGenericType(typeof(Contract), typeof(Model));
					_repository = (BaseRepository<Contract, Model>)Activator.CreateInstance(repositoryType);
				}
				BaseRepository = _repository;
				return _repository;
			}
			set { _repository = value; }
		}

		//public virtual List<Contract> Get() {
		//	return Repository.Get();
		//}

		[ActionName("Find")]
		[HttpGet]
		public virtual Contract Get(int? id) {
			if ((id ?? 0) > 0)
				return Repository.Get(id).SingleOr404();
			else
				return null;
		}

		// POST api/fund
		[ActionName("Create")]
		[HttpPost]
		public virtual IHttpActionResult Post(Contract contract) {
			if (contract == null) {
				return BadRequest("Contract is null");
			}
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}
			var saveContract = Repository.Save(contract);
			if (saveContract.Errors == null) {
				return Ok(Repository.Get(saveContract.id).FirstOrDefault());
			} else {
				int index = 0;
				foreach (var err in saveContract.Errors) {
					index++;
					ModelState.AddModelError("Error" + index, err.ErrorMessage);
				}
				return BadRequest(ModelState);
			}
		}

		[ActionName("Update")]
		[HttpPut]
		public virtual IHttpActionResult Put(int id, Contract contract) {
			if (contract == null) {
				return BadRequest("Contract is null");
			}
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}
			contract.id = id;
			var saveContract = Repository.Save(contract);
			if (saveContract.Errors == null) {
				return Ok(Repository.Get(saveContract.id).FirstOrDefault());
			} else {
				int index = 0;
				foreach (var err in saveContract.Errors) {
					index++;
					ModelState.AddModelError("Error" + index, err.ErrorMessage);
				}
				return BadRequest(ModelState);
			}
		}

		[ActionName("Delete")]
		[HttpDelete]
		public virtual IHttpActionResult Delete(int id) {
			IEnumerable<ErrorInfo> errors = Repository.Delete(id);
			if (errors.Any() == false) {
				return Ok("Success");
			} else {
				int index = 0;
				foreach (var err in errors) {
					index++;
					ModelState.AddModelError("Delete " + index, err.ErrorMessage);
				}
				return BadRequest(ModelState);
			}
		}

		//// GET api/T?criteria
		//[System.Web.Http.HttpGet]
		//[System.Web.Mvc.ActionName("Search")]
		//public virtual List<Contract> Search([FromUri] Contract criteria) {
		//    return Repository.Find(criteria);
		//}

		// GET api/T?criteria
		[System.Web.Http.HttpGet]
		[System.Web.Mvc.ActionName("Search")]
		public virtual PaginatedListResult<Contract> Search([FromUri] Contract criteria, [FromUri] Paging paging) {
			return Repository.Search(criteria, paging);
		}
	}
}