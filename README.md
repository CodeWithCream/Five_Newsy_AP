# Five_Newsy_API
FIVE Backend interview project. 

## Description
The project is an APS.NET Core application. The application is a backend API that will support a news-related web and mobile application.
The apps are supposed to be used by readers and by authors who publish and edit articles. 

## Usage
This application can run locally, or the published version can be used for testing.  

### Local

#### Database
To run the server locally, a local database must be created, and the connection string in `appsettings.Development.json` has to be changed to point to the local database. 
To run migrations, open package manager console in Visual Studio and run: 
```
	dotnet ef database update --project Newsy_API.DAL
```

After the database is set, the application can connect to it and be used.

#### Application
To use an API, you must first build and start the application.  
Running the application opens up the Swagger page on `https://localhost:7178/swagger/index.html`. 

The application port can be changed in `launchSettings.json`.

### Production
The application is published to Azure, so it can be used immediately without building locally.  

#### Database
Production database connection string is `"server=newsydbserver.database.windows.net;database=NewsyDb;uid=newsy;pwd=FivePass!;"`.
The database is already filled with some data, so API can be used without registering new users, only by using the token to authorize. 

```
{
  "firstName": "Pero",
  "lastName": "Perić",
  "eMail": "pero.peric@mail.com",
  "password": "Pero123!",
  "confirmPassword": "Pero123!",
  "roleKey": "reader"
}
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicGVyby5wZXJpY0BtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiZDY3ZDg4MjktMjVhZS00ZTJmLWFiZjktNDMyZjk1ZTY1ZjNlIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiUmVhZGVyIiwiZXhwIjoxNjU3MzcxMDc0LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNiIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjQ0MzM2In0.aQ7TJjKJOHAUa_sLO3zc4C7jRZuioXRQaVHALTFVmqc",
  "userId": 2
}


{
  "firstName": "Ana",
  "lastName": "Anić",
  "eMail": "ana.anic@mail.com",
  "password": "Ana123!",
  "confirmPassword": "Ana123!",
  "roleKey": "author"
}
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYW5hLmFuaWNAbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjRmYThkYzJmLWFmOTItNDE3Yi05NWIxLTgwMWZkOTUwMjQ2MyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkF1dGhvciIsImV4cCI6MTY1NzM3MTExMiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzYiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNiJ9.giZXDKeuB4dr08Dd6dByz27Oxy4dJFTLEqo-A-0_rHs",
  "userId": 3
}

{
  "firstName": "Iva",
  "lastName": "Ivić",
  "eMail": "iva.ivic@mail.com",
  "password": "Iva123!",
  "confirmPassword": "Iva123!",
  "roleKey": "author"
}
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiaXZhLml2aWNAbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE0Y2Q5YjUzLTJiNDktNDhlYS1hMmI5LWExZTlhMDA2YjA3MCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkF1dGhvciIsImV4cCI6MTY1NzM3MTM3OSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMzYiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo0NDMzNiJ9.fq1My3glauq4keIIKphUcKaNt7_dHF2Mesyta5LKLZg",
  "userId": 4
}
```

### Server
The application is running on Azure on `https://five-newsy.azurewebsites.net/`.
Detailed API description can be found on the Swagger page  (`https://five-newsy.azurewebsites.net/swagger/index.html`), which can be used for testing the API. 



