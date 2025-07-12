--DOCTOR TABLE SELECT ALL BY USERID PROCEDURE
CREATE OR ALTER PROCEDURE [DBO].[PR_Doctor_SelectAll]
AS
BEGIN
	
	SELECT	
				[DBO].[Doctor].[DoctorID],
				[DBO].[Doctor].[Name],
				[DBO].[Doctor].[Phone],
				[DBO].[Doctor].[Email],
				[DBO].[Doctor].[Qualification],
				[DBO].[Doctor].[Specialization],
				[DBO].[Doctor].[IsActive],
				[DBO].[Doctor].[Created],
				[DBO].[Doctor].[Modified]

	FROM [DBO].[Doctor]

END
	

--DOCTOR TABLE SELECT BY PK AND USERID PROCEDURE
CREATE OR ALTER PROCEDURE [DBO].[PR_Doctor_SelectByPK]

@DOCTORID				INT,
@USERID					INT

AS
BEGIN

		SELECT	
				[DBO].[User].[UserID],
				[DBO].[User].[UserName],
				[DBO].[Doctor].[DoctorID],
				[DBO].[Doctor].[Name],
				[DBO].[Doctor].[Phone],
				[DBO].[Doctor].[Email],
				[DBO].[Doctor].[Qualification],
				[DBO].[Doctor].[Specialization],
				[DBO].[Doctor].[IsActive],
				[DBO].[Doctor].[Created],
				[DBO].[Doctor].[Modified]

	FROM [DBO].[Doctor]

	INNER JOIN [DBO].[User]

	ON [DBO].[Doctor].[UserID] = [DBO].[User].[UserID]

	WHERE [DBO].[Doctor].[UserID] = @USERID OR [DBO].[User].[UserID] = @USERID
END


--DOCTOE TABLE INSERT TABLE PROCUDURE
CREATE OR ALTER PROCEDURE [DBO].[PR_Doctor_Insert]

@NAME				 NVARCHAR(100),
@PHONE				 NVARCHAR(20),
@EMAIL				 NVARCHAR(100),
@QUALIFICATION		 NVARCHAR(100),
@SPECIALIZATION		 NVARCHAR(100),
@ISACTIVE			 BIT, 
@CREATED			 DATETIME,
@MODIFIED			 DATETIME,
@USERID				 INT

AS
BEGIN
	INSERT INTO [DBO].[Doctor]
	(
			 [DBO].[Doctor].[Name]
			,[DBO].[Doctor].[Phone]
			,[DBO].[Doctor].[Email]
			,[DBO].[Doctor].[Qualification]
			,[DBO].[Doctor].[Specialization]
			,[DBO].[Doctor].[IsActive]
			,[DBO].[Doctor].[Created]
			,[DBO].[Doctor].[Modified]
			,[DBO].[Doctor].[UserID]
		
	)
	VALUES
	(
		@NAME,
		@PHONE,
		@EMAIL,
		@QUALIFICATION,
		@SPECIALIZATION,
		@ISACTIVE,
		@CREATED,
		@MODIFIED,
		@USERID
	)
END
	

--DOCTOR TABLE UPDATE BY PK PROCEDURE
CREATE OR ALTER PROCEDURE [DBO].[PR_Department_UpdateByPK]

@DOCTORID			 INT,
@NAME				 NVARCHAR(100),
@PHONE				 NVARCHAR(20),
@EMAIL				 NVARCHAR(100),
@QUALIFICATION		 NVARCHAR(100),
@SPECIALIZATION		 NVARCHAR(100),
@ISACTIVE			 BIT, 
@CREATED			 DATETIME,
@MODIFIED			 DATETIME,
@USERID				 INT

AS
BEGIN
	UPDATE [DBO].[Doctor]

	SET [DBO].[Doctor].[Name] = @NAME,
		[DBO].[Doctor].[Phone] = @PHONE,
		[DBO].[Doctor].[Email] = @EMAIL,
		[DBO].[Doctor].[Qualification] = @QUALIFICATION,
		[DBO].[Doctor].[Specialization] = @SPECIALIZATION,
		[DBO].[Doctor].[IsActive] = @ISACTIVE,
		[DBO].[Doctor].[Created] = @CREATED,
		[DBO].[Doctor].[Modified] = @MODIFIED,
		[DBO].[Doctor].[UserID] = @USERID

	WHERE [DBO].[Doctor].[DoctorID] = @DOCTORID
END


--DOCTOR TABLE DELETE BY PK PROCEDURE
CREATE OR  ALTER PROCEDURE [DBO].[PR_Doctor_DeleteByPK]

@DOCTORID INT

AS
BEGIN
	DELETE
	FROM [DBO].[Doctor]
	WHERE [DBO].[Doctor].[DoctorID] = @DOCTORID
END


exec PR_Doctor_SelectAll