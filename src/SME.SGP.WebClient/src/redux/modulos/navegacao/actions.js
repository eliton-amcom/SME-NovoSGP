export function menuCollapsed(collapsed) {
  return {
    type: '@navegacao/collapsed',
    payload: collapsed,
  };
}

export function activeRoute(route) {
  return {
    type: '@navegacao/activeRoute',
    payload: route,
  };
}

export function setRotas(routas) {
  return {
    type: '@navegacao/rotas',
    payload: routas,
  };
}
