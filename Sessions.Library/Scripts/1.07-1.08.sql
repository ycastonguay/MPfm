/*--
   MIGRATION SCRIPT FOR THE Sessions DATABASE
   Database version: 1.07 --> 1.08

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the SessionsGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Added new fields to AudioFiles table

   It is not possible to remove columns from a table in SQLite so the Loops table will be left as is.
--*/

UPDATE Settings 
SET SettingValue = '1.08'
WHERE SettingName = 'DatabaseVersion'

/**/

ALTER TABLE [AudioFiles] ADD [StartPosition] varchar(20)  NULL

/**/

ALTER TABLE [AudioFiles] ADD [EndPosition] varchar(20)  NULL

/**/

ALTER TABLE [AudioFiles] ADD [CueFilePath] varchar(3000)  NULL
