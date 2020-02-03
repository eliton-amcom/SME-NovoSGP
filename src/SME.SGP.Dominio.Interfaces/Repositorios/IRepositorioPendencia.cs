﻿namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioPendencia : IRepositorioBase<Pendencia>
    {
        void AtualizarPendencias(long fechamentoId, SituacaoPendencia situacaoPendencia, TipoPendencia tipoPendencia);

        void RemoverPendenciasPorTipo(long fechamentoId, TipoPendencia tipoPendencia);
    }
}