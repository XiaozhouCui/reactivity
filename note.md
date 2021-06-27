## Reset database with seed values
- To drop the messed up database, run `dotnet ef database drop -s API -p Persistence`
- `-s` is for starter project, `-p` is for project
- A new dummy database will be created when running `dotnet watch run` 

## Modelling many-to-many relationship
- To model the many-to-many relationship between AppUser and Activity, need to create a join table ActivityAttendee
- To create a join table, create a new class ActivityAttendee in Domain, and update the AppUser and Activity class to refer to ActivityAttendee
- Update the DataContext to set primary key and foreign key for ActivityAttendee table
- Run the migration `dotnet ef migrations add ActivityAttendee -p Persistence -s API`
- To undo the migration, run `dotnet ef migrations remove -p Persistence -s API`

## Add a new project: Infrastructure
- In root solution folder, run `dotnet new classlib -n Infrastructure`
- To add the new project into solution, run `dotnet sln add Infrastructure`
- Add reference from Infrastructure to the Application, goto ./Infrastructure/ and run `dotnet add reference ../Application`
- Add reference from API to Infrastructure, goto ./API/ and run `dotnet add reference ../Infrastructure`
- Go back to solution root folder, run `dotnet restore`, so all projects are aware of the new dependencies

## Adding new property into Activity entity
- When a new property is added into the Activity entity in Domain, need to run migration again
- Goto solution folder and run migration `dotnet ef migrations add AddCancelledProperty -p Persistence -s API`

## Integrate Cloudinary
- Register a new account at Cloudinary for photo uploads
- In NuGet, install CloudinaryDotNet SDK into Infrastructure project
- In API project, edit `appsettings.json` to include CloudName, ApiKeys and ApiSecret for Cloudinary

## Adding Photo entity and relate to AppUsers
- Add Photo entity, and add Photos property in AppUsers, migration will auto add one-to-many relationship
- Go back to root folder, run migration `dotnet ef migrations add PhotoEntityAdded -p Persistence -s API`
- Once relationship is added, go to API project and run `dotnet watch run` to create new table
- To check the new table in VS Code, quick open `SQLite: Open Database`, select the db file, then click the **SQLITE EXPLORER** in VS Code side bar on the left

## Add PostgreSQL to replace SQLite in development
- Install PostgreSQL in docker: `docker run --name dev -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=secret -p 5432:5432 -d postgres:latest`
- In NuGet Gallery, install **Npgsql.EntityFrameworkCore.PostgreSQL** to Persistence project
- Update ApplicationServiceExtensions.cs, replace `opt.UseSqlite` with `opt.UseNpgsql`
- In appsettings.Development.json, update the connection string `Server=localhost; Port=5432; User Id=admin; Password=secret; Database=reactivities`
- Delete Migration folder in Persistence project, to remove all SQLite migrations
- Re-run migration using PostgreSQL provider: `dotnet ef migrations add PGInitial -p Persistence -s API`
- To update Entity Framework, run `dotnet tool update -g dotnet-ef`

## Setup Heroku
- On Heroku, create a project **reactivities88**
- Create a PostgreSQL database: *Resources -> Add-ons -> Heroku Postgres -> free version*
- Install Heroku CLI, then run `heroku login`
- Add remote repo: `heroku git:remote -a reactivities88`
- Add buildpack: `heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack`

## Deploy on Heroku
- Update services.AddDbContext to dynamically get DB connection string from Heroku
- Commit changes and run `git push heroku`

## Improve security
- In NuGet, install **NWebsec.AspNetCore.Middleware** to API
- Add security middleware in Startup.cs