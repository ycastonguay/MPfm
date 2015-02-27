/*--
   MIGRATION SCRIPT FOR THE Sessions DATABASE
   Database version: 1.08 --> 1.09

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the SessionsGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Added new fields to Loops and Markers tables
   - Dropped Segments table; rolling back to simple loops after migrating to libssp_player
--*/

UPDATE Settings 
SET SettingValue = '1.09'
WHERE SettingName = 'DatabaseVersion'

/**/

ALTER TABLE [Loops] ADD [StartPositionMS] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL

/**/

ALTER TABLE [Loops] ADD [EndPositionMS] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL

/**/

ALTER TABLE [Loops] ADD [LengthMS] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL

/**/

ALTER TABLE [Markers] ADD [PositionMS] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL

/**/

DROP TABLE [Segments]
