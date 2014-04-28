/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.04 --> 1.05

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Added columns to the AudioFile table

--*/

ALTER TABLE [AudioFiles] ADD [FileSize] integer DEFAULT '0' NULL

/**/

UPDATE Settings 
SET SettingValue = '1.05'
WHERE SettingName = 'DatabaseVersion'
