create table public.pais(
	id_pais serial4 not null, --Codigo de pais ISO 3166-1 alfa-2
	nombre_pais varchar(200) not null, --Nombre del Pais
	abreviacion2 varchar(2) not null, --Codigo de pais ISO 3166-1 alfa-2
	abreviacion3 varchar(3) not null, --Codigo de pais ISO 3166-1 alfa-3
	
	aud_estado int4 default 0 not null,
	
	constraint pk_id_pais primary key (id_pais)
);

comment on column public.pais.id_pais is 'Codigo de pais ISO 3166-1 alfa-2';
comment on column public.pais.nombre_pais is 'Nombre del Pais';
comment on column public.pais.abreviacion2 is 'Codigo de pais iso 3166-1 alfa-2';
comment on column public.pais.abreviacion3 is 'Codigo de pais iso 3166-1 alfa-3';


create table public.departamento(
	id_departamento serial4 not null,
	id_pais int4 not null,--
	id_region_geografica int4 not null, --Parametro region geografica
	nombre_departamento varchar(50) not null, --Nombre del Departamento
	abrev_int char(1) not null, -- Abreviacion internacional iso 3166-2
	abrev_2 varchar(2) not null, --Abreviatura de dos caracteres
	abrev_3 varchar(3) not null, --Abreviatura de tres caracteres
	
	aud_estado int4 default 0 not null,
	constraint pk_id_departamento primary key (id_departamento),
	constraint fk_pais_departamento foreign key (id_pais) references public.pais(id_pais)
);

comment on column public.departamento.id_region_geografica is 'Parametro region geografica';
comment on column public.departamento.nombre_departamento is 'Nombre del departamento';
comment on column public.departamento.abrev_int is 'Abreviacion internaciona iso 3166-2';
comment on column public.departamento.abrev_2 is 'Abreviatura de dos caracteres';
comment on column public.departamento.abrev_3 is 'Abreviatura de tres caracteres';


create table public.provincia(
	id_provincia serial4 not null,
	id_provincia_aux int4 not null,
	id_departamento int4 not null,
	nombre_provincia varchar(150) not null,
	
	aud_estado int4 default 0 not null,
	constraint pk_id_provincia primary key (id_provincia),
	constraint fk_departamento_pronvincia foreign key (id_departamento) references public.departamento(id_departamento)
);
comment on column public.provincia.id_provincia_aux is 'Secuencial de la provincia dentro el departamento';
comment on column public.provincia.id_departamento is 'Departamento al que pertenece la provincia';
comment on column public.provincia.nombre_provincia is 'Nombre del Departamento';

create table public.municipio( 
	id_municipio serial4 not null,
	id_provincia int4 not null,
	nombre_municipio varchar(100) not null,
	seccion varchar(50) not null,
	
	aud_estado int4 default 0 not null,
	constraint pk_id_municipio primary key (id_municipio),
	constraint fk_provincia_municipio foreign key (id_provincia) references public.municipio(id_municipio)
);

comment on column public.municipio.id_provincia is 'Provincia a la que pertenece el municipio';
comment on column public.municipio.nombre_municipio is 'Nombre del Municipio';
comment on column public.municipio.seccion is 'Seccion municipal puede ser Seccion capital, primera, segunda ...';


create table public.localidad(
	id_localidad serial4 not null,
	id_provincia int4 not null,
	nombre_localidad varchar(100) not null,
	
	aud_estado int4 default 0 not null,
	
	constraint pk_id_localidad primary key (id_localidad),
	constraint fk_provincia_localidad foreign key (id_provincia) references public.provincia(id_provincia)
);

comment on table public.localidad is 'Es una region geografica que depende de una provincia';
comment on column public.localidad.id_provincia is 'Provincia a la que pertenece';
comment on column public.localidad.nombre_localidad is 'Nombre de la Localidad';