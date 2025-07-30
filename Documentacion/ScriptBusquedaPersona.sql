-- DROP FUNCTION public.buscar_personas(text);

CREATE OR REPLACE FUNCTION public.buscar_personas(criterio text)
 RETURNS TABLE(id_persona uuid, nombre character varying, primer_apellido character varying, segundo_apellido character varying, fecha_nacimiento date, numero_identificacion character varying, complemento character varying, genero integer, direccion text, telefono text, correo text, aud_estado integer, aud_usuario character varying, aud_fecha timestamp without time zone, aud_ip character varying)
 LANGUAGE plpgsql
AS $function$
DECLARE
    aux TEXT[];
BEGIN
    IF criterio ~ '^\d+$' THEN
        RETURN QUERY
        SELECT p.id_persona, p.nombre, p.primer_apellido, p.segundo_apellido, p.fecha_nacimiento,
			   p.numero_identificacion, p.complemento, p.genero, null as direccion, null as telefono, null as correo, p.aud_estado, p.aud_usuario, p.aud_fecha, p.aud_ip 
        FROM public.persona p
        WHERE p.numero_identificacion LIKE '%' || criterio || '%';
    ELSIF criterio ~ '^(0[1-9]|[12][0-9]|3[01])[-\/](0[1-9]|1[0-2])[-\/](\d{4})$' THEN
        RETURN QUERY
        SELECT p.id_persona, p.nombre, p.primer_apellido, p.segundo_apellido, p.fecha_nacimiento,
			   p.numero_identificacion, p.complemento, p.genero, null as direccion, null as telefono, null as correo, p.aud_estado, p.aud_usuario, p.aud_fecha, p.aud_ip
        FROM public.persona p
        WHERE p.fecha_nacimiento = CAST(criterio AS DATE);
    ELSE
        aux := string_to_array(criterio, ' ');

        RETURN QUERY
        SELECT p.id_persona, p.nombre, p.primer_apellido, p.segundo_apellido, p.fecha_nacimiento,
			   p.numero_identificacion, p.complemento, p.genero, null as direccion, null as telefono, null as correo, p.aud_estado, p.aud_usuario, p.aud_fecha, p.aud_ip
        FROM public.persona p
        WHERE (
            CASE array_length(aux, 1)
                WHEN 1 THEN normalizar_cadena(p.primer_apellido) LIKE '%' || normalizar_cadena(aux[1]) || '%'
                WHEN 2 THEN normalizar_cadena(p.nombre) LIKE '%' || normalizar_cadena(aux[1]) || '%' 
							AND normalizar_cadena(p.primer_apellido) LIKE '%' || normalizar_cadena(aux[2]) || '%'
                WHEN 3 THEN normalizar_cadena(p.nombre) LIKE '%' || normalizar_cadena(aux[1]) || '%' 
							AND normalizar_cadena(p.primer_apellido) LIKE '%' || normalizar_cadena(aux[2]) || '%' 
							AND normalizar_cadena(p.segundo_apellido) LIKE '%' || normalizar_cadena(aux[3]) || '%'
                WHEN 4 THEN ((normalizar_cadena(p.nombre) LIKE '%' || normalizar_cadena(aux[1]) || '%' 
							OR normalizar_cadena(p.nombre) LIKE '%' || normalizar_cadena(aux[2]) || '%') 
							AND normalizar_cadena(p.primer_apellido) LIKE '%' || normalizar_cadena(aux[3]) || '%' 
							AND normalizar_cadena(p.segundo_apellido) LIKE '%' || normalizar_cadena(aux[4]) || '%')
                ELSE normalizar_cadena(p.primer_apellido) LIKE '%' || normalizar_cadena(criterio) || '%'
            END
        );
    END IF;
END;
$function$
;

select * from public.buscar_personas('13-11-1968');
