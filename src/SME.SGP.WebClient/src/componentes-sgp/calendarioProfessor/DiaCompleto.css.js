import styled from 'styled-components';
import { Base } from '~/componentes/colors';
import Button from '~/componentes/button';

export const Div = styled.div`
  &.list-group-item-action:hover {
    background: transparent !important;
  }
`;

export const Evento = styled(Div)`
  &:hover {
    background: ${Base.Roxo};
    color: ${Base.Branco};
  }
`;

export const Botao = styled(Button)`
  ${Evento}:hover & {
    background: transparent !important;
    border-color: ${Base.Branco} !important;
    color: ${Base.Branco} !important;
  }
  &.zIndex {
    z-index: 2;
  }
`;

export const BotoesAuxiliaresEstilo = styled(Div)`
  align-items: right;
  display: flex;
  justify-content: flex-end;
  padding: 16px;
  padding-bottom: 0;
  width: 100%;
`;
