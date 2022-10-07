use #dataBaseName# 

select
	p.ParticipantID
	,p.Surname + ' ' + p.Name + ' ' + p.SecondName as [ФИО]
	,p.DocumentSeries + ' ' + p.DocumentNumber
    ,DB_NAME(db_id()) as [Имя БД]
from
	rbd_Participants as p
	inner join res_HumanTests as ht on ht.ParticipantFK = p.ParticipantID
where
	p.DeleteType = 0
	and ht.ProcessCondition = 6
	and ht.SubjectCode < 50
    --and PartExam.[Экзамены участника] is not NULL
	and p.Surname like '%#surname#%'
	and p.Name like '%#name#%'
	and p.SecondName like '%#secondName#%'
	and p.DocumentSeries like '%#docSeries#%'
	and p.DocumentNumber like '%#docNumber#%'

group by
	p.ParticipantID
	,p.Surname + ' ' + p.Name + ' ' + p.SecondName
	,p.DocumentSeries + ' ' + p.DocumentNumber
    --,DB_NAME(db_id())

having count(distinct ht.HumanTestID) != 0