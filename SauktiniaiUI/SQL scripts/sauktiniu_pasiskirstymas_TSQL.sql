SELECT count(*) Skaičius
	,bdate [Gimimo data]
	,cast(count(*) * 1.0 / (
			SELECT count(*)
			FROM [TABLE_NAME]
			) AS DECIMAL(10, 4)) [Pakviestų procentas bendras]
	,sum(iif(info = 'privalomoji karo tarnyba atidėta', 1, 0)) [Tarnyba atidėta]
	,isnull(cast(1.0 * nullif((sum(iif(info = 'privalomoji karo tarnyba atidėta', 1, 0))), 0) / count(*) AS DECIMAL(10, 4)), 0) [Tarnyba atidėta procentas]
FROM [TABLE_NAME]
GROUP BY bdate
ORDER BY [Gimimo data]