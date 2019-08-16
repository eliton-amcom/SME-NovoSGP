﻿using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Aplicacao.Integracoes.Respostas;
using SME.SGP.Dto;
using System.Collections.Generic;
using System.Linq;

namespace SME.SGP.Aplicacao
{
    public class ConsultasObjetivoAprendizagem : IConsultasObjetivoAprendizagem
    {
        private readonly ServicoJurema servicoJurema;

        public ConsultasObjetivoAprendizagem(ServicoJurema servicoJurema)
        {
            this.servicoJurema = servicoJurema;
        }

        public IEnumerable<ObjetivoAprendizagemDto> Listar()
        {
            return MapearParaDto(servicoJurema.ObterListaObjetivosAprendizagem());
        }

        private IEnumerable<ObjetivoAprendizagemDto> MapearParaDto(IEnumerable<ObjetivoAprendizagemResposta> objetivos)
        {
            return objetivos?.Select(m => new ObjetivoAprendizagemDto()
            {
                Descricao = m.Descricao,
                Id = m.Id,
                Ano = m.Ano,
                AtualizadoEm = m.AtualizadoEm,
                Codigo = m.Codigo,
                CriadoEm = m.CriadoEm,
                IdComponenteCurricular = m.IdComponenteCurricular
            });
        }
    }
}