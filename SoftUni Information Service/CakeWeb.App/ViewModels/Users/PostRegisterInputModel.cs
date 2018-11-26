namespace CakeWeb.App.ViewModels.Users
{
    //Може да се казва и InputModel вместо ViewModel
    //Input Model e подходящо за параметри, които ни идват от Action-a,
    //Т.е параметри, които получаваме към аction-a
    //A ViewModel е когато подаваме модел към дадено View.
    public class PostRegisterInputModel
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string FullName { get; set; }
    }
}
