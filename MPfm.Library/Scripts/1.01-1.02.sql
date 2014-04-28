/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.01 --> 1.02

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:
   - Removed the PlaylistItems table
   - Added columns to the Playlists table

--*/

UPDATE Settings 
SET SettingValue = '1.02'
WHERE SettingName = 'DatabaseVersion'

/**/

DROP TABLE Playlists

/**/

DROP TABLE PlaylistItems

/**/

CREATE TABLE [PlaylistFiles] (
[FilePath] varchar(500)  NOT NULL PRIMARY KEY,
[Format] varchar(50)  NULL
)
