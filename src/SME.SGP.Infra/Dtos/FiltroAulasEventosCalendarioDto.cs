﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SME.SGP.Infra
{
    public class FiltroAulasEventosCalendarioDto
    {
        [Required(ErrorMessage = "A Dre é obrigatória.")]
        public string DreCodigo { get; set; }

        [Required(ErrorMessage = "A Ue é obrigatória.")]
        public string UeCodigo { get; set; }

        [Required(ErrorMessage = "O ano letivo é obrigatória.")]
        public int AnoLetivo { get; set; }

    }
}
