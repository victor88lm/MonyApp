using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonyApp.Models;
namespace MonyApp.Helpers
{
    public static class ModelMappers
    {
        // Convierte un objeto Usuario a UsuarioResponse
        public static UsuarioResponse ToResponse(this Usuario usuario)
        {
            if (usuario == null)
                return null;

            return new UsuarioResponse
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                FechaRegistro = usuario.FechaRegistro
            };
        }

        // Convierte un RegistroRequest a un objeto Usuario
        public static Usuario ToEntity(this RegistroRequest request)
        {
            if (request == null)
                return null;

            return new Usuario
            {
                NombreCompleto = request.NombreCompleto,
                Email = request.Email,
                Telefono = request.Telefono,
                Password = request.Password, // Nota: Esta contraseña debe ser hasheada antes de almacenarla
                FechaRegistro = DateTime.Now,
                Activo = true
            };
        }

        // Convierte una lista de objetos Usuario a una lista de UsuarioResponse
        public static List<UsuarioResponse> ToResponseList(this IEnumerable<Usuario> usuarios)
        {
            if (usuarios == null)
                return new List<UsuarioResponse>();

            return usuarios.Select(u => u.ToResponse()).ToList();
        }

        // Método para convertir un usuario a LoginResponse
        public static LoginResponse ToLoginResponse(this Usuario usuario, string token)
        {
            if (usuario == null)
                return null;

            return new LoginResponse
            {
                UserId = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email,
                Token = token
            };
        }
    }
}