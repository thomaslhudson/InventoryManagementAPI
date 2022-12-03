USE master;  
GO  
EXEC sp_who;  
GO  

/*
USE master
GO
KILL 61
GO
*/

/*
USE master;
GO
SELECT blocking_session_id, *
 FROM sys.dm_exec_requests 
 */