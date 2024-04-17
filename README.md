- song files are stored locally in a folder, not in the db
- db entries only contain the filenames
- streaming part = fiecare melodie are un endpoint de la care se poate descarca de client (care o sa fie cached inainte de playback)
- see QuickstartSql for initial db setup, sample songs in AudioFiles

create appsettings.Development.json with (appsettings.json if launched with prod):
1. DefaultConnection = db connection string
2. AudioFilesAbsPath = absolute path to the folder containing the audio files in local pc

- open with swagger for endpoints

TODO: 
- transactions on queries for rollback

DONE: 
- service data validation
