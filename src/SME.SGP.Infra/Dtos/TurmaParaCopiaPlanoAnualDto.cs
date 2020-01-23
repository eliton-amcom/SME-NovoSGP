﻿using Newtonsoft.Json;

namespace SME.SGP.Infra
{
    public class TurmaParaCopiaPlanoAnualDto
    {
        [JsonProperty("nomeTurma")]
        public string Nome { get; set; }

        public bool PossuiPlano { get; set; }

        [JsonProperty("codTurma")]
        public int TurmaId { get; set; }
    }
}