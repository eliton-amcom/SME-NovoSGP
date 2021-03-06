﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SME.SGP.Dominio
{
    public enum TipoParametroSistema
    {
        //Calendário

        EjaDiasLetivos = 1,
        FundamentalMedioDiasLetivos = 2,

        //Frequência

        PercentualFrequenciaAlerta = 3,
        PercentualFrequenciaCritico = 4,
        QuantidadeAulasNotificarProfessor = 5,
        QuantidadeAulasNotificarGestorUE = 6,
        QuantidadeAulasNotificarSupervisorUE = 7,
        QuantidadeDiasNotificarAlteracaoChamadaEfetivada = 8,

        //Aula Prevista
        QuantidadeDiasNotificarProfessor = 9,

        //Compensação Ausência
        CompensacaoAusenciaPercentualRegenciaClasse = 10,

        CompensacaoAusenciaPercentualFund2 = 11,
        QuantidadeMaximaCompensacaoAusencia = 12,

        //Recuperação Paralela PAP
        RecuperacaoParalelaFrequencia = 13,

        // Média
        MediaBimestre = 14,

        PercentualAlunosInsuficientes = 15,

        // Notificação alunos ausentes
        QuantidadeDiasNotificaoCPAlunosAusentes = 16,

        QuantidadeDiasNotificaoDiretorAlunosAusentes = 17,

        //Nota Fechamento
        QuantidadeDiasAlteracaoNotaFinal = 19,

        // Frequencia
        PercentualFrequenciaCriticoBaseNacional = 20,
        
        DataUltimaAtualizacaoObjetivosJurema = 22,

        MunicipioAtendimentoHistoricoEscolar = 25,

        //Infantil
        ExecutarManutencaoAulasInfantil = 26,

        //Frequencia Infantil
        PercentualFrequenciaMinimaInfantil = 27,

        //PAP        
        PAPInicioAnoLetivo = 28,
        DataInicioSGP = 29,

        //Pendencias Gerais
        QuantidadeEventosConselhoClasse = 30,
        QuantidadeEventosAPM = 31,
        QuantidadeEventosConselhoEscolar = 32,
        QuantidadeEventosPedagogicos = 33,
        DataInicioGeracaoPendencias = 34,
        GerarPendenciaAulasDiasNaoLetivos = 35,
        GerarPendenciaDiasLetivosInsuficientes = 36,

        // Pendencias Fechamento
        DiasGeracaoPendenciaAvaliacao = 37,
        DiasGeracaoPendenciaAusenciaFechamento = 38,
        
        // Notificacao Andamento do Fechamento
        DiasNotificacaoAndamentoFechamento = 39,

        DiasNotificacaoResultadoInsatisfatorio = 40,

        // Notificação UE fechamentos insuficientes
        DiasNotificacaoFechamentoPendente = 41,
        PercentualFechamentosInsuficientesNotificacao = 42,

        DiasNotificacaoReuniaoPedagogica = 45,

        DiasNotificacaoPeriodoFechamentoUe = 46,
        DiasNotificacaoPeriodoFechamentoDre = 47,

        //Notificacao Periodo Fechamento
        DiasNotificacaoPeriodoFechamentoInicio = 43,
        DiasNotificacaoPeriodoFechamentoFim = 44,

        [Display(Name = "Pendência por ausência de registro individual")]        
        PendenciaPorAusenciaDeRegistroIndividual = 48,

        //Sistema
        HabilitarServicosEmBackground = 100
    }
}