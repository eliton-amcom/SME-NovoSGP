﻿using Microsoft.Extensions.Configuration;
using Sentry;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoNotificacaoFrequencia : IServicoNotificacaoFrequencia
    {
        private readonly IConfiguration configuration;
        private readonly IRepositorioFrequencia repositorioFrequencia;
        private readonly IRepositorioFrequenciaAlunoDisciplinaPeriodo repositorioFrequenciaAluno;
        private readonly IRepositorioNotificacaoFrequencia repositorioNotificacaoFrequencia;
        private readonly IRepositorioParametrosSistema repositorioParametrosSistema;
        private readonly IRepositorioSupervisorEscolaDre repositorioSupervisorEscolaDre;
        private readonly IRepositorioCompensacaoAusencia repositorioCompensacaoAusencia;
        private readonly IRepositorioCompensacaoAusenciaAluno repositorioCompensacaoAusenciaAluno;
        private readonly IRepositorioTurma repositorioTurma;
        private readonly IRepositorioUe repositorioUe;
        private readonly IRepositorioDre repositorioDre;
        private readonly IRepositorioNotificacaoCompensacaoAusencia repositorioNotificacaoCompensacaoAusencia;
        private readonly IRepositorioPeriodoEscolar repositorioPeriodoEscolar;
        private readonly IRepositorioTipoCalendario repositorioTipoCalendario;
        private readonly IServicoEOL servicoEOL;
        private readonly IServicoNotificacao servicoNotificacao;
        private readonly IServicoUsuario servicoUsuario;

        public ServicoNotificacaoFrequencia(IRepositorioNotificacaoFrequencia repositorioNotificacaoFrequencia,
                                            IRepositorioParametrosSistema repositorioParametrosSistema,
                                            IRepositorioFrequencia repositorioFrequencia,
                                            IRepositorioFrequenciaAlunoDisciplinaPeriodo repositorioFrequenciaAluno,
                                            IRepositorioSupervisorEscolaDre repositorioSupervisorEscolaDre,
                                            IRepositorioCompensacaoAusencia repositorioCompensacaoAusencia,
                                            IRepositorioCompensacaoAusenciaAluno repositorioCompensacaoAusenciaAluno,
                                            IRepositorioTurma repositorioTurma,
                                            IRepositorioUe repositorioUe,
                                            IRepositorioDre repositorioDre,
                                            IRepositorioNotificacaoCompensacaoAusencia repositorioNotificacaoCompensacaoAusencia,
                                            IRepositorioPeriodoEscolar repositorioPeriodoEscolar,
                                            IRepositorioTipoCalendario repositorioTipoCalendario,
                                            IServicoNotificacao servicoNotificacao,
                                            IServicoUsuario servicoUsuario,
                                            IServicoEOL servicoEOL,
                                            IConfiguration configuration)
        {
            this.repositorioNotificacaoFrequencia = repositorioNotificacaoFrequencia ?? throw new ArgumentNullException(nameof(repositorioNotificacaoFrequencia));
            this.repositorioParametrosSistema = repositorioParametrosSistema ?? throw new ArgumentNullException(nameof(repositorioParametrosSistema));
            this.servicoNotificacao = servicoNotificacao ?? throw new ArgumentNullException(nameof(servicoNotificacao));
            this.repositorioFrequencia = repositorioFrequencia ?? throw new ArgumentNullException(nameof(repositorioFrequencia));
            this.repositorioFrequenciaAluno = repositorioFrequenciaAluno ?? throw new ArgumentNullException(nameof(repositorioFrequenciaAluno));
            this.servicoUsuario = servicoUsuario ?? throw new ArgumentNullException(nameof(servicoUsuario));
            this.repositorioSupervisorEscolaDre = repositorioSupervisorEscolaDre ?? throw new ArgumentNullException(nameof(repositorioSupervisorEscolaDre));
            this.repositorioCompensacaoAusencia = repositorioCompensacaoAusencia ?? throw new ArgumentNullException(nameof(repositorioCompensacaoAusencia));
            this.repositorioCompensacaoAusenciaAluno = repositorioCompensacaoAusenciaAluno ?? throw new ArgumentNullException(nameof(repositorioCompensacaoAusenciaAluno));
            this.repositorioTurma = repositorioTurma ?? throw new ArgumentNullException(nameof(repositorioTurma));
            this.repositorioUe = repositorioUe ?? throw new ArgumentNullException(nameof(repositorioUe));
            this.repositorioDre = repositorioDre ?? throw new ArgumentNullException(nameof(repositorioDre));
            this.repositorioNotificacaoCompensacaoAusencia = repositorioNotificacaoCompensacaoAusencia ?? throw new ArgumentNullException(nameof(repositorioNotificacaoCompensacaoAusencia));
            this.repositorioPeriodoEscolar = repositorioPeriodoEscolar ?? throw new ArgumentNullException(nameof(repositorioPeriodoEscolar));
            this.repositorioTipoCalendario = repositorioTipoCalendario ?? throw new ArgumentNullException(nameof(repositorioTipoCalendario));
            this.servicoEOL = servicoEOL ?? throw new ArgumentNullException(nameof(servicoEOL));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        #region Metodos Publicos
        public void ExecutaNotificacaoFrequencia()
        {
            Console.WriteLine($"Notificando usuários de aulas sem frequência.");
            NotificarAusenciaFrequencia(TipoNotificacaoFrequencia.Professor);
            NotificarAusenciaFrequencia(TipoNotificacaoFrequencia.GestorUe);
            NotificarAusenciaFrequencia(TipoNotificacaoFrequencia.SupervisorUe);
            Console.WriteLine($"Rotina finalizada.");
        }

        public void VerificaRegraAlteracaoFrequencia(long registroFrequenciaId, DateTime criadoEm, DateTime alteradoEm, long usuarioAlteracaoId)
        {
            // Parametro do sistema de dias para notificacao
            var qtdDiasParametro = int.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(
                                                    TipoParametroSistema.QuantidadeDiasNotificarAlteracaoChamadaEfetivada,
                                                    DateTime.Now.Year));

            var qtdDiasAlteracao = (alteradoEm.Date - criadoEm.Date).TotalDays;

            // Verifica se ultrapassou o limite de dias para alteração
            if (qtdDiasAlteracao >= qtdDiasParametro)
            {
                var usuariosNotificacao = new List<Usuario>();

                // Dados da Aula
                var registroFrequencia = repositorioFrequencia.ObterAulaDaFrequencia(registroFrequenciaId);
                MeusDadosDto professor = servicoEOL.ObterMeusDados(registroFrequencia.ProfessorRf).Result;

                // Gestores
                var usuarios = BuscaGestoresUe(registroFrequencia.CodigoUe);
                if (usuarios != null)
                    usuariosNotificacao.AddRange(usuarios);

                // Supervisores
                usuarios = BuscaSupervisoresUe(registroFrequencia.CodigoUe);
                if (usuarios != null)
                    usuariosNotificacao.AddRange(usuarios);

                foreach (var usuario in usuariosNotificacao)
                {
                    NotificaAlteracaoFrequencia(usuario, registroFrequencia, professor.Nome);
                }
            }
        }

        public void NotificarCompensacaoAusencia(long compensacaoId)
        {
            // Verifica se compensação possui alunos vinculados
            var alunos = repositorioCompensacaoAusenciaAluno.ObterPorCompensacao(compensacaoId).Result;
            if (alunos == null || !alunos.Any())
                return;

            // Verifica se possui aluno não notificado na compensação
            alunos = alunos.Where(a => !a.Notificado);
            if (!alunos.Any())
                return;

            // Carrega dados da compensacao a notificar
            var compensacao = repositorioCompensacaoAusencia.ObterPorId(compensacaoId);
            var turma = repositorioTurma.ObterPorId(compensacao.TurmaId);
            var ue = repositorioUe.ObterUEPorTurma(turma.CodigoTurma);
            var dre = repositorioDre.ObterPorId(ue.DreId);
            var disciplinaEOL = ObterNomeDisciplina(compensacao.DisciplinaId);
            MeusDadosDto professor = servicoEOL.ObterMeusDados(compensacao.CriadoRF).Result;

            // Carrega dados dos alunos não notificados
            var alunosTurma = servicoEOL.ObterAlunosPorTurma(turma.CodigoTurma).Result;
            var alunosDto = new List<CompensacaoAusenciaAlunoQtdDto>();
            foreach (var aluno in alunos)
            {
                var alunoEol = alunosTurma.FirstOrDefault(a => a.CodigoAluno == aluno.CodigoAluno);
                alunosDto.Add(new CompensacaoAusenciaAlunoQtdDto()
                {
                    CodigoAluno = aluno.CodigoAluno,
                    NomeAluno = alunoEol.NomeAluno,
                    QuantidadeCompensacoes = aluno.QuantidadeFaltasCompensadas
                });
            }

            var gestores = BuscaGestoresUe(ue.CodigoUe);
            if (gestores != null && gestores.Any())
            {
                foreach (var gestor in gestores)
                {
                    var notificacaoId = NotificarCompensacaoAusencia(compensacaoId
                            , gestor
                            , professor.Nome
                            , disciplinaEOL
                            , turma.CodigoTurma
                            , turma.Nome
                            , ue.CodigoUe
                            , ue.Nome
                            , dre.CodigoDre
                            , dre.Nome
                            , compensacao.Bimestre
                            , compensacao.Nome
                            , alunosDto);

                    // Grava vinculo de notificação x compensação
                    repositorioNotificacaoCompensacaoAusencia.Inserir(notificacaoId, compensacaoId);
                }

                // Marca aluno como notificado
                alunosDto.ForEach(alunoDto =>
                {
                    var aluno = alunos.FirstOrDefault(a => a.CodigoAluno == alunoDto.CodigoAluno);
                    aluno.Notificado = true;
                    repositorioCompensacaoAusenciaAluno.Salvar(aluno);
                });
            }
        }

        public void VerificaNotificacaoBimestral()
        {
            var dataAtual = DateTime.Now.Date;

            // Verifica Notificação Fund e Medio
            var tipoCalendario = repositorioTipoCalendario.BuscarPorAnoLetivoEModalidade(dataAtual.Year, ModalidadeTipoCalendario.FundamentalMedio);
            if (tipoCalendario != null)
                VerificaNotificacaoBimestralCalendario(tipoCalendario, dataAtual, ModalidadeTipoCalendario.FundamentalMedio);

            // Verifica Notificação EJA
            tipoCalendario = repositorioTipoCalendario.BuscarPorAnoLetivoEModalidade(dataAtual.Year, ModalidadeTipoCalendario.EJA, dataAtual.Month > 6 ? 2 : 1);
            if (tipoCalendario != null)
                VerificaNotificacaoBimestralCalendario(tipoCalendario, dataAtual, ModalidadeTipoCalendario.EJA);
        }
        #endregion

        #region Metodos Privados
        private void VerificaNotificacaoBimestralCalendario(TipoCalendario tipoCalendario, DateTime dataAtual, ModalidadeTipoCalendario modalidade)
        {
            var periodos = repositorioPeriodoEscolar.ObterPorTipoCalendario(tipoCalendario.Id);
            var periodoAtual = periodos.FirstOrDefault(p => p.PeriodoInicio <= dataAtual && p.PeriodoFim >= dataAtual);

            if (periodoAtual == null)
                return;

            var ultimoBimestre = periodoAtual == periodos.OrderBy(x => x.PeriodoInicio).Last();
            // Ultimo dia do bimestre e primeiro dia do ultimo mes quando ser tratar do ultimo bimestre
            var dataReferencia = ultimoBimestre ?
                                    new DateTime(periodoAtual.PeriodoFim.Year, periodoAtual.PeriodoFim.Month, 1) :
                                    periodoAtual.PeriodoFim;

            if (dataAtual == dataReferencia)
            {
                var alunosAusentes = repositorioFrequenciaAluno.ObterAlunosComAusenciaNoBimestre(periodoAtual.Bimestre);

                // Carrega dados das disciplinas do EOL
                var disciplinasIds = alunosAusentes.Select(a => long.Parse(a.DisciplinaId)).ToArray();
                var disciplinasEol = servicoEOL.ObterDisciplinasPorIds(disciplinasIds);

                // Carrega dados das turmas (ue e dre)
                var turmas = new List<Turma>();
                alunosAusentes.Select(a => a.TurmaId).Distinct().ToList()
                    .ForEach(turmaId => turmas.Add(repositorioTurma.ObterTurmaComUeEDrePorId(turmaId)));

                var percentualFrequenciaFund = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.CompensacaoAusenciaPercentualFund2));
                var percentualFrequenciaRegencia = double.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.CompensacaoAusenciaPercentualRegenciaClasse));

                // Agrupa por DRE / UE / Turma / Disciplina
                foreach (var turmasDRE in turmas.GroupBy(t => t.Ue.Dre))
                {
                    foreach (var turmasUE in turmasDRE.GroupBy(x => x.Ue))
                    {
                        var gestores = BuscaGestoresUe(turmasUE.Key.CodigoUe);

                        foreach (var turma in turmasUE)
                        {
                            var alunosEOL = servicoEOL.ObterAlunosPorTurma(turma.CodigoTurma).Result;

                            var alunosTurma = alunosAusentes.Where(c => c.TurmaId == turma.CodigoTurma);
                            alunosTurma.Select(a => a.DisciplinaId).Distinct().ToList()
                                .ForEach(disciplinaId =>
                                {
                                    var alunosDisciplina = alunosTurma.Where(c => c.DisciplinaId == disciplinaId);

                                    var alunosDto = new List<CompensacaoAusenciaAlunoQtdDto>();

                                    foreach (var alunoDisciplina in alunosDisciplina)
                                    {
                                        if (alunoDisciplina.PercentualFrequencia < (modalidade == ModalidadeTipoCalendario.FundamentalMedio ? percentualFrequenciaFund : percentualFrequenciaRegencia))
                                        {
                                            alunosDto.Add(new CompensacaoAusenciaAlunoQtdDto()
                                            {
                                                CodigoAluno = alunoDisciplina.CodigoAluno,
                                                NomeAluno = alunosEOL.FirstOrDefault(x => x.CodigoAluno == alunoDisciplina.CodigoAluno).NomeAluno,
                                                PercentualFrequencia = alunoDisciplina.PercentualFrequencia
                                            });
                                        }
                                    };

                                    var disciplinaEOL = disciplinasEol.FirstOrDefault(d => d.CodigoComponenteCurricular.ToString() == disciplinaId);
                                    foreach (var gestor in gestores)
                                    {
                                        NotificarFrequenciaBimestre(turma.CodigoTurma,
                                                            turma.Nome,
                                                            periodoAtual.Bimestre,
                                                            turmasUE.Key.CodigoUe,
                                                            turmasUE.Key.Nome,
                                                            turmasDRE.Key.CodigoDre,
                                                            turmasDRE.Key.Nome,
                                                            disciplinaEOL.Nome,
                                                            gestor.Id,
                                                            alunosDto);
                                    }
                                });
                        }
                    };
                };
            }
        }

        private void NotificarFrequenciaBimestre(string codigoTurma, string turma, int bimestre, string codigoUe, string ue, string codigoDre, string dre, string disciplina, long usuarioId, List<CompensacaoAusenciaAlunoQtdDto> alunos)
        {
            var tituloMensagem = $"Alunos com frequência irregular na turma {turma} ({bimestre}º Bimestre)";

            StringBuilder mensagemUsuario = new StringBuilder();
            mensagemUsuario.AppendLine($"<p>O(s) seguinte(s) aluno(s) da turma {turma} da {ue} ({dre}) estão com frequência irregular na disciplina {disciplina} no {bimestre}º Bimestre.</p>");

            mensagemUsuario.AppendLine("<table style='margin-left: auto; margin-right: auto;' border='2' cellpadding='5'>");
            mensagemUsuario.AppendLine("<tr>");
            mensagemUsuario.AppendLine("<td>Nº</td><td>Nome do aluno</td><td>Percentual de Frequência</td>");
            mensagemUsuario.AppendLine("</tr>");
            foreach (var aluno in alunos)
            {
                mensagemUsuario.AppendLine("<tr>");
                mensagemUsuario.Append($"<td>{aluno.CodigoAluno} </td><td> {aluno.NomeAluno} </td><td style='text-align: center;'> {aluno.PercentualFrequencia}</td>");
                mensagemUsuario.AppendLine("</tr>");
            }
            mensagemUsuario.AppendLine("</table>");

            var notificacao = new Notificacao()
            {
                Ano = DateTime.Now.Year,
                Categoria = NotificacaoCategoria.Alerta,
                Tipo = NotificacaoTipo.Frequencia,
                Titulo = tituloMensagem,
                Mensagem = mensagemUsuario.ToString(),
                UsuarioId = usuarioId,
                TurmaId = codigoTurma,
                UeId = codigoUe,
                DreId = codigoDre,
            };
            servicoNotificacao.Salvar(notificacao);
        }

        private long NotificarCompensacaoAusencia(long compensacaoId, Usuario usuario, string professor, string disciplina,
            string codigoTurma, string turma, string codigoUe, string escola, string codigoDre, string dre,
            int bimestre, string atividade, List<CompensacaoAusenciaAlunoQtdDto> alunos)
        {
            var tituloMensagem = $"Atividade de compensação da turma {turma}";

            StringBuilder mensagemUsuario = new StringBuilder();
            mensagemUsuario.AppendLine($"<p>A atividade de compensação '{atividade}' da disciplina de {disciplina} foi cadastrada para a turma {turma} da {escola} (DRE {dre}) no {bimestre}º Bimestre pelo professor {professor}.</p>");
            mensagemUsuario.AppendLine("<p>O(s) seguinte(s) aluno(s) foi(ram) vinculado(s) a atividade:</p>");

            mensagemUsuario.AppendLine("<table style='margin-left: auto; margin-right: auto;' border='2' cellpadding='5'>");
            mensagemUsuario.AppendLine("<tr>");
            mensagemUsuario.AppendLine("<td>Nº</td><td>Nome do aluno</td><td>Quantidade de aulas compensadas</td>");
            mensagemUsuario.AppendLine("</tr>");
            foreach (var aluno in alunos)
            {
                mensagemUsuario.AppendLine("<tr>");
                mensagemUsuario.Append($"<td>{aluno.CodigoAluno} </td><td> {aluno.NomeAluno} </td><td style='text-aign: center;'> {aluno.QuantidadeCompensacoes}</td>");
                mensagemUsuario.AppendLine("</tr>");
            }
            mensagemUsuario.AppendLine("</table>");

            var hostAplicacao = configuration["UrlFrontEnd"];
            mensagemUsuario.Append($"<a href='{hostAplicacao}/diario-classe/compensacao-ausencia/editar/{compensacaoId}'>Para consultar detalhes da atividade clique aqui.</a>");

            var notificacao = new Notificacao()
            {
                Ano = DateTime.Now.Year,
                Categoria = NotificacaoCategoria.Aviso,
                Tipo = NotificacaoTipo.Frequencia,
                Titulo = tituloMensagem,
                Mensagem = mensagemUsuario.ToString(),
                UsuarioId = usuario.Id,
                TurmaId = codigoTurma,
                UeId = codigoUe,
                DreId = codigoDre,
            };
            servicoNotificacao.Salvar(notificacao);
            return notificacao.Id;
        }

        private IEnumerable<Usuario> BuscaGestoresUe(string codigoUe)
        {
            // Buscar gestor da Ue
            List<UsuarioEolRetornoDto> funcionariosRetornoEol = new List<UsuarioEolRetornoDto>();

            var cps = servicoEOL.ObterFuncionariosPorCargoUe(codigoUe, (int)Cargo.CP);
            if (cps != null && cps.Any())
                funcionariosRetornoEol.AddRange(cps);
            var diretores = servicoEOL.ObterFuncionariosPorCargoUe(codigoUe, (int)Cargo.Diretor);
            if (diretores != null && diretores.Any())
                funcionariosRetornoEol.AddRange(diretores);

            var usuarios = new List<Usuario>();
            foreach (var usuarioEol in funcionariosRetornoEol)
                usuarios.Add(servicoUsuario.ObterUsuarioPorCodigoRfLoginOuAdiciona(usuarioEol.CodigoRf));

            return usuarios;
        }

        private IEnumerable<Usuario> BuscaProfessorAula(RegistroFrequenciaFaltanteDto turma)
        {
            // Buscar professor da ultima aula
            var professorRf = turma.Aulas
                    .OrderBy(o => o.DataAula)
                    .Last().ProfessorId;
            var usuario = servicoUsuario.ObterUsuarioPorCodigoRfLoginOuAdiciona(professorRf.ToString());

            return usuario != null ? new List<Usuario>()
            {
                usuario
            } : null;
        }

        private IEnumerable<Usuario> BuscaSupervisoresUe(string codigoUe)
        {
            // Buscar supervisor da Ue
            var supervisoresEscola = repositorioSupervisorEscolaDre.ObtemSupervisoresPorUe(codigoUe);
            if (supervisoresEscola == null || supervisoresEscola.Count() == 0)
                return null;

            var usuarios = new List<Usuario>();
            foreach (var supervisorEscola in supervisoresEscola)
                usuarios.Add(servicoUsuario.ObterUsuarioPorCodigoRfLoginOuAdiciona(supervisorEscola.SupervisorId));

            return usuarios;
        }

        private IEnumerable<Usuario> BuscaUsuarioNotificacao(RegistroFrequenciaFaltanteDto turma, TipoNotificacaoFrequencia tipo)
        {
            IEnumerable<Usuario> usuarios = Enumerable.Empty<Usuario>();
            switch (tipo)
            {
                case TipoNotificacaoFrequencia.Professor:
                    usuarios = BuscaProfessorAula(turma);
                    break;

                case TipoNotificacaoFrequencia.GestorUe:
                    usuarios = BuscaGestoresUe(turma.CodigoUe);
                    break;

                case TipoNotificacaoFrequencia.SupervisorUe:
                    usuarios = BuscaSupervisoresUe(turma.CodigoUe);
                    break;

                default:
                    usuarios = BuscaProfessorAula(turma);
                    break;
            }
            return usuarios;
        }

        private void NotificaAlteracaoFrequencia(Usuario usuario, RegistroFrequenciaAulaDto registroFrequencia, string usuarioAlteracao)
        {
            // carregar nomes da turma, escola, disciplina e professor para notificacao
            var disciplina = ObterNomeDisciplina(registroFrequencia.CodigoDisciplina);

            var tituloMensagem = $"Título: Alteração extemporânea de frequência  da turma {registroFrequencia.NomeTurma} na disciplina {disciplina}.";

            StringBuilder mensagemUsuario = new StringBuilder();
            mensagemUsuario.Append($"O Professor {usuarioAlteracao} realizou alterações no registro de frequência do dia {registroFrequencia.DataAula} da turma {registroFrequencia.NomeTurma} ({registroFrequencia.NomeUe}) na disciplina {disciplina}.");

            var hostAplicacao = configuration["UrlFrontEnd"];
            mensagemUsuario.Append($"<a href='{hostAplicacao}/diario-classe/frequencia-plano-aula'>Clique aqui para acessar esse registro.</a>");

            var notificacao = new Notificacao()
            {
                Ano = DateTime.Now.Year,
                Categoria = NotificacaoCategoria.Alerta,
                Tipo = NotificacaoTipo.Frequencia,
                Titulo = tituloMensagem,
                Mensagem = mensagemUsuario.ToString(),
                UsuarioId = usuario.Id,
                TurmaId = registroFrequencia.CodigoTurma,
                UeId = registroFrequencia.CodigoUe,
                DreId = registroFrequencia.CodigoDre,
            };
            servicoNotificacao.Salvar(notificacao);
        }

        private void NotificarAusenciaFrequencia(TipoNotificacaoFrequencia tipo)
        {
            // Busca registro de aula sem frequencia e sem notificação do tipo
            IEnumerable<RegistroFrequenciaFaltanteDto> turmasSemRegistro = null;
            turmasSemRegistro = repositorioNotificacaoFrequencia.ObterTurmasSemRegistroDeFrequencia(tipo);

            if (turmasSemRegistro != null)
            {
                // Busca parametro do sistema de quantidade de aulas sem frequencia para notificação
                var qtdAulasNotificacao = QuantidadeAulasParaNotificacao(tipo);
                foreach (var turma in turmasSemRegistro)
                {
                    // Carrega todas as aulas sem registro de frequencia da turma e disciplina para notificação
                    turma.Aulas = repositorioFrequencia.ObterAulasSemRegistroFrequencia(turma.CodigoTurma, turma.DisciplinaId, tipo);
                    if (turma.Aulas != null && turma.Aulas.Count() >= qtdAulasNotificacao)
                    {
                        // Busca Professor/Gestor/Supervisor da Turma ou Ue
                        var usuarios = BuscaUsuarioNotificacao(turma, tipo);

                        if (usuarios != null)
                            foreach (var usuario in usuarios)
                            {
                                NotificaRegistroFrequencia(usuario, turma, tipo);
                            }
                    }
                    else
                        Console.WriteLine($"Notificação não necessária pois quantidade de aulas sem frequência: {turma.Aulas?.Count() ?? 0 } está dentro do limite: {qtdAulasNotificacao}.");
                }
            }
        }

        private void NotificaRegistroFrequencia(Usuario usuario, RegistroFrequenciaFaltanteDto turmaSemRegistro, TipoNotificacaoFrequencia tipo)
        {
            var disciplinas = servicoEOL.ObterDisciplinasPorIds(new long[] { long.Parse(turmaSemRegistro.DisciplinaId) });
            if (disciplinas != null && disciplinas.Any())
            {
                var disciplina = disciplinas.FirstOrDefault();

                var tituloMensagem = $"Frequência da turma {turmaSemRegistro.NomeTurma} - {turmaSemRegistro.DisciplinaId} ({turmaSemRegistro.NomeUe})";
                StringBuilder mensagemUsuario = new StringBuilder();
                mensagemUsuario.Append($"A turma a seguir esta a <b>{turmaSemRegistro.Aulas.Count()} aulas</b> sem registro de frequência da turma");
                mensagemUsuario.Append("<br />");
                mensagemUsuario.Append($"<br />Escola: <b>{turmaSemRegistro.NomeUe}</b>");
                mensagemUsuario.Append($"<br />Turma: <b>{turmaSemRegistro.NomeTurma}</b>");
                mensagemUsuario.Append($"<br />Disciplina: <b>{disciplina.Nome}</b>");
                mensagemUsuario.Append($"<br />Aulas:");

                mensagemUsuario.Append("<ul>");
                foreach (var aula in turmaSemRegistro.Aulas)
                {
                    mensagemUsuario.Append($"<li>Data: {aula.DataAula}</li>");
                }
                mensagemUsuario.Append("</ul>");

                var hostAplicacao = configuration["UrlFrontEnd"];
                var parametros = $"turma={turmaSemRegistro.CodigoTurma}&DataAula={turmaSemRegistro.Aulas.FirstOrDefault().DataAula.ToShortDateString()}&disciplina={turmaSemRegistro.DisciplinaId}";
                mensagemUsuario.Append($"<a href='{hostAplicacao}diario-classe/frequencia-plano-aula?{parametros}'>Clique aqui para regularizar.</a>");

                var notificacao = new Notificacao()
                {
                    Ano = DateTime.Now.Year,
                    Categoria = NotificacaoCategoria.Alerta,
                    Tipo = NotificacaoTipo.Frequencia,
                    Titulo = tituloMensagem,
                    Mensagem = mensagemUsuario.ToString(),
                    UsuarioId = usuario.Id,
                    TurmaId = turmaSemRegistro.CodigoTurma,
                    UeId = turmaSemRegistro.CodigoUe,
                    DreId = turmaSemRegistro.CodigoDre,
                };
                servicoNotificacao.Salvar(notificacao);
                foreach (var aula in turmaSemRegistro.Aulas)
                {
                    repositorioNotificacaoFrequencia.Salvar(new NotificacaoFrequencia()
                    {
                        Tipo = tipo,
                        NotificacaoCodigo = notificacao.Codigo,
                        AulaId = aula.Id,
                        DisciplinaCodigo = turmaSemRegistro.DisciplinaId
                    });
                }
            }
            else
            {
                Console.WriteLine("Não foi possível obter a disciplina pois o EOL não respondeu");
                SentrySdk.CaptureEvent(new SentryEvent(new NegocioException("Não foi possível obter a disciplina pois o EOL não respondeu")));
            }
        }

        private string ObterNomeDisciplina(string codigoDisciplina)
        {
            long[] disciplinaId = { long.Parse(codigoDisciplina) };
            var disciplina = servicoEOL.ObterDisciplinasPorIds(disciplinaId);

            if (!disciplina.Any())
                throw new NegocioException("Disciplina não encontrada no EOL.");

            return disciplina.FirstOrDefault().Nome;
        }

        private int QuantidadeAulasParaNotificacao(TipoNotificacaoFrequencia tipo)
                            => int.Parse(repositorioParametrosSistema.ObterValorPorTipoEAno(
                                        tipo == TipoNotificacaoFrequencia.Professor ? TipoParametroSistema.QuantidadeAulasNotificarProfessor
                                            : tipo == TipoNotificacaoFrequencia.GestorUe ? TipoParametroSistema.QuantidadeAulasNotificarGestorUE
                                            : TipoParametroSistema.QuantidadeAulasNotificarSupervisorUE,
                                        DateTime.Now.Year)); 
        #endregion
    }
}