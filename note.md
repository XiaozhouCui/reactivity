## Reset database with seed values
- To drop the messed up database, run `dotnet ef database drop -s API -p Persistence`
- A new dummy database will be created when running `dotnet watch run` 

## Modelling many-to-many relationship
- To model the many-to-many relationship between AppUser and Activity, need to create a join table ActivityAttendee
- To create a join table, create a new class ActivityAttendee in Domain, and update the AppUser and Activity class to refer to ActivityAttendee
- Update the DataContext to set primary key and foreign key for ActivityAttendee table
- Run the migration `dotnet ef migrations add ActivityAttendee -p Persistence -s API`
- To undo the migration, run `dotnet ef migrations remove -p Persistence -s API`

## Add a new project Infrastructure
- In root solution folder, run `dotnet new classlib -n Infrastructure`
- To add the new project into solution, run `dotnet sln add Infrastructure`
- Add reference from Infrastructure to the Application, goto ./Infrastructure/ and run `dotnet add reference ../Application`
- Add reference from API to Infrastructure, goto ./API/ and run `dotnet add reference ../Infrastructure`
- Go back to solution root folder, run `dotnet restore`, so all projects are aware of the new dependencies