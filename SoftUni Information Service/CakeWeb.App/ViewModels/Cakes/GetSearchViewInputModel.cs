namespace CakeWeb.App.ViewModels.Cakes
{
    using System.Collections.Generic;

    public class GetSearchViewInputModel
    {
        public GetSearchViewInputModel()
        {
            this.CakesList = new List<CakeViewModel>();
        }

        public List<CakeViewModel> CakesList { get; set; }
    }
}
