﻿using SME.SGP.Dominio.Enumerados;

namespace SME.SGP.Dominio.Entidades
{
    public class EventoTipo : EntidadeBase
    {
        public bool Ativo { get; set; }
        public bool Concomitancia { get; set; }
        public bool Dependencia { get; set; }
        public string Descricao { get; set; }
        public bool Excluido { get; set; }
        public EventoLetivo Letivo { get; set; }
        public EventoLocalOcorrencia LocalOcorrencia { get; set; }
        public EventoTipoData TipoData { get; set; }
    }
}