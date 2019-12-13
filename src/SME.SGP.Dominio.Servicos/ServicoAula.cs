﻿using Microsoft.Extensions.Configuration;
using SME.SGP.Aplicacao;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Aplicacao.Integracoes.Respostas;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoAula : IServicoAula
    {
        private readonly IComandosPlanoAula comandosPlanoAula;

        private readonly IComandosWorkflowAprovacao comandosWorkflowAprovacao;

        private readonly IConfiguration configuration;

        private readonly IConsultasAbrangencia consultasAbrangencia;

        private readonly IConsultasGrade consultasGrade;

        private readonly IConsultasPeriodoEscolar consultasPeriodoEscolar;

        private readonly IRepositorioAtividadeAvaliativa repositorioAtividadeAvaliativa;

        private readonly IRepositorioAtribuicaoCJ repositorioAtribuicaoCJ;
        private readonly IRepositorioAula repositorioAula;

        private readonly IRepositorioTipoCalendario repositorioTipoCalendario;

        private readonly IRepositorioTurma repositorioTurma;
        private readonly IServicoDiaLetivo servicoDiaLetivo;

        private readonly IServicoEOL servicoEOL;

        private readonly IServicoFrequencia servicoFrequencia;

        private readonly IServicoLog servicoLog;

        private readonly IServicoNotificacao servicoNotificacao;

        private readonly IConsultasFrequencia consultasFrequencia;
        private readonly IConsultasPlanoAula consultasPlanoAula;

        public ServicoAula(IRepositorioAula repositorioAula,
                           IServicoEOL servicoEOL,
                           IRepositorioTipoCalendario repositorioTipoCalendario,
                           IServicoDiaLetivo servicoDiaLetivo,
                           IConsultasGrade consultasGrade,
                           IConsultasPeriodoEscolar consultasPeriodoEscolar,
                           IConsultasFrequencia consultasFrequencia,
                           IConsultasPlanoAula consultasPlanoAula,
                           IServicoLog servicoLog,
                           IServicoNotificacao servicoNotificacao,
                           IConsultasAbrangencia consultasAbrangencia,
                           IComandosWorkflowAprovacao comandosWorkflowAprovacao,
                           IComandosPlanoAula comandosPlanoAula,
                           IServicoFrequencia servicoFrequencia,
                           IConfiguration configuration,
                           IRepositorioAtividadeAvaliativa repositorioAtividadeAvaliativa,
                           IRepositorioAtribuicaoCJ repositorioAtribuicaoCJ,
                           IRepositorioTurma repositorioTurma)
        {
            this.repositorioAula = repositorioAula ?? throw new System.ArgumentNullException(nameof(repositorioAula));
            this.servicoEOL = servicoEOL ?? throw new System.ArgumentNullException(nameof(servicoEOL));
            this.repositorioTipoCalendario = repositorioTipoCalendario ?? throw new System.ArgumentNullException(nameof(repositorioTipoCalendario));
            this.servicoDiaLetivo = servicoDiaLetivo ?? throw new System.ArgumentNullException(nameof(servicoDiaLetivo));
            this.consultasGrade = consultasGrade ?? throw new System.ArgumentNullException(nameof(consultasGrade));
            this.consultasPeriodoEscolar = consultasPeriodoEscolar ?? throw new ArgumentNullException(nameof(consultasPeriodoEscolar));
            this.consultasFrequencia = consultasFrequencia ?? throw new ArgumentNullException(nameof(consultasFrequencia));
            this.consultasPlanoAula = consultasPlanoAula ?? throw new ArgumentNullException(nameof(consultasPlanoAula));
            this.servicoLog = servicoLog ?? throw new ArgumentNullException(nameof(servicoLog));
            this.consultasAbrangencia = consultasAbrangencia ?? throw new ArgumentNullException(nameof(consultasAbrangencia));
            this.comandosWorkflowAprovacao = comandosWorkflowAprovacao ?? throw new ArgumentNullException(nameof(comandosWorkflowAprovacao));
            this.configuration = configuration;
            this.servicoNotificacao = servicoNotificacao ?? throw new ArgumentNullException(nameof(servicoNotificacao));
            this.comandosPlanoAula = comandosPlanoAula ?? throw new ArgumentNullException(nameof(comandosPlanoAula));
            this.servicoFrequencia = servicoFrequencia ?? throw new ArgumentNullException(nameof(servicoFrequencia));
            this.repositorioAtividadeAvaliativa = repositorioAtividadeAvaliativa ?? throw new ArgumentNullException(nameof(repositorioAtividadeAvaliativa));
            this.repositorioAtribuicaoCJ = repositorioAtribuicaoCJ ?? throw new ArgumentNullException(nameof(repositorioAtribuicaoCJ));
            this.repositorioTurma = repositorioTurma ?? throw new ArgumentNullException(nameof(repositorioTurma));
        }

        private enum Operacao
        {
            Inclusao,
            Alteracao,
            Exclusao
        }

        public async Task<string> Excluir(Aula aula, RecorrenciaAula recorrencia, Usuario usuario)
        {
            await ExcluirAula(aula, usuario.CodigoRf);

            if (recorrencia != RecorrenciaAula.AulaUnica)
                await ExcluirRecorrencia(aula, recorrencia, usuario);

            return "Aula e suas dependencias excluídas com sucesso!";
        }

        public void GravarRecorrencia(bool inclusao, Aula aula, Usuario usuario, RecorrenciaAula recorrencia)
        {
            var fimRecorrencia = consultasPeriodoEscolar.ObterFimPeriodoRecorrencia(aula.TipoCalendarioId, aula.DataAula.Date, recorrencia);

            if (inclusao)
                GerarRecorrencia(aula, usuario, fimRecorrencia);
            else
                AlterarRecorrencia(aula, usuario, fimRecorrencia);
        }

        public string Salvar(Aula aula, Usuario usuario, RecorrenciaAula recorrencia)
        {
            var tipoCalendario = repositorioTipoCalendario.ObterPorId(aula.TipoCalendarioId);

            if (tipoCalendario == null)
                throw new NegocioException("O tipo de calendário não foi encontrado.");

            IEnumerable<long> disciplinasProfessor = null;

            if (usuario.EhProfessorCj())
            {
                IEnumerable<AtribuicaoCJ> lstDisciplinasProfCJ = repositorioAtribuicaoCJ.ObterPorFiltros(null, aula.TurmaId, aula.UeId, 0, usuario.CodigoRf, usuario.Nome, null).Result;

                if (lstDisciplinasProfCJ != null && lstDisciplinasProfCJ.Any())
                    disciplinasProfessor = lstDisciplinasProfCJ.Select(d => d.DisciplinaId);
            }
            else
            {
                IEnumerable<DisciplinaResposta> lstDisciplinasProf = servicoEOL.ObterDisciplinasPorCodigoTurmaLoginEPerfil(aula.TurmaId, usuario.Login, usuario.PerfilAtual).Result;

                if (lstDisciplinasProf != null && lstDisciplinasProf.Any())
                    disciplinasProfessor = lstDisciplinasProf.Select(d => Convert.ToInt64(d.CodigoComponenteCurricular));
            }

            var usuarioPodeCriarAulaNaTurmaUeEModalidade = repositorioAula.UsuarioPodeCriarAulaNaUeTurmaEModalidade(aula, tipoCalendario.Modalidade);

            if (disciplinasProfessor == null || !disciplinasProfessor.Any(c => c.ToString() == aula.DisciplinaId) || !usuarioPodeCriarAulaNaTurmaUeEModalidade)
                throw new NegocioException("Você não pode criar aulas para essa UE/Turma/Disciplina.");

            var temLiberacaoExcepcionalNessaData = servicoDiaLetivo.ValidaSeEhLiberacaoExcepcional(aula.DataAula, aula.TipoCalendarioId, aula.UeId);
           
              if (!temLiberacaoExcepcionalNessaData)
            {
                if (!servicoDiaLetivo.ValidarSeEhDiaLetivo(aula.DataAula, aula.TipoCalendarioId, null, aula.UeId))
                    throw new NegocioException("Não é possível cadastrar essa aula pois a data informada está fora do período letivo.");
            }


            if (aula.RecorrenciaAula != RecorrenciaAula.AulaUnica && aula.TipoAula == TipoAula.Reposicao)
                throw new NegocioException("Uma aula do tipo Reposição não pode ser recorrente.");

            var ehInclusao = aula.Id == 0;

            if (aula.RecorrenciaAula == RecorrenciaAula.AulaUnica && aula.TipoAula == TipoAula.Reposicao)
            {
                var aulas = repositorioAula.ObterAulas(aula.TipoCalendarioId, aula.TurmaId, aula.UeId, usuario.CodigoRf).Result;
                var quantidadeDeAulasSomadas = aulas.ToList().FindAll(x => x.DataAula.Date == aula.DataAula.Date).Sum(x => x.Quantidade) + aula.Quantidade;

                var turma = repositorioTurma.ObterTurmaComUeEDrePorId(aula.TurmaId);

                if (turma == null)
                    throw new NegocioException("Turma não localizada.");

                if (ReposicaoDeAulaPrecisaDeAprovacao(quantidadeDeAulasSomadas, turma))
                {
                    var nomeDisciplina = RetornaNomeDaDisciplina(aula, usuario);

                    repositorioAula.Salvar(aula);
                    PersistirWorkflowReposicaoAula(aula, turma.Ue.Dre.Nome, turma.Ue.Nome, nomeDisciplina,
                                                 turma.Nome, turma.Ue.Dre.CodigoDre);
                    return "Aula cadastrada com sucesso e enviada para aprovação.";
                }
            }
            else
            {
                if (usuario.EhProfessorCj() && aula.Quantidade > 2)
                    throw new NegocioException("Quantidade de aulas por dia/disciplina excedido.");

                // Busca quantidade de aulas semanais da grade de aula
                var semana = (aula.DataAula.DayOfYear / 7) + 1;
                var gradeAulas = consultasGrade.ObterGradeAulasTurmaProfessor(aula.TurmaId, int.Parse(aula.DisciplinaId), semana.ToString(), aula.DataAula, usuario.CodigoRf).Result;
                               
                var quantidadeAulasRestantes = gradeAulas == null ? int.MaxValue : gradeAulas.QuantidadeAulasRestante;

                if (!ehInclusao)
                {
                    // Na alteração tem que considerar que uma aula possa estar mudando de dia na mesma semana, então não soma as aulas do proprio registro
                    var aulasSemana = repositorioAula.ObterAulas(aula.TipoCalendarioId, aula.TurmaId, aula.UeId, usuario.CodigoRf, mes: null, semanaAno: semana, disciplinaId: aula.DisciplinaId).Result;
                    var quantidadeAulasSemana = aulasSemana.Where(a => a.Id != aula.Id).Sum(a => a.Quantidade);

                    quantidadeAulasRestantes = gradeAulas == null ? int.MaxValue : gradeAulas.QuantidadeAulasGrade - quantidadeAulasSemana;
                }

                if ((gradeAulas != null) && (quantidadeAulasRestantes < aula.Quantidade))
                    throw new NegocioException("Quantidade de aulas superior ao limíte de aulas da grade.");
            }

            repositorioAula.Salvar(aula);

            // Verifica recorrencia da gravação
            if (recorrencia != RecorrenciaAula.AulaUnica)
            {
                Background.Core.Cliente.Executar<IServicoAula>(x => x.GravarRecorrencia(ehInclusao, aula, usuario, recorrencia));

                var mensagem = ehInclusao ? "cadastrada" : "alterada";
                return $"Aula {mensagem} com sucesso. Serão {mensagem}s aulas recorrentes, em breve você receberá uma notificação com o resultado do processamento.";
            }
            return "Aula cadastrada com sucesso.";
        }

        private static bool ReposicaoDeAulaPrecisaDeAprovacao(int quantidadeAulasExistentesNoDia, Turma turma)
        {
            int.TryParse(turma.Ano, out int anoTurma);
            return ((turma.ModalidadeCodigo == Modalidade.Fundamental && anoTurma >= 1 && anoTurma <= 5) ||  //Valida se é Fund 1
                               (Modalidade.EJA == turma.ModalidadeCodigo && (anoTurma == 1 || anoTurma == 2)) // Valida se é Eja Alfabetizacao ou  Basica
                               && quantidadeAulasExistentesNoDia > 1) || ((turma.ModalidadeCodigo == Modalidade.Fundamental && anoTurma >= 6 && anoTurma <= 9) || //valida se é fund 2
                                     (Modalidade.EJA == turma.ModalidadeCodigo && anoTurma == 3 || anoTurma == 4) ||  // Valida se é Eja Complementar ou Final
                                     (turma.ModalidadeCodigo == Modalidade.Medio) && quantidadeAulasExistentesNoDia > 2);
        }

        private void AlterarRecorrencia(Aula aula, Usuario usuario, DateTime fimRecorrencia)
        {
            var dataRecorrencia = aula.DataAula.AddDays(7);
            var aulasRecorrencia = repositorioAula.ObterAulasRecorrencia(aula.AulaPaiId ?? aula.Id, aula.Id, fimRecorrencia).Result;
            List<(DateTime data, string erro)> aulasQueDeramErro = new List<(DateTime, string)>();

            foreach (var aulaRecorrente in aulasRecorrencia)
            {
                aulaRecorrente.DataAula = dataRecorrencia;
                aulaRecorrente.Quantidade = aula.Quantidade;

                try
                {
                    Salvar(aulaRecorrente, usuario, aulaRecorrente.RecorrenciaAula);
                }
                catch (NegocioException nex)
                {
                    aulasQueDeramErro.Add((dataRecorrencia, nex.Message));
                }
                catch (Exception ex)
                {
                    servicoLog.Registrar(ex);
                    aulasQueDeramErro.Add((dataRecorrencia, $"Erro Interno: {ex.Message}"));
                }

                dataRecorrencia = dataRecorrencia.AddDays(7);
            }

            NotificarUsuario(usuario, aula, Operacao.Alteracao, aulasRecorrencia.Count() - aulasQueDeramErro.Count, aulasQueDeramErro);
        }

        private async Task ExcluirAula(Aula aula, string CodigoRf)
        {
            if (await repositorioAtividadeAvaliativa.VerificarSeExisteAvaliacao(aula.DataAula.Date, aula.UeId, aula.TurmaId, CodigoRf, aula.DisciplinaId))
                throw new NegocioException("Aula com avaliação vinculada. Para excluir esta aula primeiro deverá ser excluída a avaliação.");
            await servicoFrequencia.ExcluirFrequenciaAula(aula.Id);
            await comandosPlanoAula.ExcluirPlanoDaAula(aula.Id);
            aula.Excluido = true;
            await repositorioAula.SalvarAsync(aula);
        }

        private async Task ExcluirRecorrencia(Aula aula, RecorrenciaAula recorrencia, Usuario usuario)
        {
            var fimRecorrencia = consultasPeriodoEscolar.ObterFimPeriodoRecorrencia(aula.TipoCalendarioId, aula.DataAula.Date, recorrencia);
            var aulasRecorrencia = await repositorioAula.ObterAulasRecorrencia(aula.AulaPaiId ?? aula.Id, aula.Id, fimRecorrencia);
            List<(DateTime data, string erro)> aulasQueDeramErro = new List<(DateTime, string)>();
            List<(DateTime data, bool existeFrequencia, bool existePlanoAula)> aulasComFrenciaOuPlano = new List<(DateTime data, bool existeFrequencia, bool existePlanoAula)>();

            foreach (var aulaRecorrente in aulasRecorrencia)
            {
                try
                {
                    var existeFrequencia = await consultasFrequencia.FrequenciaAulaRegistrada(aulaRecorrente.Id);
                    var existePlanoAula = await consultasPlanoAula.PlanoAulaRegistrado(aulaRecorrente.Id);

                    if (existeFrequencia || existePlanoAula)
                        aulasComFrenciaOuPlano.Add((aulaRecorrente.DataAula, existeFrequencia, existePlanoAula));

                    await ExcluirAula(aulaRecorrente, usuario.CodigoRf);
                }
                catch (NegocioException nex)
                {
                    aulasQueDeramErro.Add((aulaRecorrente.DataAula, nex.Message));
                }
                catch (Exception ex)
                {
                    servicoLog.Registrar(ex);
                    aulasQueDeramErro.Add((aulaRecorrente.DataAula, $"Erro Interno: {ex.Message}"));
                }
            }

            NotificarUsuario(usuario, aula, Operacao.Exclusao, aulasRecorrencia.Count() - aulasQueDeramErro.Count, aulasQueDeramErro, aulasComFrenciaOuPlano);
        }

        private void GerarAulaDeRecorrenciaParaDias(Aula aula, List<DateTime> diasParaIncluirRecorrencia, Usuario usuario)
        {
            List<(DateTime data, string erro)> aulasQueDeramErro = new List<(DateTime, string)>();

            foreach (var dia in diasParaIncluirRecorrencia)
            {
                var aulaParaAdicionar = (Aula)aula.Clone();
                aulaParaAdicionar.DataAula = dia;
                aulaParaAdicionar.AdicionarAulaPai(aula);
                aulaParaAdicionar.RecorrenciaAula = RecorrenciaAula.AulaUnica;

                try
                {
                    Salvar(aulaParaAdicionar, usuario, aulaParaAdicionar.RecorrenciaAula);
                }
                catch (NegocioException nex)
                {
                    aulasQueDeramErro.Add((dia, nex.Message));
                }
                catch (Exception ex)
                {
                    servicoLog.Registrar(ex);
                    aulasQueDeramErro.Add((dia, "Erro Interno."));
                }
            }

            NotificarUsuario(usuario, aula, Operacao.Inclusao, diasParaIncluirRecorrencia.Count - aulasQueDeramErro.Count, aulasQueDeramErro);
        }

        private void GerarRecorrencia(Aula aula, Usuario usuario, DateTime fimRecorrencia)
        {
            var inicioRecorrencia = aula.DataAula.AddDays(7);

            GerarRecorrenciaParaPeriodos(aula, inicioRecorrencia, fimRecorrencia, usuario);
        }

        private void GerarRecorrenciaParaPeriodos(Aula aula, DateTime inicioRecorrencia, DateTime fimRecorrencia, Usuario usuario)
        {
            List<DateTime> diasParaIncluirRecorrencia = new List<DateTime>();

            diasParaIncluirRecorrencia.AddRange(ObterDiaEntreDatas(inicioRecorrencia, fimRecorrencia));

            GerarAulaDeRecorrenciaParaDias(aula, diasParaIncluirRecorrencia, usuario);
        }

        private void NotificarUsuario(Usuario usuario, Aula aula, Operacao operacao, int quantidade, List<(DateTime data, string erro)> aulasQueDeramErro, List<(DateTime data, bool existeFrequencia, bool existePlanoAula)> aulasComFrenciaOuPlano = null)
        {
            var perfilAtual = usuario.PerfilAtual;
            if (perfilAtual == Guid.Empty)
                throw new NegocioException($"Não foi encontrado o perfil do usuário informado.");

            var turma = repositorioTurma.ObterTurmaComUeEDrePorId(aula.TurmaId);

            if (turma is null)
                throw new NegocioException($"Não foi possível localizar a turma de Id {aula.TurmaId} na abrangência ");

            var disciplinasEol = servicoEOL.ObterDisciplinasPorCodigoTurmaLoginEPerfil(aula.TurmaId, usuario.Login, perfilAtual).Result;

            if (disciplinasEol is null || !disciplinasEol.Any())
                throw new NegocioException($"Não foi possível localizar as disciplinas da turma {aula.TurmaId}");

            var disciplina = disciplinasEol.FirstOrDefault(a => a.CodigoComponenteCurricular == int.Parse(aula.DisciplinaId));

            if (disciplina == null)
                throw new NegocioException($"Não foi possível localizar a disciplina de Id {aula.DisciplinaId}.");

            var operacaoStr = operacao == Operacao.Inclusao ? "Criação" : operacao == Operacao.Alteracao ? "Alteração" : "Exclusão";
            var tituloMensagem = $"{operacaoStr} de Aulas de {disciplina.Nome} na turma {turma.Nome}";
            StringBuilder mensagemUsuario = new StringBuilder();

            operacaoStr = operacao == Operacao.Inclusao ? "criadas" : operacao == Operacao.Alteracao ? "alteradas" : "excluídas";
            mensagemUsuario.Append($"Foram {operacaoStr} {quantidade} aulas da disciplina {disciplina.Nome} para a turma {turma.Nome} da {turma.Ue?.Nome} ({turma.Ue?.Dre?.Nome}).");

            if (aulasComFrenciaOuPlano != null && aulasComFrenciaOuPlano.Any())
            {
                mensagemUsuario.Append($"<br><br>Nas seguintes datas haviam registros de plano de aula e frequência:<br>");

                foreach (var aulaFrequenciaOuPlano in aulasComFrenciaOuPlano)
                {
                    var frequenciaPlano = aulaFrequenciaOuPlano.existeFrequencia ?
                                            "Frequência" + (aulaFrequenciaOuPlano.existePlanoAula ? " e " : "")
                                            : "Plano de Aula";
                    mensagemUsuario.Append($"<br /> {aulaFrequenciaOuPlano.data.ToShortDateString()} - {frequenciaPlano}");
                }
            }

            if (aulasQueDeramErro.Any())
            {
                operacaoStr = operacao == Operacao.Inclusao ? "criar" : operacao == Operacao.Alteracao ? "alterar" : "excluir";
                mensagemUsuario.Append($"<br><br>Não foi possível {operacaoStr} aulas nas seguintes datas:<br>");
                foreach (var aulaComErro in aulasQueDeramErro)
                {
                    mensagemUsuario.AppendFormat("<br /> {0} - {1}", $"{aulaComErro.data.Day}/{aulaComErro.data.Month}/{aulaComErro.data.Year}", aulaComErro.erro);
                }
            }

            servicoNotificacao.Salvar(new Notificacao()
            {
                Ano = aula.CriadoEm.Year,
                Categoria = NotificacaoCategoria.Aviso,
                DreId = turma.Ue.Dre.CodigoDre,
                Mensagem = mensagemUsuario.ToString(),
                UsuarioId = usuario.Id,
                Tipo = NotificacaoTipo.Calendario,
                Titulo = tituloMensagem,
                TurmaId = aula.TurmaId,
                UeId = turma.Ue.CodigoUe,
            });
        }

        private IEnumerable<DateTime> ObterDiaEntreDatas(DateTime inicio, DateTime fim)
        {
            for (DateTime i = inicio; i < fim; i = i.AddDays(7))
            {
                yield return i;
            }
        }

        private void PersistirWorkflowReposicaoAula(Aula aula, string nomeDre, string nomeEscola, string nomeDisciplina,
                                                          string nomeTurma, string dreId)
        {
            var linkParaReposicaoAula = $"{configuration["UrlFrontEnd"]}calendario-escolar/calendario-professor/cadastro-aula/editar/:{aula.Id}/";

            var wfAprovacaoAula = new WorkflowAprovacaoDto()
            {
                Ano = aula.DataAula.Year,
                NotificacaoCategoria = NotificacaoCategoria.Workflow_Aprovacao,
                EntidadeParaAprovarId = aula.Id,
                Tipo = WorkflowAprovacaoTipo.ReposicaoAula,
                UeId = aula.UeId,
                DreId = dreId,
                NotificacaoTitulo = $"Criação de Aula de Reposição na turma {nomeTurma}",
                NotificacaoTipo = NotificacaoTipo.Calendario,
                NotificacaoMensagem = $"Foram criadas {aula.Quantidade} aula(s) de reposição de {nomeDisciplina} na turma {nomeTurma} da {nomeEscola} ({nomeDre}). Para que esta aula seja considerada válida você precisa aceitar esta notificação. Para visualizar a aula clique  <a href='{linkParaReposicaoAula}'>aqui</a>."
            };

            wfAprovacaoAula.Niveis.Add(new WorkflowAprovacaoNivelDto()
            {
                Cargo = Cargo.CP,
                Nivel = 1
            });
            wfAprovacaoAula.Niveis.Add(new WorkflowAprovacaoNivelDto()
            {
                Cargo = Cargo.Diretor,
                Nivel = 2
            });

            var idWorkflow = comandosWorkflowAprovacao.Salvar(wfAprovacaoAula);

            aula.EnviarParaWorkflowDeAprovacao(idWorkflow);

            repositorioAula.Salvar(aula);
        }

        private string RetornaNomeDaDisciplina(Aula aula, Usuario usuario)
        {
            var disciplinasEol = servicoEOL.ObterDisciplinasPorCodigoTurmaLoginEPerfil(aula.TurmaId, usuario.Login, usuario.PerfilAtual).Result;

            if (disciplinasEol is null && !disciplinasEol.Any())
                throw new NegocioException($"Não foi possível localizar as disciplinas da turma {aula.TurmaId}");

            var disciplina = disciplinasEol.FirstOrDefault(a => a.CodigoComponenteCurricular == int.Parse(aula.DisciplinaId));

            if (disciplina == null)
                throw new NegocioException($"Não foi possível localizar a disciplina de Id {aula.DisciplinaId}.");

            return disciplina.Nome;
        }
    }
}