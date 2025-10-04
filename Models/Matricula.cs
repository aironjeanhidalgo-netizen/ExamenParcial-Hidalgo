using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Models
{
    public enum MatriculaEstado { Pendiente = 0, Confirmada = 1, Cancelada = 2 }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        public MatriculaEstado Estado { get; set; } = MatriculaEstado.Pendiente;

        [ForeignKey(nameof(CursoId))]
        public Curso? Curso { get; set; }
    }
}
