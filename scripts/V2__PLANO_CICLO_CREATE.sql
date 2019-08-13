create table if not exists public.plano_ciclo ( descricao varchar not null,
id int8 not null generated always as identity,
ano int8 not null,
ciclo_id int8 not null,
escola_id int8 not null,
migrado boolean default false,
criado_em timestamp not null,
criado_por varchar (200) not null,
alterado_em timestamp null,
alterado_por varchar (200) null,
constraint plano_ciclo_pk primary key (id),
constraint plano_ciclo_un unique (ano,
ciclo_id,
escola_id) );

create table if not exists public.matriz_saber ( descricao varchar(100) not null,
id int8 not null generated always as identity,
criado_em timestamp not null,
criado_por varchar (200) not null,
alterado_em timestamp null,
alterado_por varchar (200) null,
constraint matriz_saber_pk primary key (id) );

create table if not exists public.matriz_saber_plano ( id int8 not null generated always as identity,
plano_id int8 not null,
matriz_id int8 not null,
criado_em timestamp not null,
criado_por varchar (200) not null,
alterado_em timestamp null,
alterado_por varchar (200) null,
constraint matriz_saber_plano_pk primary key (id),
constraint matriz_saber_plano_un unique (plano_id,
matriz_id) );

alter table if exists public.matriz_saber_plano drop constraint if exists matriz_id_fk;

alter table if exists public.matriz_saber_plano add constraint matriz_id_fk foreign key (matriz_id) references matriz_saber(id);

alter table if exists public.matriz_saber_plano drop constraint if exists plano_id_fk;

alter table if exists public.matriz_saber_plano add constraint plano_id_fk foreign key (plano_id) references plano_ciclo(id);

create table if not exists public.objetivo_desenvolvimento ( descricao varchar(100) not null,
id int8 not null generated always as identity,
criado_em timestamp not null,
criado_por varchar (200) not null,
alterado_em timestamp null,
alterado_por varchar (200) null,
constraint objetivo_desenvolvimento_pk primary key (id) );

create table if not exists public.objetivo_desenvolvimento_plano ( id int8 not null generated always as identity,
plano_id int8 not null,
objetivo_desenvolvimento_id int8 not null,
criado_em timestamp not null,
criado_por varchar (200) not null,
alterado_em timestamp null,
alterado_por varchar (200) null,
constraint objetivo_desenvolvimento_plano_pk primary key (id),
constraint objetivo_desenvolvimento_un unique (plano_id,
objetivo_desenvolvimento_id) );

alter table if exists public.objetivo_desenvolvimento_plano drop constraint if exists objetivo_desenvolvimento_id_fk;

alter table if exists public.objetivo_desenvolvimento_plano add constraint objetivo_desenvolvimento_id_fk foreign key (objetivo_desenvolvimento_id) references objetivo_desenvolvimento(id);

alter table if exists public.objetivo_desenvolvimento_plano drop constraint if exists plano_id_fk;

alter table if exists public.objetivo_desenvolvimento_plano add constraint plano_id_fk foreign key (plano_id) references plano_ciclo(id);

/*Inserts Matriz Saber
                    -----------------------
                    -----------------------
                    -----------------------
                    -----------------------
                    */
insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Pensamento Cient�fico, Cr�tico e Criativo',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Pensamento Cient�fico, Cr�tico e Criativo' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Resolu��o de Problemas',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Resolu��o de Problemas' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Comunica��o',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Comunica��o' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Autoconhecimento e Autocuidado',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Autoconhecimento e Autocuidado' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Autonomia e Determina��o',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Autonomia e Determina��o' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Abertura � Diversidade',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Abertura � Diversidade' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Resposabilidade e Participa��o',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Resposabilidade e Participa��o' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Empatia e Colabora��o',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Empatia e Colabora��o' );

insert
	into
	public.matriz_saber (descricao,criado_em,criado_por)
select
	'Repert�rio Cultural',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.matriz_saber
	where
		descricao = 'Repert�rio Cultural' );

/*Inserts Objetivos desenvolvimento
                    -----------------------
                    -----------------------
                    -----------------------
                    -----------------------
                    */
insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Erradica��o da Pobreza',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Erradica��o da Pobreza' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Fome zero e agricultura sustent�vel',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Fome zero e agricultura sustent�vel' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Sa�de e Bem Estar',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Sa�de e Bem Estar' );

;

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Educa��o de Qualidade',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Educa��o de Qualidade' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Igualdade de G�nero',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Igualdade de G�nero' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'�gua Pot�vel e Saneamento',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = '�gua Pot�vel e Saneamento' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Energia Limpa e Acess�vel',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Energia Limpa e Acess�vel' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Trabalho decente e crescimento econ�mico',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Trabalho decente e crescimento econ�mico' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Ind�stria, inova��o e infraestrutura',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Ind�stria, inova��o e infraestrutura' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Redu��o das desigualdades',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Redu��o das desigualdades' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Cidades e comunidades sustent�veis',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Cidades e comunidades sustent�veis' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Consumo e produ��o respons�veis',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Consumo e produ��o respons�veis' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'A��o contra a mudan�a global do clima',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'A��o contra a mudan�a global do clima' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Vida na �gua',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Vida na �gua' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Vida terrestre',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Vida terrestre' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Paz, justi�a e institui��es eficazes',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Paz, justi�a e institui��es eficazes' );

insert
	into
	public.objetivo_desenvolvimento (descricao,criado_em,criado_por)
select
	'Parcerias e meios de implementa��o',now(),'Carga Inicial'
where
	not exists(
	select
		1
	from
		public.objetivo_desenvolvimento
	where
		descricao = 'Parcerias e meios de implementa��o' );