CREATE PROCEDURE PromoteStudents (@StudiesName VARCHAR(100),@Semester INT)
AS
BEGIN
	BEGIN TRAN
	DECLARE @IdStudies INT = (SELECT IdStudy FROM Studies Where [Name] = @StudiesName);
	IF @IdStudies IS NULL
	BEGIN
		RAISERROR (15600,-1,-1, 'Studies Do not exists');  
	END

	DECLARE @oldEnrollment INT = (SELECT IdEnrollment From Enrollment Where IdStudy = @IdStudies AND Semester = @Semester);
	DECLARE @IdNextEnrollment INT = (SELECT IdEnrollment From Enrollment Where IdStudy = @IdStudies AND Semester = @Semester+1);
	DECLARE @current_Enroll_id INT = (Select Top 1 IdEnrollment from Enrollment Order By  IdEnrollment DESC);
	IF @IdNextEnrollment IS NULL
	BEGIN
		INSERT INTO Enrollment(IdEnrollment,IdStudy, Semester, StartDate) VALUES (@current_Enroll_id,@IdStudies,@Semester+1,CURRENT_TIMESTAMP);
	END

	UPDATE Student SET
	IdEnrollment = @IdNextEnrollment
	WHERE IdEnrollment = @oldEnrollment;

	COMMIT

END