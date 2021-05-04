# PES-Server
<p>Server for PES social media service</p>

<h2>1. Setup </h2>
The easy way to setup this application is to locate docker-compose.yaml file, that is located in ./src/ and then type
<code>docker-compose up</code>
Alternatively you can manually run the application, it that case:
1) You would need PostgreSQL Database running
2) Configure postgre sql connection connection string. You can do that either from appsettings file, command line arguments or enviroment variables. You would need to configure following variables:
<code>User</code> <code>Password</code> 
- login and password of the database user, that is provided to the application
<code>Port</code>
- port, that database exposes. It would be used to create connection
<code>Host</code>
- name of the database host.
<h5>If running database locally it would be <b>localhost</b></h5>
<code>InitializeDb</code>
- boolean, that indicates whether ef core migrations would need to be applied to database in startup
<code>DB</code>
- optional, would be used as the name for created database if InitializeDb configuration is set to true
3) Apply ef core migrations:
<h5>a) Use InitializeDb configuration, in which case they would be applied automatically</h5>
<h5>b) Use migration files by running </h5> <code>dotnet ef database update -p Infrastructure/Infrastructure.csproj -s WebAPI/WebAPI.csproj -c PESContext</code>
<h5>c) Using sql scripts: </h5><h5>c.1) Run: </h5><code>dotnet ef dbcontext script -p Infrastructure/Infrastructure.csproj -s WebAPI/WebAPI.csproj -c PESContext</code>
<h5>c.2) Apply generated sql scripts to the database</h5>
<h2>2. About frontend</h2>

If you wish to explore frontend of the PES, you may visit [this repository](https://github.com/catdevdev/pes-client "PES Client") 

![image](https://user-images.githubusercontent.com/64675654/116991462-4298eb80-acdd-11eb-90d9-0a33f78e4f38.png)
