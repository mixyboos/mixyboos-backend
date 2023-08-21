USE
  master
GO
WHILE EXISTS(select NULL
             from sys.databases
             where name = 'MixyBoos')
  BEGIN
    DECLARE
      @SQL varchar(max)
    SELECT @SQL = COALESCE(@SQL, '') + 'Kill ' + Convert(varchar, SPId) + ';'
    FROM MASTER..SysProcesses
    WHERE DBId = DB_ID(N'MixyBoos')
      AND SPId <> @@SPId
    EXEC (@SQL)
    DROP
      DATABASE [MixyBoos]
  END
GO

CREATE
  DATABASE MixyBoos
GO
