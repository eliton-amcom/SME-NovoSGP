import React, { useState } from 'react';
import { Tabs } from 'antd';
import { ContainerTabsCard } from '~/componentes/tabs/tabs.css';
import ListaNotasConselho from './ListaNotasConselho/listaNotasConselho';
import ComponenteSemNota from './ComponenteSemNota/ComponenteSemNota';
import {
  EnriquecimentoCurricular,
  AtendimentoEducacional,
} from './ComponenteSemNota/mock-componente-sem-nota';
import { Base } from '~/componentes';
// import ListaNotasConselho from './ListaNotasConselho/ListaNotasConselho';

const DadosConselhoClasse = () => {
  const [abaAtiva, setAbaAtiva] = useState('1');
  const onChangeTab = chaveAba => {
    setAbaAtiva(chaveAba);
  };
  const { TabPane } = Tabs;
  return (
    <>
      <ContainerTabsCard
        type="card"
        onChange={onChangeTab}
        activeKey={abaAtiva}
        className="ant-tab-nav-20"
      >
        <TabPane tab="1º Bimestre" key="1">
          <ListaNotasConselho />
          <ComponenteSemNota
            dados={EnriquecimentoCurricular}
            nomeColunaComponente="Enriquecimento curricular"
            corBorda={Base.Azul}
          />
          <ComponenteSemNota
            dados={AtendimentoEducacional}
            nomeColunaComponente="Atendimento educacional especializado"
            corBorda={Base.RoxoEventoCalendario}
          />
        </TabPane>
        <TabPane tab="2º Bimestre" key="2">
          2
        </TabPane>
        <TabPane tab="3º Bimestre" key="3">
          3
        </TabPane>
        <TabPane tab="4º Bimestre" key="4">
          4
        </TabPane>
        <TabPane tab="Final" key="final">
          Final
        </TabPane>
      </ContainerTabsCard>
    </>
  );
};

export default DadosConselhoClasse;
