using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MonyApp.Models
{
    public class Usuario
    {
        // Propiedades principales que corresponden a las columnas de la base de datos
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Ingrese un número telefónico válido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 255 caracteres")]
        public string Password { get; set; }

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }

        // Constructor para inicializar valores por defecto
        public Usuario()
        {
            FechaRegistro = DateTime.Now;
            Activo = true;
        }
    }
}