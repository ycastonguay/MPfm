/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.00 --> 1.01

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:
   - Added a new setting called DatabaseVersion so the application can check
     when to update the database structure (and to run this script).
   - Replacing the LastPlayed column in the AudioFile table by LastPlayedDateTime (text instead of date).
     Unfortunately SQLite doesn't support removing columns, so the LastPlayed column is considered OBSOLETE (already!).
   - Added the History table, which will replace the PlayCount/LastPlayed
     columns of the AudioFiles table. This will enable deeper dynamic playlists
	 in the future.

--*/

INSERT INTO Settings 
(SettingId, SettingName, SettingValue)
VALUES
('c84dcec5-8aa5-468d-a154-cafc0492f77f', 'DatabaseVersion', '1.01')

/**/

ALTER TABLE AudioFiles ADD LastPlayedDateTime text NULL

/**/

CREATE TABLE [History] (
[HistoryId] varchar(50)  NOT NULL PRIMARY KEY,
[AudioFileId] varchar(50) NULL,
[EventDateTime] text  NULL
)
