using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using MonyApp.Models;
using MonyApp.Helpers;
using System.Configuration;
using MonyApp.Data;

namespace MonyApp.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuariosController()
        {
            // Inicializar el repositorio con la cadena de conexión desde Web.config
            var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
            var database = new MySqlDatabase(connectionString);
            _usuarioRepository = new UsuarioRepository(database);
        }


        // Endpoint para el registro de usuarios
        [HttpPost]
        [Route("registro")]
        public async Task<IHttpActionResult> Registrar(RegistroRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el email ya existe
                bool emailExiste = await _usuarioRepository.ExisteEmail(request.Email);
                if (emailExiste)
                {
                    return BadRequest("El correo electrónico ya está registrado");
                }

                // Validar que las contraseñas coincidan
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest("Las contraseñas no coinciden");
                }

                // Crear el usuario a partir de la solicitud
                var usuario = new Usuario
                {
                    NombreCompleto = request.NombreCompleto,
                    Email = request.Email,
                    Telefono = request.Telefono,
                    Password = request.Password,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                // Hashear la contraseña antes de guardarla
                usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                // Guardar el usuario en la base de datos
                int userId = await _usuarioRepository.CrearUsuario(usuario);

                // Devolver respuesta exitosa
                return Ok(new
                {
                    Success = true,
                    Message = "Usuario registrado correctamente",
                    UserId = userId
                });
            }
            catch (Exception ex)
            {
                // Loguear el error para depuración
                System.Diagnostics.Debug.WriteLine($"Error en registro: {ex.Message}");

                // Devolver error interno del servidor
                return InternalServerError(ex);
            }
        }

        // Endpoint para iniciar sesión
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(LoginRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Buscar usuario por email
                var usuario = await _usuarioRepository.ObtenerUsuarioPorEmail(request.Email);
                if (usuario == null)
                {
                    return BadRequest("Credenciales incorrectas");
                }

                // Verificar contraseña
                bool passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password);
                if (!passwordValido)
                {
                    return BadRequest("Credenciales incorrectas");
                }

                // Verificar si la cuenta está activa
                if (!usuario.Activo)
                {
                    return BadRequest("La cuenta está desactivada");
                }

                // Generar respuesta exitosa (para implementación JWT, deberías generar un token aquí)
                return Ok(new
                {
                    Success = true,
                    Message = "Inicio de sesión exitoso",
                    UserId = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Endpoint para obtener datos de un usuario por ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> ObtenerUsuario(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerUsuarioPorId(id);

                if (usuario == null)
                {
                    return NotFound();
                }

                // Devolver datos del usuario (sin la contraseña)
                return Ok(new
                {
                    Id = usuario.Id,
                    NombreCompleto = usuario.NombreCompleto,
                    Email = usuario.Email,
                    Telefono = usuario.Telefono,
                    FechaRegistro = usuario.FechaRegistro,
                    Activo = usuario.Activo
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Endpoint para recuperar contraseña
        [HttpPost]
        [Route("recuperar-password")]
        public async Task<IHttpActionResult> RecuperarPassword(RecuperarPasswordRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el email existe
                bool emailExiste = await _usuarioRepository.ExisteEmail(request.Email);
                if (!emailExiste)
                {
                    // Por seguridad, no revelar si el email existe o no
                    return Ok(new
                    {
                        Success = true,
                        Message = "Si el correo existe, recibirás instrucciones para restablecer tu contraseña"
                    });
                }

                // Aquí implementarías la lógica para generar un token y enviar un email
                // Esta es una implementación simulada
                string token = Guid.NewGuid().ToString();
                // await _emailService.EnviarEmailRecuperacion(request.Email, token);

                return Ok(new
                {
                    Success = true,
                    Message = "Si el correo existe, recibirás instrucciones para restablecer tu contraseña"
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("prueba-conexion")]
        public async Task<IHttpActionResult> ProbarConexion()
        {
            try
            {
                // Obtener la cadena de conexión
                var connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;

                // Crear conexión
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
                {
                    // Intentar abrir la conexión
                    await connection.OpenAsync();

                    // Ejecutar una consulta simple
                    using (var command = new MySql.Data.MySqlClient.MySqlCommand("SELECT 1", connection))
                    {
                        var result = await command.ExecuteScalarAsync();

                        // Si llegamos aquí, la conexión fue exitosa
                        return Ok(new
                        {
                            Success = true,
                            Message = "Conexión exitosa a la base de datos",
                            ServerInfo = connection.ServerVersion
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Devolver información detallada del error
                return InternalServerError(new Exception($"Error de conexión: {ex.Message}", ex));
            }
        }
    }


}
