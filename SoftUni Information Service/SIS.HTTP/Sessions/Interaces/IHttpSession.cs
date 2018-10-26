namespace SIS.HTTP.Sessions.Interfaces
{
    public interface IHttpSession
    {
        string Id { get; }

        object GetParameters(string name);

        bool ContainsParamter(string name);

        void AddParamter(string name, object parameter);

        void ClearParameters();
    }
}
