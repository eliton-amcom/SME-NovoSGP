﻿using SME.SGP.Dominio;

namespace SME.SGP.Infra.Dtos.Relatorios
{
    public class FiltroRelatorioNotasEConceitosFinaisDto
    {
        public int AnoLetivo { get; set; }
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public Modalidade? Modalidade { get; set; }
        public int? Semestre { get; set; }
        public string[] Anos { get; set; }
        public string[] ComponentesCurriculares { get; set; }
        public int? Bimestre { get; set; }
        public string UsuarioRf { get; set; }
        public string UsuarioNome { get; set; }
        public string Condicao { get; set; }
        public string ValorCondicao { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
    }
}