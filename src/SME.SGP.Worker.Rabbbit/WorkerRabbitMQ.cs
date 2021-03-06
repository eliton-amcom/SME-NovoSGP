﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentry;
using Sentry.Protocol;
using SME.SGP.Aplicacao;
using SME.SGP.Aplicacao.Interfaces;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using SME.SGP.Infra.Contexto;
using SME.SGP.Infra.Excecoes;
using SME.SGP.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Worker.RabbitMQ
{
    public class WorkerRabbitMQ : IHostedService
    {
        private readonly IModel canalRabbit;
        private readonly string sentryDSN;
        private readonly IConnection conexaoRabbit;
        private readonly IServiceScopeFactory serviceScopeFactory;

        /// <summary>
        /// configuração da lista de tipos para a fila do rabbit instanciar, seguindo a ordem de propriedades:
        /// rota do rabbit, usaMediatr?, tipo
        /// </summary>
        private readonly Dictionary<string, ComandoRabbit> comandos;


        public WorkerRabbitMQ(IConnection conexaoRabbit, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            sentryDSN = configuration.GetValue<string>("Sentry:DSN");
            this.conexaoRabbit = conexaoRabbit ?? throw new ArgumentNullException(nameof(conexaoRabbit));
            this.serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            canalRabbit = conexaoRabbit.CreateModel();

            canalRabbit.ExchangeDeclare(RotasRabbit.ExchangeServidorRelatorios, ExchangeType.Topic);
            canalRabbit.QueueDeclare(RotasRabbit.FilaSgp, false, false, false, null);
            canalRabbit.QueueBind(RotasRabbit.FilaSgp, RotasRabbit.ExchangeServidorRelatorios, "*", null);            
            canalRabbit.QueueDeclare(RotasRabbit.WorkerRelatoriosSgp, false, false, false, null);
            canalRabbit.QueueBind(RotasRabbit.WorkerRelatoriosSgp, RotasRabbit.ExchangeServidorRelatorios, "*", null);

            comandos = new Dictionary<string, ComandoRabbit>();
            RegistrarUseCases();
        }

        private void RegistrarUseCases()
        {
            comandos.Add(RotasRabbit.RotaRelatoriosProntos, new ComandoRabbit("Receber dados do relatório", typeof(IReceberRelatorioProntoUseCase)));
            comandos.Add(RotasRabbit.RotaInserirAulaRecorrencia, new ComandoRabbit("Inserir aulas recorrentes", typeof(IInserirAulaRecorrenteUseCase)));
            comandos.Add(RotasRabbit.RotaAlterarAulaRecorrencia, new ComandoRabbit("Alterar aulas recorrentes", typeof(IAlterarAulaRecorrenteUseCase)));
            comandos.Add(RotasRabbit.RotaExcluirAulaRecorrencia, new ComandoRabbit("Excluir aulas recorrentes", typeof(IExcluirAulaRecorrenteUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoUsuario, new ComandoRabbit("Notificar usuário", typeof(INotificarUsuarioUseCase)));
            comandos.Add(RotasRabbit.RotaRelatorioComErro, new ComandoRabbit("Notificar relatório com erro", typeof(IReceberRelatorioComErroUseCase)));
            comandos.Add(RotasRabbit.RotaRelatorioCorrelacaoCopiar, new ComandoRabbit("Copiar e gerar novo código de correlação", typeof(ICopiarCodigoCorrelacaoUseCase)));
            comandos.Add(RotasRabbit.RotaRelatorioCorrelacaoInserir, new ComandoRabbit("Inserir novo código de correlação", typeof(IInserirCodigoCorrelacaoUseCase)));
            comandos.Add(RotasRabbit.RotaSincronizarAulasInfatil, new ComandoRabbit("Sincronizar aulas da modalidade Infantil que devem ser criadas ou excluídas", typeof(ICriarAulasInfantilAutomaticamenteUseCase)));
            comandos.Add(RotasRabbit.RotaCriarAulasInfatilAutomaticamente, new ComandoRabbit("Criar aulas da modalidade Infantil automaticamente", typeof(ICriarAulasInfantilUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoExclusaoAulasComFrequencia, new ComandoRabbit("Notificar usuário sobre a exclusão de aulas com frequência registrada", typeof(INotificarExclusaoAulaComFrequenciaUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoNovaObservacaoDiarioBordo, new ComandoRabbit("Notificar usuário sobre nova observação no diário de bordo", typeof(INotificarDiarioBordoObservacaoUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoAlterarObservacaoDiarioBordo, new ComandoRabbit("Alterar as notificações dos usuários excluídos das observação no diário de bordo", typeof(IAlterarNotificacaoObservacaoDiarioBordoUseCase)));
            comandos.Add(RotasRabbit.RotaNovaNotificacaoObservacaoCartaIntencoes, new ComandoRabbit("Notificar usuário sobre nova observação na carta de intenções", typeof(ISalvarNotificacaoCartaIntencoesObservacaoUseCase)));
            comandos.Add(RotasRabbit.RotaExcluirNotificacaoObservacaoCartaIntencoes, new ComandoRabbit("Excluir uma notificação sobre observação na carta de intenções", typeof(IExcluirNotificacaoCartaIntencoesObservacaoUseCase)));
            comandos.Add(RotasRabbit.RotaNovaNotificacaoDevolutiva, new ComandoRabbit("Notificar usuário sobre a criação de uma devolutiva", typeof(ISalvarNotificacaoDevolutivaUseCase)));
            comandos.Add(RotasRabbit.RotaExcluirNotificacaoDevolutiva, new ComandoRabbit("Excluir uma notificação sobre devolutiva", typeof(IExcluirNotificacaoDevolutivaUseCase)));
            comandos.Add(RotasRabbit.RotaExcluirNotificacaoDiarioBordo, new ComandoRabbit("Excluir uma notificação do diario de bordo", typeof(IExcluirNotificacaoDiarioBordoUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaPendenciasAula, new ComandoRabbit("Verifica as pendências de aula e cria caso exista", typeof(IPendenciaAulaUseCase)));
            comandos.Add(RotasRabbit.RotaSincronizaComponetesCurricularesEol, new ComandoRabbit("Sincroniza os compoentes curriculares com o Eol", typeof(ISincronizarComponentesCurricularesUseCase)));
            comandos.Add(RotasRabbit.RotaCalculoFrequenciaPorTurmaComponente, new ComandoRabbit("Cálculo de frequência por Turma e Componente", typeof(ICalculoFrequenciaTurmaDisciplinaUseCase)));

            comandos.Add(RotasRabbit.RotaExecutaVerificacaoPendenciasGerais, new ComandoRabbit("Executa verficação das pendências gerais", typeof(IExecutaVerificacaoPendenciasGeraisUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaExclusaoPendenciasAula, new ComandoRabbit("Executa exclusão de pendências da aula", typeof(IExecutarExclusaoPendenciasAulaUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaExclusaoPendenciasDiasLetivosInsuficientes, new ComandoRabbit("Executa exclusão de pendências de dias letivos insuficientes", typeof(IExecutarExclusaoPendenciaDiasLetivosInsuficientes)));
            comandos.Add(RotasRabbit.RotaExecutaExclusaoPendenciaParametroEvento, new ComandoRabbit("Executa exclusão de pendências de eventos por parâmetro", typeof(IExecutarExclusaoPendenciaParametroEvento)));
            comandos.Add(RotasRabbit.RotaExecutaExclusaoPendenciasAusenciaAvaliacao, new ComandoRabbit("Executa exclusão de pendências de ausencia de avaliação", typeof(IExecutarExclusaoPendenciasAusenciaAvaliacaoUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaVerificacaoPendenciasProfessor, new ComandoRabbit("Executa verificação de pendências de avaliação do professor", typeof(IExecutaVerificacaoGeracaoPendenciaProfessorAvaliacaoUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaVerificacaoPendenciasAusenciaFechamento, new ComandoRabbit("Executa verificação de pendências de fechamento de bimestre", typeof(IExecutaVerificacaoGeracaoPendenciaAusenciaFechamentoUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaExclusaoPendenciasAusenciaFechamento, new ComandoRabbit("Executa exclusão de pendências de ausencia de fechamento", typeof(IExecutarExclusaoPendenciasAusenciaFechamentoUseCase)));

            comandos.Add(RotasRabbit.RotaNotificacaoResultadoInsatisfatorio, new ComandoRabbit("Notificar usuário resultado insatisfatório de aluno", typeof(INotificarResultadoInsatisfatorioUseCase)));
            comandos.Add(RotasRabbit.RotaExecutaAtualizacaoSituacaoConselhoClasse, new ComandoRabbit("Executa atualização da situação do conselho de classe", typeof(IAtualizarSituacaoConselhoClasseUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoAndamentoFechamento, new ComandoRabbit("Executa notificação sobre o andamento do fechamento", typeof(INotificacaoAndamentoFechamentoUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoUeFechamentosInsuficientes, new ComandoRabbit("Executa notificação UE sobre fechamento insuficientes", typeof(INotificacaoUeFechamentosInsuficientesUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoReuniaoPedagogica, new ComandoRabbit("Executa notificação sobre o andamento do fechamento", typeof(INotificacaoReuniaoPedagogicaUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoPeriodoFechamento, new ComandoRabbit("Executa notificação sobre período de fechamento", typeof(INotificacaoPeriodoFechamentoUseCase)));

            comandos.Add(RotasRabbit.RotaNotificacaoInicioFimPeriodoFechamento, new ComandoRabbit("Executa notificação sobre o início e fim do Periodo de fechamento", typeof(INotificacaoInicioFimPeriodoFechamentoUseCase)));
            comandos.Add(RotasRabbit.RotaNotificacaoFrequenciaUe, new ComandoRabbit("Notificar frequências dos alunos no bimestre para UE", typeof(INotificacaoFrequenciaUeUseCase)));
            
            comandos.Add(RotasRabbit.RotaTrataNotificacoesNiveis, new ComandoRabbit("Trata Níveis e Cargos das notificações aguardando ação", typeof(ITrataNotificacoesNiveisCargosUseCase)));
            comandos.Add(RotasRabbit.RotaPendenciaAusenciaRegistroIndividual, new ComandoRabbit("Gerar as pendências por ausência de registro individual", typeof(IGerarPendenciaAusenciaRegistroIndividualUseCase)));
            comandos.Add(RotasRabbit.RotaAtualizarPendenciaAusenciaRegistroIndividual, new ComandoRabbit("Atualizar pendência por ausência de registro individual", typeof(IAtualizarPendenciaRegistroIndividualUseCase)));
        }

        private async Task TratarMensagem(BasicDeliverEventArgs ea)
        {
            var mensagem = Encoding.UTF8.GetString(ea.Body.Span);
            var rota = ea.RoutingKey;
            if (comandos.ContainsKey(rota))
            {
                using (SentrySdk.Init(sentryDSN))
                {
                    var mensagemRabbit = JsonConvert.DeserializeObject<MensagemRabbit>(mensagem);
                    SentrySdk.AddBreadcrumb($"Dados: {mensagemRabbit.Mensagem}");
                    var comandoRabbit = comandos[rota];
                    try
                    {
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            AtribuirContextoAplicacao(mensagemRabbit, scope);

                            SentrySdk.CaptureMessage($"{mensagemRabbit.UsuarioLogadoRF} - {mensagemRabbit.CodigoCorrelacao.ToString().Substring(0, 3)} - EXECUTANDO - {ea.RoutingKey} - {DateTime.Now:dd/MM/yyyy HH:mm:ss}", SentryLevel.Debug);
                            var casoDeUso = scope.ServiceProvider.GetService(comandoRabbit.TipoCasoUso);

                            await ObterMetodo(comandoRabbit.TipoCasoUso, "Executar").InvokeAsync(casoDeUso, new object[] { mensagemRabbit });

                            SentrySdk.CaptureMessage($"{mensagemRabbit.UsuarioLogadoRF} - {mensagemRabbit.CodigoCorrelacao.ToString().Substring(0, 3)} - SUCESSO - {ea.RoutingKey}", SentryLevel.Info);
                            canalRabbit.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                    catch (NegocioException nex)
                    {
                        canalRabbit.BasicReject(ea.DeliveryTag, false);
                        SentrySdk.AddBreadcrumb($"Erros: {nex.Message}");
                        SentrySdk.CaptureException(nex);
                        RegistrarSentry(ea, mensagemRabbit, nex);
                        if (mensagemRabbit.NotificarErroUsuario)
                            NotificarErroUsuario(nex.Message, mensagemRabbit.UsuarioLogadoRF, comandoRabbit.NomeProcesso);
                    }
                    catch (ValidacaoException vex)
                    {
                        canalRabbit.BasicReject(ea.DeliveryTag, false);
                        SentrySdk.AddBreadcrumb($"Erros: {JsonConvert.SerializeObject(vex.Mensagens())}");
                        SentrySdk.CaptureException(vex);
                        RegistrarSentry(ea, mensagemRabbit, vex);
                        if (mensagemRabbit.NotificarErroUsuario)
                            NotificarErroUsuario($"Ocorreu um erro interno, por favor tente novamente", mensagemRabbit.UsuarioLogadoRF, comandoRabbit.NomeProcesso);
                    }
                    catch (Exception ex)
                    {
                        canalRabbit.BasicReject(ea.DeliveryTag, false);
                        SentrySdk.AddBreadcrumb($"Erros: {ex.Message}");
                        SentrySdk.CaptureException(ex);
                        RegistrarSentry(ea, mensagemRabbit, ex);
                        if (mensagemRabbit.NotificarErroUsuario)
                            NotificarErroUsuario($"Ocorreu um erro interno, por favor tente novamente", mensagemRabbit.UsuarioLogadoRF, comandoRabbit.NomeProcesso);
                    }
                }
            }
            else
                canalRabbit.BasicReject(ea.DeliveryTag, false);
        }

        private void RegistrarSentry(BasicDeliverEventArgs ea, MensagemRabbit mensagemRabbit, Exception ex)
        {
            SentrySdk.CaptureMessage($"{mensagemRabbit.UsuarioLogadoRF} - {mensagemRabbit.CodigoCorrelacao.ToString().Substring(0, 3)} - ERRO - {ea.RoutingKey}", SentryLevel.Error);
            SentrySdk.CaptureException(ex);
        }

        private static void AtribuirContextoAplicacao(MensagemRabbit mensagemRabbit, IServiceScope scope)
        {
            if (!string.IsNullOrWhiteSpace(mensagemRabbit.UsuarioLogadoRF))
            {
                var contextoAplicacao = scope.ServiceProvider.GetService<IContextoAplicacao>();
                var variaveis = new Dictionary<string, object>();
                variaveis.Add("NomeUsuario", mensagemRabbit.UsuarioLogadoNomeCompleto);
                variaveis.Add("UsuarioLogado", mensagemRabbit.UsuarioLogadoRF);
                variaveis.Add("RF", mensagemRabbit.UsuarioLogadoRF);
                variaveis.Add("login", mensagemRabbit.UsuarioLogadoRF);
                variaveis.Add("Claims", new List<InternalClaim> { new InternalClaim { Value = mensagemRabbit.PerfilUsuario, Type = "perfil" } });
                contextoAplicacao.AdicionarVariaveis(variaveis);
            }
        }

        private void NotificarErroUsuario(string message, string usuarioRf, string nomeProcesso)
        {
            if (!string.IsNullOrEmpty(usuarioRf))
            {
                var command = new NotificarUsuarioCommand($"Ocorreu um erro ao: '{nomeProcesso}'",
                                                          message,
                                                          usuarioRf,
                                                          NotificacaoCategoria.Aviso,
                                                          NotificacaoTipo.Worker);

                var request = new MensagemRabbit(string.Empty, command, Guid.NewGuid(), usuarioRf);
                var mensagem = JsonConvert.SerializeObject(request);
                var body = Encoding.UTF8.GetBytes(mensagem);

                canalRabbit.QueueBind(RotasRabbit.FilaSgp, RotasRabbit.ExchangeSgp, RotasRabbit.RotaNotificacaoUsuario);
                canalRabbit.BasicPublish(RotasRabbit.ExchangeSgp, RotasRabbit.RotaNotificacaoUsuario, null, body);
            }
        }

        private MethodInfo ObterMetodo(Type objType, string method)
        {
            var executar = objType.GetMethod(method);

            if (executar == null)
            {
                foreach (var itf in objType.GetInterfaces())
                {
                    executar = ObterMetodo(itf, method);
                    if (executar != null)
                        break;
                }
            }

            return executar;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            canalRabbit.Close();
            conexaoRabbit.Close();
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(canalRabbit);
            consumer.Received += async (ch, ea) =>
            {

                await TratarMensagem(ea);
            };

            canalRabbit.BasicConsume(RotasRabbit.FilaSgp, false, consumer);
            return Task.CompletedTask;
        }
    }
}
