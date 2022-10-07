select
name
,substring(name, 6,3)
,substring(name, 14,2)
from
sys.databases
where substring(name, 6,3) = 'ege'
and name not like ('%_app%')
and name not like ('%_dt')