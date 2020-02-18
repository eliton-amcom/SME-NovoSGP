import React, { useState, useEffect } from 'react';
import shortid from 'shortid';

// Redux
import { useSelector } from 'react-redux';

// Componentes SGP
import { Cabecalho } from '~/componentes-sgp';

// Componentes
import { Card, TabelaRetratil, ButtonGroup, Loader } from '~/componentes';

function PlanoDeTrabalho() {
  const [itensSelecionados, setItensSelecionados] = useState([]);
  const [itens, setItens] = useState([]);
  const [filtro, setFiltro] = useState({});
  const [somenteConsulta, setSomenteConsulta] = useState(false);
  const permissoesTela = useSelector(store => store.usuario.permissoes);

  const alunos = [
    {
      id: 1,
      idEol: 1,
      numeroChamada: 1,
      nome: 'Italo Gustavo Pereira de Maio',
      ativo: true,
    },
    {
      id: 2,
      idEol: 2,
      numeroChamada: 2,
      nome: 'Thiago de Oliveira Ramos',
      ativo: true,
    },
    {
      id: 3,
      idEol: 3,
      numeroChamada: 3,
      nome: 'Alana Ferreira de Oliveira',
      ativo: true,
    },
    {
      id: 4,
      idEol: 4,
      numeroChamada: 4,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 5,
      idEol: 5,
      numeroChamada: 5,
      nome: 'Alany Santos da Silva Sauro',
      ativo: false,
    },
    {
      id: 6,
      idEol: 6,
      numeroChamada: 6,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 7,
      idEol: 7,
      numeroChamada: 7,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 8,
      idEol: 8,
      numeroChamada: 8,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 9,
      idEol: 9,
      numeroChamada: 9,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 10,
      idEol: 10,
      numeroChamada: 10,
      nome: 'Alany Santos da Silva Sauro',
      ativo: false,
    },
    {
      id: 11,
      idEol: 11,
      numeroChamada: 11,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
    {
      id: 12,
      idEol: 12,
      numeroChamada: 12,
      nome: 'Alany Santos da Silva Sauro',
      ativo: true,
    },
  ];

  const onClickVoltar = () => null;

  const onClickBotaoPrincipal = () => null;

  const onClickEditar = item => null;

  return (
    <>
      <Cabecalho pagina="Relatório semestral" />
      <Loader loading={false}>
        <Card mx="mx-0">
          <ButtonGroup
            somenteConsulta={somenteConsulta}
            permissoesTela={
              permissoesTela[
                {
                  podeIncluir: true,
                  podeExcluir: true,
                  podeInserir: true,
                  podeAlterar: true,
                }
              ]
            }
            temItemSelecionado={
              itensSelecionados && itensSelecionados.length >= 1
            }
            onClickVoltar={onClickVoltar}
            onClickBotaoPrincipal={onClickBotaoPrincipal}
            labelBotaoPrincipal="Novo"
            desabilitarBotaoPrincipal={
              !!filtro.DreId === false && !!filtro.UeId === false
            }
          />
          <TabelaRetratil alunos={alunos} />
        </Card>
      </Loader>
    </>
  );
}

export default PlanoDeTrabalho;
