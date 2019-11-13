﻿using Sentry;
using SME.Background.Core.Enumerados;
using System;
using System.Linq.Expressions;

namespace SME.Background.Core
{
    public static class Cliente
    {
        public static string Executar(Expression<Action> metodo, TipoProcessamento tipoProcessamento = TipoProcessamento.ExecucaoLonga)
        {
            GravarLog($"Novo processamento background solicitado {metodo.Body.ToString()}");
            return Orquestrador.ObterProcessador(tipoProcessamento).Executar(metodo);
        }

        public static string Executar<T>(Expression<Action<T>> metodo, TipoProcessamento tipoProcessamento = TipoProcessamento.ExecucaoLonga)
        {
            GravarLog($"Novo processamento background solicitado {metodo.Body.ToString()}");
            return Orquestrador.ObterProcessador(tipoProcessamento).Executar<T>(metodo);
        }

        public static void ExecutarPeriodicamente(Expression<Action> metodo, string cron)
        {
            Orquestrador.ObterProcessador(TipoProcessamento.ExecucaoRecorrente).ExecutarPeriodicamente(metodo, cron);
        }
        
        static void GravarLog(string mensagem)
        {
            SentrySdk.AddBreadcrumb($"{mensagem} - {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}", "Background Processing");
            Console.WriteLine($"{mensagem} - {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
        }

    }
}
