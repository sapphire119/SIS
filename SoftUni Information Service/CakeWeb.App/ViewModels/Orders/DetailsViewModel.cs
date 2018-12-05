namespace CakeWeb.App.ViewModels.Orders
{
    using CakeWeb.App.ViewModels.Cakes;
    using System.Collections.Generic;

    public class DetailsViewModel
    {
        public DetailsViewModel()
        {
            this.Cakes = new List<CakeViewModel>();
        }

        public DetailsViewModel(int id, string dateOfRegistration)
            : this()
        {
            this.Id = id;
            this.DateOfRegistration = dateOfRegistration;
        }

        public int Id { get; set; }

        public List<CakeViewModel> Cakes { get; set; }

        public string DateOfRegistration { get; set; }
    }
}
