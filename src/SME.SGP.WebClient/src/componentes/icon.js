import React from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';

export default function Icon(props) {

    const { icon, pack, color} = props;

    const Icone = styled.i`
        ${color ? `color: ${color}` : ""}
    `;

    return (
        <Icone className={`${pack} ${icon} `}/>
    );
}

Icon.defaultProps = {
    icon: "fa-check",
    pack: "fa",
    color: null
};

Icon.propTypes = {
    icon: PropTypes.string,
    pack: PropTypes.string,
    color: PropTypes.string
  };