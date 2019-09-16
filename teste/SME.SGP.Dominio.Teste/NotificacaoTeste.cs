using Xunit;

namespace SME.SGP.Dominio.Teste
{
    public class NotificacaoTeste
    {
        [Fact]
        public void DeveDispararExcecaoAoMarcarComoLidaNotificacaoDiferenteDeAlerta()
        {
            var notificacao = new Notificacao()
            {
                Categoria = NotificacaoCategoria.Aviso
            };
            Assert.Equal($"A notifica��o com id: '{notificacao.Id}' n�o pode ser marcada como lida ou j� est� nesse status.",
                Assert.Throws<NegocioException>(() => notificacao.MarcarComoLida()).Message);
            Assert.True(notificacao.Status == NotificacaoStatus.Pendente);
        }

        [Fact]
        public void DeveMarcarComoLida()
        {
            var notificacao = new Notificacao()
            {
                Categoria = NotificacaoCategoria.Alerta
            };
            notificacao.MarcarComoLida();
            Assert.True(notificacao.Status == NotificacaoStatus.Lida);
        }
    }
}