﻿using System;

namespace SME.SGP.Dominio
{
    public class Aula : EntidadeBase, ICloneable
    {
        public Aula()
        {
            Status = EntidadeStatus.Aprovado;

        }

        public Aula AulaPai { get; set; }
        public long? AulaPaiId { get; set; }
        public DateTime DataAula { get; set; }
        public string DisciplinaId { get; set; }
        public bool Excluido { get; set; }
        public bool Migrado { get; set; }
        public long ProfessorId { get; set; }
        public int Quantidade { get; set; }
        public RecorrenciaAula RecorrenciaAula { get; set; }
        public TipoAula TipoAula { get; set; }
        public TipoCalendario TipoCalendario { get; set; }
        public long TipoCalendarioId { get; set; }
        public string TurmaId { get; set; }
        public string UeId { get; set; }
        public long WorkflowAprovacaoId { get; set; }

        public EntidadeStatus Status { get; set; }
        public void AdicionarAulaPai(Aula aula)
        {
            AulaPai = aula ?? throw new NegocioException("É necessário informar uma aula.");
            AulaPaiId = aula.Id;
        }

        public object Clone()
        {
            return new Aula
            {
                AlteradoEm = AlteradoEm,
                AlteradoPor = AlteradoPor,
                AlteradoRF = AlteradoRF,
                CriadoEm = CriadoEm,
                CriadoPor = CriadoPor,
                CriadoRF = CriadoRF,
                Excluido = Excluido,
                Id = Id,
                UeId = UeId,
                AulaPai = AulaPai,
                DisciplinaId = DisciplinaId,
                AulaPaiId = AulaPaiId,
                DataAula = DataAula,
                Migrado = Migrado,
                TipoCalendario = TipoCalendario,
                ProfessorId = ProfessorId,
                Quantidade = Quantidade,
                RecorrenciaAula = RecorrenciaAula,
                TipoAula = TipoAula,
                TipoCalendarioId = TipoCalendarioId,
                TurmaId = TurmaId
            };
        }

        public void AprovaWorkflow()
        {
            if (Status != EntidadeStatus.AguardandoAprovacao)
                throw new NegocioException("Esta aula não pode ser aprovada.");

            Status = EntidadeStatus.Aprovado;
        }

        public void EnviarParaWorkflowDeAprovacao(long idWorkflow)
        {
            WorkflowAprovacaoId = idWorkflow;
            Status = EntidadeStatus.AguardandoAprovacao;
        }

        public void ReprovarWorkflow()
        {
            if (Status != EntidadeStatus.AguardandoAprovacao)
                throw new NegocioException("Este Evento não pode ser recusado.");

            Status = EntidadeStatus.Recusado;
        }

    }
}