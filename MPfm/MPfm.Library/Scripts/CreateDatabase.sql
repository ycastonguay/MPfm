/*--
   CREATION SCRIPT FOR THE MPFM DATABASE
   Database version: 1.00

   SQLite doesn't support creating multiple tables in a single statement, so
   the statements need to be separated with /**/ so that the MPfmGateway can split
   the statements correctly.
--*/

CREATE TABLE [AudioFiles] (
[AudioFileId] varchar(50)  NOT NULL PRIMARY KEY,
[FilePath] varchar(3000)  NULL,
[FileType] varchar(10)  NULL,
[Title] varchar(5000)  NULL,
[ArtistName] varchar(3000)  NULL,
[AlbumTitle] varchar(3000)  NULL,
[DiscNumber] integer DEFAULT '0' NULL,
[TrackNumber] integer  NULL,
[TrackCount] integer  NULL,
[Year] varchar(50)  NULL,
[Genre] varchar(250)  NULL,
[Lyrics] text  NULL,
[PlayCount] integer  NULL,
[LastPlayed] date  NULL,
[Rating] integer  NULL,
[Tempo] integer  NULL,
[Length] varchar(50)  NULL,
[LengthBytes] INTEGER  NULL,
[LengthSamples] INTEGER  NULL
)

/**/

CREATE TABLE [EQPresets] (
[EQPresetId] varchar(50)  UNIQUE NOT NULL,
[Name] varchar(250)  NULL,
[Gain0] float(50)  NULL,
[Gain1] float(50)  NULL,
[Gain2] float(50)  NULL,
[Gain3] float(50)  NULL,
[Gain4] float(50)  NULL,
[Gain5] float(50)  NULL,
[Gain6] float(50)  NULL,
[Gain7] float(50)  NULL,
[Gain8] float(50)  NULL,
[Gain9] float(50)  NULL,
[Gain10] float(50)  NULL,
[Gain11] float(50)  NULL,
[Gain12] float(50)  NULL,
[Gain13] float(50)  NULL,
[Gain14] float(50)  NULL,
[Gain15] float(50)  NULL,
[Gain16] float(50)  NULL,
[Gain17] float(50)  NULL
)

/**/

CREATE TABLE [Folders] (
    [FolderId] varchar(50) PRIMARY KEY NOT NULL,
    [FolderPath] varchar(5000),
    [LastUpdated] date,
    [IsRecursive] boolean DEFAULT 0
)

/**/

CREATE TABLE [Loops] (
[LoopId] varchar(50)  PRIMARY KEY NOT NULL,
[AudioFileId] varchar(50)  NOT NULL,
[Name] varchar(500)  NULL,
[Comments] varchar(5000)  NULL,
[Length] varchar(50)  NULL,
[LengthBytes] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL,
[LengthSamples] INTEGER DEFAULT '''''''''''''''0''''''''''''''' NULL,
[StartPosition] VARCHAR(50)  NULL,
[StartPositionBytes] INTEGER DEFAULT '''''''0''''''' NULL,
[StartPositionSamples] INTEGER DEFAULT '''''''0''''''' NULL,
[EndPosition] varchar(50)  NULL,
[EndPositionBytes] integer DEFAULT '''0''' NULL,
[EndPositionSamples] INTEGER DEFAULT '''0''' NULL
)

/**/

CREATE TABLE [Markers] (
[MarkerId] varchar(50)  PRIMARY KEY NOT NULL,
[AudioFileId] varchar(50)  NULL,
[Name] varchar(500)  NULL,
[Comments] varchar(5000)  NULL,
[Position] VARCHAR(50)  NULL,
[PositionBytes] integer DEFAULT '''0''' NULL,
[PositionSamples] integer DEFAULT '''0''' NULL
)

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
[Name] varchar(500)  NULL
)

/**/

CREATE TABLE [Settings] (
  [SettingId]     varchar(50) PRIMARY KEY NOT NULL UNIQUE,
  [SettingName]  varchar(500),
  [SettingValue]  text
)