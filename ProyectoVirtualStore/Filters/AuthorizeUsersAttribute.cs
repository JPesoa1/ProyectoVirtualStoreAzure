using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ProyectoVirtualStore.Filters
{
    public class AuthorizeUsersAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;


            string controller =
               context.RouteData.Values["controller"].ToString();

            string action =
                context.RouteData.Values["action"].ToString();






            string idjuego = "";
            if (context.RouteData.Values.ContainsKey("idjuego"))
            {
                idjuego = context.RouteData.Values["idjuego"].ToString();
            }


            //RepositoryHospital repo =
            //    context.HttpContext.RequestServices.GetService<RepositoryHospital>();
            //Esto me sirve si quisiera inyectar algun servicio, repo ,helper ,etc.. en una clase cualquiera (lo de arriba)

            ITempDataProvider provider =
                context.HttpContext.RequestServices.GetService<ITempDataProvider>();

            var TempData = provider.LoadTempData(context.HttpContext);

            TempData["controller"] = controller;
            TempData["action"] = action;
            TempData["idjuego"] = idjuego;


            //ALMACENAMOS NUESTRO TEMPDATA DENTRO DE LA APP
            provider.SaveTempData(context.HttpContext, TempData);

            if (user.Identity.IsAuthenticated == false)
            {
                //NECESITAMOS REALIZAR LA REDIRECCION PARA 
                //LLEVARNOS LA PETICION A LOGIN DE MANAGED
                RouteValueDictionary rutalogin = new RouteValueDictionary(new
                {
                    controller = "Managed",
                    action = "Login"
                });
                RedirectToRouteResult result = new RedirectToRouteResult(rutalogin);
                context.Result = result;
            }
        }
    }
}
