﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SME.SGP.Dominio
{
    public enum SituacaoConselhoClasse
    {
        [Display(Name = "Em Andamento")]
        EmAndamento = 1,

        [Display(Name = "Concluído")]
        Concluido = 2
    }
}
