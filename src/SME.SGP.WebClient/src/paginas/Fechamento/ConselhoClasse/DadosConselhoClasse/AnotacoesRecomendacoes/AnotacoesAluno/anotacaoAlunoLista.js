import * as moment from 'moment';
import PropTypes from 'prop-types';
import React from 'react';
import { Base } from '~/componentes';
import Editor from '~/componentes/editor/editor';
import { ListaAnotacao, Tabela } from './anotacaoAluno.css';

const AnotacoesAlunoLista = props => {
  const { anotacoes } = props;

  const cores = [
    Base.AzulCalendario,
    Base.LaranjaAlerta,
    Base.Laranja,
    Base.RosaCalendario,
    Base.Roxo,
    Base.Verde,
    Base.Vermelho,
    '#A7CDE0',
    '#2279AF',
    '#B1DF94',
    '#31A041',
    '#FC999C',
    '#E31426',
    '#FDBE7B',
    '#CAB2D4',
    '#6B3E93',
    '#FEFEA6',
    '#B15732',
    Base.AzulCalendario,
    Base.LaranjaAlerta,
    Base.Laranja,
    Base.RosaCalendario,
    Base.Roxo,
    Base.Verde,
    Base.Vermelho,
    '#A7CDE0',
    '#2279AF',
    '#B1DF94',
    '#31A041',
    '#FC999C',
    '#E31426',
    '#FDBE7B',
    '#CAB2D4',
    '#6B3E93',
    '#FEFEA6',
    '#B15732',
  ];

  return (
    <ListaAnotacao>
      {anotacoes && anotacoes.length ? (
        anotacoes.map((item, index) => {
          return (
            <Tabela
              className="table-responsive mb-3"
              style={{ borderLeftColor: `${cores[index]}` }}
              key={`anotacao-disciplina-${item.disciplina}`}
            >
              <table className="table">
                <thead>
                  <tr>
                    <th>{item.disciplina}</th>
                    <th>{item.professor}</th>
                    <th>RF={item.professorRf}</th>
                    <th>{item.data ? moment(item.data).format('L') : ''}</th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td colSpan="4">
                      <Editor
                        id="anotacao-aluno"
                        inicial={item.anotacao}
                        removerToolbar
                        desabilitar
                      />
                    </td>
                  </tr>
                </tbody>
              </table>
            </Tabela>
          );
        })
      ) : (
        <div className="col-md-12 text-center">Sem dados</div>
      )}
    </ListaAnotacao>
  );
};

AnotacoesAlunoLista.propTypes = {
  anotacoes: PropTypes.oneOfType([PropTypes.array]),
};

AnotacoesAlunoLista.defaultProps = {
  anotacoes: [],
};

export default AnotacoesAlunoLista;
