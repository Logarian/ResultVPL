use #dataBaseName# 

Declare @PartExam Table (ParticipantFK uniqueidentifier, SubjectCode int, ExamType int, Mark100 int)
insert into @PartExam
select
	t0.ParticipantFK
	,t0.SubjectCode
	,t1.ExamType
	,t1.Mark100
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
			,e.WaveCode
			,e.ExamType
			,ht.ParticipantFK
			,m.Mark100
			,m.Mark5
		from
			rbd_Participants as p
			inner join res_HumanTests as ht on ht.ParticipantFK = p.ParticipantID
			inner join res_Marks as m on m.HumanTestID = ht.HumanTestID
			inner join dat_Exams as e on e.ExamDate = ht.ExamDate and e.SubjectCode = ht.SubjectCode
	) as t1 on t1.ParticipantFK = t0.ParticipantFK and t1.ExamDate = t0.ExamDate and t1.SubjectCode = t0.SubjectCode


select
	p.ParticipantID
	,p.Surname + ' ' + p.Name + ' ' + p.SecondName as [‘»ќ]
	,PartExam.[Ёкзамены участника]
    ,DB_NAME(db_id()) as [»м€ Ѕƒ]
from
	rbd_Participants as p
inner join (
	select
		p.ParticipantID
		,stuff((
			select 
				'; ' + cast(tt.[PartExam] as varchar(max))
			from
			(
				select
					p_tt.ParticipantID
					,sub.SubjectName + ' (' + convert(varchar, partexam_tt.Mark100) + ')' as [PartExam]
				from
					rbd_Participants as p_tt
					inner join @PartExam as partexam_tt on partexam_tt.ParticipantFK = p_tt.ParticipantID
					inner join dat_Subjects as sub on sub.SubjectCode = partexam_tt.SubjectCode
			) as [tt]
			where 
				tt.ParticipantID = p.ParticipantID
			order by tt.[PartExam]
			for XML path('')
			),1,1,'') as [Ёкзамены участника]
	from
		rbd_Participants as p
	group by
		p.ParticipantID
	
) PartExam on PartExam.ParticipantID = p.ParticipantID

where
	p.DeleteType = 0
    and PartExam.[Ёкзамены участника] is not NULL
	and p.Surname like '%#surname#%'
	and p.Name like '%#name#%'
	and p.SecondName like '%#secondName#%'
	and p.DocumentSeries like '%#docSeries#%'
	and p.DocumentNumber like '%#docNumber#%'