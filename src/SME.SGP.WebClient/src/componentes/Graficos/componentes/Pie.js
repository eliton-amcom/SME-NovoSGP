import React from 'react';

import { ResponsivePie } from '@nivo/pie';

const Pie = ({ data }) => (
  <div style={{ height: 400 }}>
    <ResponsivePie
      padAngle={data.length > 3 ? 18 : 0}
      style={{ height: 400 }}
      radialLabelsLinkColor={{ from: 'color' }}
      radialLabel={item => item.radialLabel || item.value}
      radialLabelsLinkDiagonalLength={30}
      radialLabelsLinkStrokeWidth={1}
      enableSlicesLabels={false}
      data={data}
      margin={{ top: 40, right: 80, bottom: 40, left: 80 }}
      colors={d => d.color}
      theme={{
        fontSize: '14px',
        fontFamily: 'Roboto',
        fontWeight: 700,
        color: '#42474a',
      }}
      isInteractive={false}
    />
  </div>
);

export default Pie;
