﻿using System.ComponentModel.DataAnnotations;

namespace SME.SGP.Dominio.Enumerados
{
    public enum SituacaoAEE
    {
        [Display(Name = "Em digitação")]
        Rascunho = 1,
        [Display(Name = "Aguardando validação da coordenação")]
        Encaminhado = 2,
        [Display(Name = "Aguardando análise do AEE")]
        Analise = 3,
        [Display(Name = "Finalizado")]
        Finalizado = 4,
        [Display(Name = "Encerrado")]
        Encerrado = 5,
        [Display(Name = "Aguardando atribuição de responsável")]
        AtribuicaoResponsavel = 6
    }
}
