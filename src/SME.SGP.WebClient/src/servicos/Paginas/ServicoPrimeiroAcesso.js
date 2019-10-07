import api from '~/servicos/api';

class ServicoPrimeiroAcesso {
  alterarSenha = async dados => {
    return api
      .post('v1/autenticacao/primeiro-acesso', dados)
      .then(resposta => {
        return { sucesso: true, resposta };
      })
      .catch(erro => {
        if (!erro.response || !erro.response.data) {
          return {
            sucesso: false,
            erro: 'Falha de comunicação com o servidor.',
          };
        }
        return { sucesso: false, erro: erro.response.data.mensagens.join(',') };
      });
  };
}

export default new ServicoPrimeiroAcesso();
