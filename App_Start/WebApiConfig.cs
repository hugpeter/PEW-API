using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;

namespace PEW_API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           
            // Web API configuration and services
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());         
            config.Filters.Add(new AuthorizeAttribute());

            GlobalConfiguration.Configuration.Formatters.Add(new FormMultipartEncodedMediaTypeFormatter(new MultipartFormatterSettings()));

            // Web API routes
            //-------------------attribute routing
            config.MapHttpAttributeRoutes();
            //------------------convention-based routing
            config.Routes.MapHttpRoute(
                name: "PEWApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
