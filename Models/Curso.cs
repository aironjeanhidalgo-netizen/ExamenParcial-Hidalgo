using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue)]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public byte[]? RowVersion { get; set; } // token de concurrencia

        public ICollection<Matricula>? Matriculas { get; set; }
    }
}
