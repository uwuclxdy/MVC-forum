1. kloniraj repo
2. con string (appsettings.json) default je:
   db - MVCforum,
   User Id oz login - MVCLogin,
   Pass - Forum123
4. update database

notes to self:
1. create server and db https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-ubuntu?view=sql-server-ver16&tabs=ubuntu2204
2. create login - MVClogin (pass: Forum123)
3. create user:
USE [MVCforum]
GO
CREATE USER [MVCuser] FOR LOGIN [MVClogin] WITH DEFAULT_SCHEMA=[db_owner]
GO
https://www.guru99.com/sql-server-create-user.html
4. permissions
