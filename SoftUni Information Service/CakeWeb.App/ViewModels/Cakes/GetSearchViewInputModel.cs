namespace CakeWeb.App.ViewModels.Cakes
{
    using System.Collections.Generic;

    public class GetSearchViewInputModel
    {
        public List<CakeViewModel> CakesList { get; set; } = new List<CakeViewModel>();
    }
}
