namespace CakeWeb.App.ViewModels.Orders
{
    using CakeWeb.App.ViewModels.Cakes;
    using System.Collections.Generic;

    public class AddToCartViewModel
    {
        public List<CakeViewModel> Cakes { get; set; }

        public decimal TotalPriceOfOrder { get; set; }
    }
}
