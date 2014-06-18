/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.05 --> 1.06

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Added columns to the AudioFile table

--*/

UPDATE Settings 
SET SettingValue = '1.06'
WHERE SettingName = 'DatabaseVersion'

/**/

CREATE TABLE [PlaylistItems] (
[PlaylistItemId] varchar(50)  NOT NULL PRIMARY KEY,
[PlaylistId] varchar(50)  NOT NULL,
[AudioFileId] varchar(50)  NOT NULL,
[Position] integer DEFAULT '0' NOT NULL
)

/**/

CREATE TABLE [Playlists] (
[PlaylistId] varchar(50)  UNIQUE NOT NULL,
[Name] varchar(500)  NULL,
[LastModified] date
)
