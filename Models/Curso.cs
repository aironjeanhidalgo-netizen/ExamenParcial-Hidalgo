using System.ComponentModel.DataAnnotations;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Models
{
    public class Curso : IValidatableObject
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public int Creditos { get; set; }

        public int CupoMaximo { get; set; }

        [Required]
        public TimeSpan HorarioInicio { get; set; }

        [Required]
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Creditos < 0)
                yield return new ValidationResult("Los créditos no pueden ser negativos.", new[] { nameof(Creditos) });

            if (HorarioInicio >= HorarioFin)
                yield return new ValidationResult("HorarioInicio debe ser menor que HorarioFin.", new[] { nameof(HorarioInicio), nameof(HorarioFin) });
        }
    }
}
