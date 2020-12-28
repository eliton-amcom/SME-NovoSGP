import React, { useState } from 'react';
import { useSelector } from 'react-redux';

import { Loader, DetalhesAluno } from '~/componentes';

import ServicoConselhoClasse from '~/servicos/Paginas/ConselhoClasse/ServicoConselhoClasse';
import { erro, sucesso } from '~/servicos';

const ObjectCardRegistroIndividual = () => {
  const [gerandoConselhoClasse, setGerandoConselhoClasse] = useState(false);

  const dadosAlunoObjectCard = useSelector(
    store => store.conselhoClasse.dadosAlunoObjectCard
  );
  const salvouJustificativa = useSelector(
    store => store.conselhoClasse.salvouJustificativa
  );
  const conselhoClasseAlunoId = useSelector(
    store =>
      store.conselhoClasse.dadosPrincipaisConselhoClasse.conselhoClasseAlunoId
  );
  const conselhoClasseId = useSelector(
    store => store.conselhoClasse.dadosPrincipaisConselhoClasse.conselhoClasseId
  );
  const fechamentoTurmaId = useSelector(
    store =>
      store.conselhoClasse.dadosPrincipaisConselhoClasse.fechamentoTurmaId
  );

  const gerarConselhoClasseAluno = async () => {
    setGerandoConselhoClasse(true);
    await ServicoConselhoClasse.gerarConselhoClasseAluno(
      conselhoClasseId,
      fechamentoTurmaId,
      dadosAlunoObjectCard.codigoEOL
    )
      .then(() => {
        sucesso(
          'Solicitação de geração do relatório gerada com sucesso. Em breve você receberá uma notificação com o resultado.'
        );
      })
      .finally(setGerandoConselhoClasse(false))
      .catch(e => erro(e));
  };

  return (
    <Loader loading={gerandoConselhoClasse}>
      <DetalhesAluno
        exibirResponsavel={false}
        exibirBotaoImprimir={false}
        exibirFrequencia={false}
        dados={dadosAlunoObjectCard}
        desabilitarImprimir={!salvouJustificativa && !conselhoClasseAlunoId}
        onClickImprimir={gerarConselhoClasseAluno}
      />
    </Loader>
  );
};

export default ObjectCardRegistroIndividual;
