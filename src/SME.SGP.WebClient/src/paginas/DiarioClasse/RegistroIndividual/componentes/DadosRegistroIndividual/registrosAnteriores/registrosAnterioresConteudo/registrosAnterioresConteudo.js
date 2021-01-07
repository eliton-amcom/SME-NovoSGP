import React, { memo, useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';

import { CampoData, Label } from '~/componentes';
import { Paginacao } from '~/componentes-sgp';

import { erros, ServicoRegistroIndividual } from '~/servicos';

import {
  setDadosPrincipaisRegistroIndividual,
  setExibirLoaderGeralRegistroIndividual,
} from '~/redux/modulos/registroIndividual/actions';

import Item from './item/item';

const RegistrosAnterioresConteudo = memo(() => {
  const [dataInicio, setDataInicio] = useState();
  const [dataFim, setDataFim] = useState();

  const {
    componenteCurricularSelecionado,
    exibirLoaderGeralRegistroIndividual,
    dadosAlunoObjectCard,
    dadosPrincipaisRegistroIndividual,
  } = useSelector(store => store.registroIndividual);

  const { turmaSelecionada } = useSelector(state => state.usuario);
  const turmaCodigo = turmaSelecionada?.id || 0;

  const dispatch = useDispatch();

  const obterRegistroIndividualPorData = useCallback(
    async (dataFormatadaInicio, dataFormatadaFim, codigoEOL) => {
      dispatch(setExibirLoaderGeralRegistroIndividual(true));
      const retorno = await ServicoRegistroIndividual.obterRegistroIndividualPorPeriodo(
        {
          alunoCodigo: codigoEOL,
          componenteCurricular: componenteCurricularSelecionado,
          dataInicio: dataFormatadaInicio,
          dataFim: dataFormatadaFim,
          turmaCodigo,
        }
      )
        .catch(e => erros(e))
        .finally(() => dispatch(setExibirLoaderGeralRegistroIndividual(false)));

      if (retorno?.data) {
        dispatch(setDadosPrincipaisRegistroIndividual(retorno.data));
      }
    },
    [dispatch, componenteCurricularSelecionado, turmaCodigo]
  );

  console.log(
    'dadosPrincipaisRegistroIndividual ====>',
    dadosPrincipaisRegistroIndividual
  );

  useEffect(() => {
    const dataFormatadaInicio = dataInicio?.format('MM-DD-YYYY');
    const dataFormatadaFim = dataFim?.format('MM-DD-YYYY');
    const temDadosAlunos = Object.keys(dadosAlunoObjectCard).length;
    const { codigoEOL } = dadosAlunoObjectCard;
    const temDadosRegistros = Object.keys(dadosPrincipaisRegistroIndividual)
      .length;

    if (
      temDadosAlunos &&
      !temDadosRegistros &&
      !exibirLoaderGeralRegistroIndividual &&
      dataInicio &&
      dataFim
    ) {
      obterRegistroIndividualPorData(
        dataFormatadaInicio,
        dataFormatadaFim,
        codigoEOL
      );
    }
  }, [
    obterRegistroIndividualPorData,
    dadosAlunoObjectCard,
    dataInicio,
    dataFim,
    dadosPrincipaisRegistroIndividual,
    exibirLoaderGeralRegistroIndividual,
  ]);

  useEffect(() => {
    if (!dataInicio && dataFim) {
      const anoAtual = dataFim.format('YYYY');
      const anoLetivo = turmaSelecionada?.anoLetivo;
      const diferencaDias = dataFim.diff(`${anoAtual}-01-01`, 'days');

      if (Number(diferencaDias) > 60) {
        setDataInicio(window.moment().subtract(60, 'd'));
        return;
      }

      if (Number(anoLetivo) !== Number(anoAtual)) {
        setDataInicio(window.moment(`${anoLetivo}-01-01`));
        setDataFim(window.moment(`${anoLetivo}-12-31`));
        return;
      }

      setDataInicio(window.moment(`${anoAtual}-01-01`));
    }
  }, [dataInicio, dataFim, turmaSelecionada]);

  useEffect(() => {
    if (!dataFim) {
      setDataFim(window.moment());
    }
  }, [dataFim]);

  const onChangePaginacao = pagina => {
    console.log('pagina ===> ', pagina);
  };

  const onChangeNumeroLinhas = (paginaAtual, numeroLinhas) => {
    console.log('paginaAtual ===> ', paginaAtual);
    console.log('numeroLinhas ===> ', numeroLinhas);
  };

  return (
    <>
      <div className="px-3">
        <div className="row">
          <div className="col-12 pl-0">
            <Label text="Visualizar registros no período" />
          </div>
          <div className="col-3 p-0 pb-2 pr-3">
            <CampoData
              formatoData="DD/MM/YYYY"
              name="dataInicio"
              valor={dataInicio}
              onChange={data => setDataInicio(data)}
              placeholder="Data início"
            />
          </div>
          <div className="col-3 p-0 pb-2 mb-4">
            <CampoData
              formatoData="DD/MM/YYYY"
              name="dataFim"
              valor={dataFim}
              onChange={data => setDataFim(data)}
              placeholder="Data fim"
            />
          </div>
        </div>
        {dadosPrincipaisRegistroIndividual?.registrosIndividuais?.items.map(
          dados => (
            <Item key={`${dados.id}`} dados={dados} />
          )
        )}

        {dadosPrincipaisRegistroIndividual?.registrosIndividuais?.items
          .length && (
          <div className="row">
            <div className="col-12 d-flex justify-content-center mt-2">
              <Paginacao
                mostrarNumeroLinhas
                numeroRegistros={
                  dadosPrincipaisRegistroIndividual?.registrosIndividuais
                    ?.totalRegistros
                }
                onChangePaginacao={onChangePaginacao}
                onChangeNumeroLinhas={onChangeNumeroLinhas}
                pageSize={5}
                pageSizeOptions={['5', '10', '20', '50', '100']}
                locale={{ items_per_page: '' }}
              />
            </div>
          </div>
        )}
      </div>
    </>
  );
});

export default RegistrosAnterioresConteudo;
