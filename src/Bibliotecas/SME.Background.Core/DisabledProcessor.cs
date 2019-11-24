﻿using SME.Background.Core.Interfaces;
using System;
using System.Linq.Expressions;

namespace SME.Background.Core
{
    public class DisabledProcessor : IProcessor
    {
        private readonly IServiceProvider provider;

        public DisabledProcessor(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public string Executar(Expression<Action> metodo)
        {
            var acao = metodo.Compile();
            acao.Invoke();

            return string.Empty;
        }

        public string Executar<T>(Expression<Action<T>> metodo)
        {
            var classe = (T)provider.GetService(typeof(T));
            var acao = metodo.Compile();
            acao(classe);

            return string.Empty;
        }

        public void ExecutarPeriodicamente(Expression<Action> metodo, string cron)
        {
            throw new Exception("O serviço de processamento em segundo plano está desativado");
        }

        public void ExecutarPeriodicamente<T>(Expression<Action<T>> metodo, string cron)
        {
            throw new Exception("O serviço de processamento em segundo plano está desativado");
        }

        public void Registrar()
        {
            // Não há nada para fazer aqui
        }
    }
}