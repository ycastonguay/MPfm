/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.06 --> 1.07

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Added Segments table

   It is not possible to remove columns from a table in SQLite so the Loops table will be left as is.
--*/

UPDATE Settings 
SET SettingValue = '1.07'
WHERE SettingName = 'DatabaseVersion'

/**/

CREATE TABLE [Segments] (
[SegmentId] varchar(50)  NOT NULL PRIMARY KEY,
[LoopId] varchar(50)  NOT NULL,
[MarkerId] varchar(50)  NOT NULL,
[SegmentIndex] integer DEFAULT '0' NOT NULL,
[Position] varchar(50)  NULL,
[PositionBytes] integer DEFAULT '0' NULL,
[PositionSamples] integer DEFAULT '0' NULL
)
