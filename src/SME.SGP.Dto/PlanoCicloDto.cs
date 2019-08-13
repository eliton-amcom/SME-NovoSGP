﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SME.SGP.Dto
{
    public class PlanoCicloDto
    {
        [Required(ErrorMessage = "O ano deve ser informado")]
        public long Ano { get; set; }

        [Required(ErrorMessage = "O ciclo deve ser informado")]
        public long CicloId { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A escola deve ser informada")]
        public long EscolaId { get; set; }

        public long Id { get; set; }

        public List<long> IdsMatrizesSaber { get; set; }

        public List<long> IdsObjetivosDesenvolvimento { get; set; }
    }
}