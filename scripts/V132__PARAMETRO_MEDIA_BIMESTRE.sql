insert into 
    public.parametros_sistema (nome,descricao,valor,ano,criado_em,criado_por, criado_rf,tipo)
select 'MediaBimestre','M�dia final para aprova��o no bimestre','5',null,now(),'Carga Inicial','Carga Inicial', 14
where  not exists(
	select 1
	from public.parametros_sistema
	where tipo = 14 );