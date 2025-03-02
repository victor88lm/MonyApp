using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MonyApp.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    // Respuesta específica para datos de usuario
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        // Nota: No incluimos Password por seguridad
    }

    // Respuesta específica para login
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Token { get; set; } // JWT token o similar para autenticación
    }
}