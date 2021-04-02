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