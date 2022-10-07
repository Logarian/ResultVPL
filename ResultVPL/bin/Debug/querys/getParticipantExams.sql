use #dataBaseName#

select
	t0.ParticipantFK
	,t0.SubjectCode
	,t1.SubjectName
	,t1.Mark100
	,t1.MarkBorder
    ,DB_NAME(db_id()) as [Èìÿ ÁÄ]
from
	(
		select
			ht.ParticipantFK
			,max(ExamDate) as [ExamDate]
			,ht.SubjectCode
		from
			res_HumanTests as ht
		where 
			ht.SubjectCode not in (22)
		group by
			ht.ParticipantFK
			,ht.SubjectCode
	) as t0
		
	inner join (
		select
			ht.HumanTestID
			,ht.ExamDate
			,ht.SubjectCode
			,sub.SubjectName
			,ht.ParticipantFK
			,m.Mark100
			,mb.MarkBorder
			,m.Mark5
			,ht.ProcessCondition
			,p.DocumentSeries + ' ' + p.DocumentNumber as [SerNum]
		from
			rbd_Participants as p
			inner join res_HumanTests as ht on ht.ParticipantFK = p.ParticipantID
			inner join res_Marks as m on m.HumanTestID = ht.HumanTestID
			inner join dat_Exams as e on e.ExamDate = ht.ExamDate and e.SubjectCode = ht.SubjectCode
			inner join dat_Subjects as sub on sub.SubjectCode = ht.SubjectCode
			left join dat_MarkBorders as mb on mb.SubjectGlobalID = sub.SubjectGlobalID
	) as t1 on t1.ParticipantFK = t0.ParticipantFK and t1.ExamDate = t0.ExamDate and t1.SubjectCode = t0.SubjectCode

where 
	t0.ParticipantFK = '#participantID#'
	and t1.ProcessCondition = 6

order by 2