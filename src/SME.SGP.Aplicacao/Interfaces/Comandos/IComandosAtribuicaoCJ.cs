﻿using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IComandosAtribuicaoCJ
    {
        Task Salvar(AtribuicaoCJPersistenciaDto atribuicaoCJPersistenciaDto);
    }
}