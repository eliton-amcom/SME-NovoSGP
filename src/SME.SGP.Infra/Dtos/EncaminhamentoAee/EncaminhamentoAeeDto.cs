﻿namespace SME.SGP.Infra
{
    public class EncaminhamentoAEEDto
    {
        public long TurmaId { get; set; }
        public string AlunoCodigo { get; set; }
        public long SecaoId { get; set; }
        public QuestaoAeeDto Questionario { get; set; }
    }
}
