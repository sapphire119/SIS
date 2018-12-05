namespace SIS.HTTP.Sessions
{
    using Interfaces;
    using System.Collections.Generic;

    public class HttpSession : IHttpSession
    {
        private readonly IDictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.parameters = new Dictionary<string, object>();
            this.Id = id;
        }

        public string Id { get; }

        public void AddParamter(string name, object parameter)
        {
            this.parameters[name] = parameter;
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }

        public void RemoveSelectedParamter(string key)
        {
            this.parameters.Remove(key);
        }

        public bool ContainsParamter(string name)
        {
            return this.parameters.ContainsKey(name);
        }

        public object GetParameters(string name)
        {
            var parameter = this.parameters[name];
            return parameter;
        }
    }
}
