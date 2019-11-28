﻿using SME.Background.Core.Exceptions;
using SME.Background.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SME.Background.Core.Processors
{
    public class DisabledProcessor : IProcessor
    {
        public bool Registrado => true;

        public string Executar(Expression<Action> metodo)
        {
            var acao = metodo.Compile();
            acao.Invoke();

            return string.Empty;
        }

        public string Executar<T>(Expression<Action<T>> metodo)
        {
            var classe = (T)Orquestrador.Provider.GetService(typeof(T));
            var acao = metodo.Compile();
            acao(classe);

            return string.Empty;
        }

        public void ExecutarPeriodicamente(Expression<Action> metodo, string cron)
        {
            throw new ExcecaoServicoDesativado("Não é possível realizar novos processamentos periódicos pois o serviço de processamento em segundo plano está desativado");
        }

        public void Registrar()
        {
            // Não há nada para fazer aqui
        }
    }
}
