namespace CakeWeb.App.ViewModels.Users
{
    using System;

    public class GetProfileInfoInputModel
    {
        public string FullName { get; set; }

        public DateTime DateOfReg { get; set; }

        public int OrdersCount { get; set; }
    }
}
