/*
Analogiškas duotam TSQL kodui pgSQL;
Programa JSON failą duoda UTF16 (nes SQL Server nepalaiko UTF8, priešingai nei Postgres), tad reikia pakeisti encoding į UTF8 (nors ir notepade);
PostgreSQL taip pat formatuoto teksto neperskaitys, todėl į one-linerį reikia perdaryti.
*/

create temp table temp_json (values text) on commit drop;
copy temp_json from 'FILE_PATH_HERE' encoding 'utf8';

 
create temp table sauktiniai on commit drop as
select (values->>'pos')::int pos,
       (values->>'number')::varchar(100) as number,
       (values->>'name')::varchar(100) as name,
       (values->>'lastname')::varchar(100) as lastname,
       (values->>'bdate')::smallint as bdate,
	   (values->>'department')::varchar(50) as department,
       (values->>'info')::varchar(100) as info,
       (values->>'date')::date as date,
       (values->>'region')::varchar(50) as region,
       (values->>'regionNo')::smallint as regionNo
from   (
           select json_array_elements(replace(values,'\','\\')::json) as values 
           from   temp_json
       ) a; 


SELECT count(*) skaičius,
	bdate gimimo_data,
	cast(count(*) * 1.0 / (
			SELECT count(*)
			FROM sauktiniai
			) AS DECIMAL(10, 4)) pakviestų_procentas_bendras,
	sum(CASE 
			WHEN info = 'privalomoji karo tarnyba atidėta'
				THEN 1
			ELSE 0
			END) tarnyba_atidėta,
	coalesce(cast(1.0 * nullif((
					sum(CASE 
							WHEN info = 'privalomoji karo tarnyba atidėta'
								THEN 1
							ELSE 0
							END)
					), 0) / count(*) AS DECIMAL(10, 4)), 0) tarnyba_atidėta_procentas
FROM sauktiniai
GROUP BY bdate
ORDER BY gimimo_data

/* užklausos rezultato pvz.:
╔══════════╦═════════════╦═════════════════════════════╦═════════════════╦═══════════════════════════╗
║ skaičius ║ gimimo_data ║ pakviestų_procentas_bendras ║ tarnyba_atidėta ║ tarnyba_atidėta_procentas ║
╠══════════╬═════════════╬═════════════════════════════╬═════════════════╬═══════════════════════════╣
║      490 ║        1997 ║                      0.0439 ║             201 ║                    0.4102 ║
║     2589 ║        1998 ║                      0.2319 ║            1135 ║                    0.4384 ║
║     2626 ║        1999 ║                      0.2352 ║            1305 ║                     0.497 ║
║     2527 ║        2000 ║                      0.2263 ║            1291 ║                    0.5109 ║
║     2155 ║        2001 ║                       0.193 ║            1163 ║                    0.5397 ║
║      778 ║        2002 ║                      0.0697 ║             167 ║                    0.2147 ║
╚══════════╩═════════════╩═════════════════════════════╩═════════════════╩═══════════════════════════╝
*/
