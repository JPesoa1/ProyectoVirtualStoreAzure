using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using ProyectoNugetVirtualStore.Models;
using ApiVirtualStore.Models;

namespace ProyectoVirtualStore.Services
{
    public class ServiceApiTienda
    {
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApiTienda;
        

        public ServiceApiTienda(IConfiguration configuration)
        {
            this.UrlApiTienda =
               configuration.GetValue<string>("ApiUrls:ApiOAuthTienda");
            this.Header =
                new MediaTypeWithQualityHeaderValue("application/json");
        }

        public async Task<string> GetTokenAsync (string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/auth/login";
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string jsonModel = JsonConvert.SerializeObject(model);
                StringContent content =
                    new StringContent(jsonModel, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data =
                        await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(data);
                    string token =
                        jsonObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<Usuario> DetailsUsuarioAsync(string token)
        {
            string request = "api/usuarios/detailsusuario";
            Usuario usuario =
                await this.CallApiAsync<Usuario>(request,token);
            return usuario;
        }

        public async Task RegisterAsync
            (string nombreusuario, string password, string email)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/usuarios/registerusario";
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);



                UsuarioAux usuario = new UsuarioAux();
                usuario.Nombreusuario = nombreusuario;
                usuario.Password = password;
                usuario.Email = email;
                string json = JsonConvert.SerializeObject(usuario);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }



        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        private async Task<T> CallApiAsync<T> (string request, string token)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add
                    ("Authorization", "bearer " + token);
                HttpResponseMessage response =
                    await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Categorias>> GetCategoriasAsync()
        {
            string request = "api/categorias/getcategorias";
            List<Categorias> categorias =
                await this.CallApiAsync<List<Categorias>>(request);
            return categorias;
        }

        public async Task<List<Juegos>> GetJuegosAsync()
        {
            string request = "api/juegos/getjuegos";
            List<Juegos> juegos =
                await this.CallApiAsync<List<Juegos>>(request);
            return juegos;
        }

        public async Task<List<Juegos>> GetJuegosEstadosAsync(string categoria)
        {
            string request = "api/juegos/getjuegosestados/"+categoria;
            List<Juegos> juegos =
                await this.CallApiAsync<List<Juegos>>(request);
            return juegos;
        }

        public async Task<Juegos> FindJuegoAsync(int idjuego)
        {
            string request = "api/juegos/findjuego/" + idjuego;
            Juegos juego =
                await this.CallApiAsync<Juegos>(request);
            return juego;
        }


        public async Task<ModelPaginarJuegos> GetJuegosFiltrosAsync(int posicion,Decimal precio, string categoria)
        {
            string request = "api/juegos/getjuegosfiltros/" + posicion+"/"+precio+"/"+categoria;
            ModelPaginarJuegos juego =
                await this.CallApiAsync<ModelPaginarJuegos>(request);
            return juego;
        }


        public async Task InsertImagenAsync
           (string imagen , string token)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/usuarios/modificarusuarioimagen/"+imagen;
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);



                

                StringContent content =
                    new StringContent("", Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task<List<Imagenes>> GetImagenes(int idjuego)
        {
            string request = "api/imagenes/getimagenes/" + idjuego;
            List<Imagenes> imagenes =
                await this.CallApiAsync<List<Imagenes>>(request);
            return imagenes;
        }

        public async Task<List<VistaComentarios>> GetVistaComentariosAsync(int id)
        {

            string request = "api/vistacomentarios/getvistacomentarios/" + id;
            List<VistaComentarios> vistaComentarios =
                await this.CallApiAsync<List<VistaComentarios>>(request);
                return vistaComentarios;
        }


        public async Task InsertComentarioAsync
            (int idjuego, int idusuario, string comentario, DateTime fecha)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/comentarios/insertarcomentarios";
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);



                Comentarios comentarios = new Comentarios();
                comentarios.Comentario = comentario;
                comentarios.IdUsuario = idusuario;
                comentarios.IdJuego = idjuego;
                comentarios.IdComentario = 0;
                comentarios.FechaPost = fecha;
                string json = JsonConvert.SerializeObject(comentarios);
                
                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }

        public async Task<List<Juegos>> GetJuegosCarrito(List<int> ids)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "api/juegos/getjuegoscarrito";
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
               
              
                string compra = JsonConvert.SerializeObject(ids);
             
                StringContent content = new StringContent(compra, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Juegos>>(responseContent);
                }
                else
                {
                    return null;
                }
            };
        }


        public async Task InsertCompraAsync
         (List<Juegos> juegos, string token)
        {
            using (HttpClient client = new HttpClient())
            {
               
                string request = "api/compra/insertarcompra/";
                client.BaseAddress = new Uri(this.UrlApiTienda);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);


                
                string json = JsonConvert.SerializeObject(juegos);

                StringContent content =
                    new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                    await client.PostAsync(request, content);
            }
        }



       

    }
}
