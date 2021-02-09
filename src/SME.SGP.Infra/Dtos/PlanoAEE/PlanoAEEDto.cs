﻿using System.Collections.Generic;

namespace SME.SGP.Infra
{
    public class PlanoAEEDto
    {
        public long? Id { get; set; }
        public AuditoriaDto Auditoria { get; set; }
        public IEnumerable<QuestaoDto> Questoes { get; set; }
        public IEnumerable<PlanoAEEVersaoDto> Versoes { get; set; }
    }
        
}
