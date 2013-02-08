/*--
   MIGRATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.03 --> 1.04

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.

   Also, SQLite doesn't fully support ALTER TABLE.
   http://www.sqlite.org/omitted.html

   Changes:   
   - Nothing. This is only used to trigger a message box when running this script.

--*/

UPDATE Settings 
SET SettingValue = '1.04'
WHERE SettingName = 'DatabaseVersion'
