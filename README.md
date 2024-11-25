1. SQL Database Setup
To initialize the Book Rental System database and seed it with initial data, follow these steps:

Run the BookRentalSystem.sql Script for located in root folder

2. Email Configuration
The system uses SMTP to send email notifications for rental actions (like book rentals and returns). You will need to configure the email server settings in the appsettings.json file.

Open appsettings.json in your project.

Under the EmailSettings section, update the configuration with your SMTP server credentials.

3. Set the API Startup Project
To run the API project and set it as the startup project:

Open Visual Studio.
Set the API project as the Startup Project:
Right-click on the API project in the Solution Explorer.
Select Set as Startup Project.

4. Run the API
Start the API:

In Visual Studio, click Run (or press F5) to start the API project.
Visual Studio will build the project and launch the API in your default web browser.
Select Hosting Option:

You can choose either IIS Express or IIS to host the application.
IIS Express is the default, but you can configure it to use IIS if you need a production-like environment.
Access the Swagger Documentation:

Once the API is running, open the browser and navigate to:

http://localhost:{port}/swagger/index.html

Replace {port} with the port number Visual Studio assigns to your application (e.g., 5000 or 5001).

This will open the Swagger UI, where you can interact with the API and test the endpoints.


5. Notes 
 1> For activity logs you can fire SQL command 
 Select * from dbo.ActivityLogs

 2> For performance metrix you can fire SQL command
  SELECT 
    Endpoint,
    COUNT(*) AS RequestsCount,
    AVG(DurationMs) AS AverageResponseTimeMs,
    MAX(DurationMs) AS MaxResponseTimeMs,
    MIN(DurationMs) AS MinResponseTimeMs
FROM ActivityLogs
WHERE LogType = 'Performance'
GROUP BY Endpoint;
