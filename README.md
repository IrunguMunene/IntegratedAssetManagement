# IntegratedAssetManagement

The integrated asset management system was initially developed to keep track of a members organization assets. However, I have adapted the idea into a library management system. The app is initially being developed for a local church as part of giving back to society. However I have changed the name of the church to the name of my private company name for purposes of show casing my skills.

The system is developed using ASP.NET Core 3.1 MVC, Entity Framework Core 3.1 and Microsoft SQL Server for the database. 
If your SQL name is not the default Instance change the Connection string in appsettings.json to what is relevant for your environment.

Upon running the application for the first time, once you get to the landing page, run the LibDemoInsertDataScript from 
SQL Server Management targeting the Library database. The database is created on first run and running the script will
populate it will test data.

The app makes use of Serilog for logging and all logging info is stored in a text file 
E:\Code\ASP.NET Core\IntegratedAssetManagement\Library\bin\Debug\netcoreapp3.1\ErrorLogs.txt. 

This is still work in progress but show cases the basic functionality the system will have once completed.
