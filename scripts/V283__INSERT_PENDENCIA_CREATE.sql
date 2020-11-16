create or replace PROCEDURE INSERIR_PENDENCIA(titulo varchar, descricao varchar, tipo int, aulaId int8, turmaId varchar, componenteId varchar) 
 language plpgsql
as $$
declare 
  pendenciaId int8;
begin
	START TRANSACTION;
	
    insert into pendencia(titulo, descricao, situacao, tipo, excluido, migrado, criado_em, criado_por, criado_rf, alterado_em, alterado_por, alterado_rf)
		  values (titulo, descricao, 1, tipo, false, false, NOW(), 'SISTEMA', 'SISTEMA', null, '', '')
		RETURNING id INTO pendenciaId;

	 update pendencia_aula
	        set pendencia_id = pendenciaId
	  from aula 
	  where aula.id = aulaId
	    and aula.turma_id = turmaId
	    and aula.disciplina_id = componenteId;
		
	commit;
end; $$;