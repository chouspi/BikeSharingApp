using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Web.Controllers.Api
{
    [ApiController]
    [Route("api/docs")]
    public class ApiDocsController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            List<ApiDocItem> docs = new List<ApiDocItem>();

            List<Type> controllers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t))
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("Web.Controllers.Api"))
                .OrderBy(t => t.Name)
                .ToList();

            foreach (Type controller in controllers)
            {
                string controllerRoute = "";
                RouteAttribute? routeAttribute = controller.GetCustomAttribute<RouteAttribute>();

                if (routeAttribute != null && routeAttribute.Template != null)
                {
                    controllerRoute = routeAttribute.Template;
                }

                List<AuthorizeAttribute> controllerAuthorize = controller.GetCustomAttributes<AuthorizeAttribute>().ToList();

                MethodInfo[] methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

                foreach (MethodInfo method in methods)
                {
                    List<HttpMethodAttribute> httpAttributes = method.GetCustomAttributes<HttpMethodAttribute>().ToList();

                    foreach (HttpMethodAttribute httpAttribute in httpAttributes)
                    {
                        ApiDocItem item = new ApiDocItem();
                        item.Controller = controller.Name;
                        item.Action = method.Name;
                        item.Route = GetFullRoute(controllerRoute, httpAttribute.Template);
                        item.HttpMethods = httpAttribute.HttpMethods.ToList();
                        item.Parameters = GetParameters(method);

                        List<AuthorizeAttribute> methodAuthorize = method.GetCustomAttributes<AuthorizeAttribute>().ToList();
                        bool allowAnonymous = method.GetCustomAttribute<AllowAnonymousAttribute>() != null || controller.GetCustomAttribute<AllowAnonymousAttribute>() != null;

                        item.RequiresAuthorization = !allowAnonymous && (controllerAuthorize.Count > 0 || methodAuthorize.Count > 0);
                        item.Roles = GetRoles(controllerAuthorize, methodAuthorize);

                        docs.Add(item);
                    }
                }
            }

            return Ok(docs);
        }

        private string GetFullRoute(string controllerRoute, string? actionRoute)
        {
            string route = controllerRoute;

            if (!string.IsNullOrWhiteSpace(actionRoute))
            {
                route = route.TrimEnd('/') + "/" + actionRoute.TrimStart('/');
            }

            return "/" + route.Trim('/');
        }

        private List<string> GetParameters(MethodInfo method)
        {
            List<string> parameters = new List<string>();

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                parameters.Add(parameter.Name + ": " + parameter.ParameterType.Name);
            }

            return parameters;
        }

        private List<string> GetRoles(List<AuthorizeAttribute> controllerAuthorize, List<AuthorizeAttribute> methodAuthorize)
        {
            List<string> roles = new List<string>();

            foreach (AuthorizeAttribute authorize in controllerAuthorize)
            {
                if (!string.IsNullOrWhiteSpace(authorize.Roles))
                {
                    roles.Add(authorize.Roles);
                }
            }

            foreach (AuthorizeAttribute authorize in methodAuthorize)
            {
                if (!string.IsNullOrWhiteSpace(authorize.Roles))
                {
                    roles.Add(authorize.Roles);
                }
            }

            return roles;
        }
    }

    public class ApiDocItem
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string Route { get; set; } = "";
        public List<string> HttpMethods { get; set; } = new List<string>();
        public List<string> Parameters { get; set; } = new List<string>();
        public bool RequiresAuthorization { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
