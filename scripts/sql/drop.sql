USE master;
GO
IF EXISTS(SELECT 1
          FROM sys.databases
          WHERE [name] = N'MixyBoos')
  BEGIN
    ALTER DATABASE [MixyBoos] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [MixyBoos];
  END;
GO