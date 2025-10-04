using System;
using System.Collections.Generic;

namespace JEAN_HIDALGO_EXAMEN_PARCIAL.Models
{
    public class CourseFilterViewModel
    {
        public string? Nombre { get; set; }
        public int? CreditosMin { get; set; }
        public int? CreditosMax { get; set; }
        public TimeSpan? HorarioInicio { get; set; }   // vinculado a <input type="time">
        public TimeSpan? HorarioFin { get; set; }
        public List<Curso>? Resultados { get; set; }
    }
}
