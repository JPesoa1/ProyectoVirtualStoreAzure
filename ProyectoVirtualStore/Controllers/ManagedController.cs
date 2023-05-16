using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Mvc;
using ProyectoVirtualStore.Filters;



using System.Security.Claims;
using ProyectoVirtualStore.Extensions;
using ProyectoNugetVirtualStore.Models;
using ProyectoVirtualStore.Services;
using ProyectoVirtualStore.Models;

namespace ProyectoVirtualStore.Controllers
{
    public class ManagedController : Controller
    {
        private IWebHostEnvironment webHostEnvironment;
      
        private ServiceApiTienda service;
        private ServiceStorageBlobs serviceStorageBlobs;

        public ManagedController(IWebHostEnvironment webHostEnvironment, ServiceApiTienda service, ServiceStorageBlobs serviceStorageBlobs) 
        {
            this.webHostEnvironment = webHostEnvironment;
            this.serviceStorageBlobs = serviceStorageBlobs;
            this.service = service;
        }



        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {

            string token  = await this.service.GetTokenAsync(email, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Credenciales incorrectas";
                return View();
            }
            else
            {
                Usuario usuario = await this.service.DetailsUsuarioAsync(token);
                HttpContext.Session.SetString("TOKEN", token);

                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.NameIdentifier);

                Claim claimId = new Claim(ClaimTypes.NameIdentifier,password.ToString());
                Claim claimUser = new Claim(ClaimTypes.Name, email);
                identity.AddClaim(claimId);
                identity.AddClaim(claimUser);

                if (usuario.Imagen != null)
                {
                    string imagen =await this.serviceStorageBlobs.GetBlobUriAsync("usuarios",usuario.Imagen);
                    HttpContext.Session.SetObject("imagenP", imagen);

                }
                else {

                    HttpContext.Session.SetObject("imagenP", "nada");
                }
                

                //if (token != null)
                //{
                //    Claim claimImagen = new Claim("imagen", user.Imagen);
                //    identity.AddClaim(claimId);
                //    identity.AddClaim(claimUser);
                //    identity.AddClaim(claimEmail);
                //    identity.AddClaim(claimImagen);
                //    HttpContext.Session.SetObject("imagenP", user.Imagen);
                //}
                //else {

                //    Claim claimImagen = new Claim("imagen", "nada");
                //    HttpContext.Session.SetObject("imagenP","nada");
                //    identity.AddClaim(claimId);
                //    identity.AddClaim(claimUser);
                //    identity.AddClaim(claimEmail);
                //    identity.AddClaim(claimImagen);

                //}










                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal);


                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();
                string idjuego = TempData["idjuego"].ToString();

                return RedirectToAction(action, controller , new {idjuego = idjuego});
               
            }
        }

        public IActionResult Register() { 
            
            return View();
        }


        [HttpPost]
        public async  Task<IActionResult> Register(string nombreusuario, string password , string email) {

            
            await this.service.RegisterAsync(nombreusuario,password,email);
            ViewData["MENSAJE"] = "Usuario registrado correctamnet";
            return RedirectToAction("Index","Home");
        }



        [AuthorizeUsers]
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");

        }


        [AuthorizeUsers]
        public async Task<IActionResult> User() 
        {
            

            string token =
               HttpContext.Session.GetString("TOKEN");

            Usuario usuario = await this.service.DetailsUsuarioAsync(token);
            return View(usuario);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async  Task<IActionResult> User(IFormFile file)
        {
            string rooFolder = this.webHostEnvironment.WebRootPath;
           

            string filename =  file.FileName;
            string token =
                HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await this.service.DetailsUsuarioAsync(token);
            string blobName = usuario.IdUsuario+file.FileName;
           
            using (Stream stream = file.OpenReadStream())
            {
               
                await this.service.InsertImagenAsync(blobName,token);
                
                await this.serviceStorageBlobs.UploadBlobAsync
                    ("usuarios", blobName, stream);
                HttpContext.Session.SetObject("imagenP", blobName);
            }

           

            //string path = Path.Combine(rooFolder,"perfil",filename);
            //string filenameUsuario = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value + "_" + filename;
            //string newpath = Path.Combine(rooFolder, "perfil", filenameUsuario);

            //using (Stream stream = new FileStream(path, FileMode.Create))
            //{ 
            //    await file.CopyToAsync(stream);
            //}
            //if (System.IO.File.Exists(path))
            //{
            //    // Cambiar el nombre del archivo
            //    System.IO.File.Move(path, newpath);

            //    // Actualizar la base de datos u otros datos relevantes con el nuevo nombre de archivo
            //    //await this.repo.ModificarUsuarioImagen(usuario.IdUsuario, filenameUsuario);
            //}
           

            //using (Stream stream = new FileStream(newpath, FileMode.Create))
            //{
            //    await file.CopyToAsync(stream);
            //}

            ////await this.repo.ModificarUsuarioImagen(usuario.IdUsuario,filenameUsuario);
            //HttpContext.Session.SetObject("imagenP", filename);


           
            return View(usuario);
        }

    }
}
