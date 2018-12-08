namespace IRunesWebApp.ViewModels
{
    using SIS.Framework.Attributes.Properties;

    public class LoginViewModel
    {
        [Regex(@"\w+")]
        public string Username { get; set; }
        
        public string Password { get; set; }
    }
}
