import React, { useCallback, useEffect, useState } from 'react';
import shortid from 'shortid';
import { Loader, Base } from '~/componentes';
import Cabecalho from '~/componentes-sgp/cabecalho';
import Paginacao from '~/componentes-sgp/Paginacao/paginacao';
import Card from '~/componentes/card';
import CardCollapse from '~/componentes/cardCollapse';
import { erros } from '~/servicos';
import ServicoPendencias from '~/servicos/Paginas/ServicoPendencias';

const PendenciasGerais = () => {
  const [carregando, setCarregando] = useState(false);
  const [dadosPendencias, setDadosPendencias] = useState([]);
  const [numeroRegistros, setNumeroRegistros] = useState(0);

  const configCabecalho = {
    altura: '50px',
    corBorda: Base.Roxo,
  };

  const obterPendencias = useCallback(async paginaAtual => {
    setCarregando(true);
    const resposta = await ServicoPendencias.obterPendenciasListaPaginada(
      paginaAtual
    )
      .catch(e => erros(e))
      .finally(() => setCarregando(false));

    if (resposta?.data?.items) {
      setDadosPendencias(resposta.data);
      setNumeroRegistros(resposta.data.totalRegistros);
    } else {
      setDadosPendencias([]);
    }
  }, []);

  useEffect(() => {
    obterPendencias();
  }, [obterPendencias]);

  const titutoPersonalizado = item => {
    return (
      <div className="row pl-2">
        {item.tipo ? (
          <>
            <span style={{ color: Base.Roxo }}>Tipo:</span>
            {item.tipo}
            <span className="mr-3 ml-3">|</span>
          </>
        ) : (
          ''
        )}
        {item.turma ? (
          <>
            <span style={{ color: Base.Roxo }}>Turma:</span>
            {item.turma}
            <span className="mr-3 ml-3">|</span>
          </>
        ) : (
          ''
        )}
        {item.titulo ? (
          <>
            <span style={{ color: Base.Roxo }}>Título:</span>
            {item.titulo}
          </>
        ) : (
          ''
        )}
      </div>
    );
  };

  return (
    <Loader loading={carregando}>
      <Card className="mb-4 mt-4">
        <div className="col-md-12">
          <div className="col-md-12 pl-1 mb-3">
            <Cabecalho pagina="Pendências" />
          </div>
          <div className="col-md-12">
            {dadosPendencias?.items?.length ? (
              dadosPendencias.items.map(item => {
                return (
                  <div key={shortid.generate()} className="mb-3">
                    <CardCollapse
                      key={`pendencia-${shortid.generate()}-collapse-key`}
                      titulo={titutoPersonalizado(item)}
                      indice={`pendencia-${shortid.generate()}-collapse-indice`}
                      alt={`pendencia-${shortid.generate()}-alt`}
                      configCabecalho={configCabecalho}
                      styleCardBody={{ backgroundColor: Base.CinzaBadge }}
                    >
                      <span
                        dangerouslySetInnerHTML={{
                          __html: item.detalhe,
                        }}
                      />
                    </CardCollapse>
                  </div>
                );
              })
            ) : (
              <div className="text-center">Sem Pendências</div>
            )}
          </div>

          {dadosPendencias?.items?.length && numeroRegistros ? (
            <div className="col-md-12">
              <Paginacao
                numeroRegistros={numeroRegistros}
                onChangePaginacao={obterPendencias}
              />
            </div>
          ) : (
            ''
          )}
        </div>
      </Card>
    </Loader>
  );
};

export default PendenciasGerais;
