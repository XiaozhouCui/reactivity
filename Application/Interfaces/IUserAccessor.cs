namespace Application.Interfaces
{
    // Application proj does NOT have dependency on Infrastructure proj, need to access the class via an Interface
    public interface IUserAccessor
    {
         string GetUsername();
    }
}