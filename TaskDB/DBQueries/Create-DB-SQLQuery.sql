
Create Database MRTaskDB
on
 (
	NAME='MRTaskDB',
	FILENAME=N'D:\workTask\TaskDB\DBFiles\MRTaskDB.mdf',
	SIZE=2MB,
	FILEGROWTH=1MB
 )
LOG ON
 (
	NAME='MRTaskDB_log',
	FILENAME=N'D:\workTask\TaskDB\DBFiles\MRTaskDB_log.ldf',
	SIZE=2MB,
	FILEGROWTH=1MB
 );



----------Tables--------------------------
 use MRTaskDB
 go

 CREATE TABLE Sponsor
 (
	Code int,
	[Name] nvarchar(20),
	Authorized bit,
	CONSTRAINT SponsorPK Primary Key (code)
 );

 CREATE TABLE Person
 (
	ID int Identity(1,1),
	[Name] nvarchar(30) NOT NULL,
	UserName nvarchar(20) NOT NULL  UNIQUE,
	[Password] nvarchar(20) NOT NULL,
	CONSTRAINT PersonPK Primary Key (ID)
 )

 ALTER TABLE Sponsor Add CreatedByPersonID int,AuthorizedByPersonID int

 Alter Table Sponsor Add Constraint SponsorCreatedByPersonFK foreign key (CreatedByPersonID) references Person(ID)

 Alter Table Sponsor Add Constraint SponsorAuthorizedByPersonFK foreign key (AuthorizedByPersonID) references Person(ID)