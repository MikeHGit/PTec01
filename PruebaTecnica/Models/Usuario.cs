using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Models
{
    public partial class Usuario
    {
        //public int Identificador { get; set; }
        //public string Usuario1 { get; set; } = null!;
        //public string Pass { get; set; } = null!;
        //public DateTime FechaCreacion { get; set; }
        //public DateTime FechaModificacion { get; set; }

        public int Identificador { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "El nombre de usuario no puede contener espacios ni caracteres especiales.")]
        public string Usuario1 { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(@"^[^\s]+$", ErrorMessage = "La contraseña no puede contener espacios.")]
        public string Pass { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
