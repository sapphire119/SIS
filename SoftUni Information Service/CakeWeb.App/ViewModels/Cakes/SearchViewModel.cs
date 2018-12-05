namespace CakeWeb.App.ViewModels.Cakes
{
    using System.Collections.Generic;

    public class SearchViewModel
    {
        public SearchViewModel()
        {
            this.Cakes = new List<CakeViewModel>();
        }

        public SearchViewModel(string searchResult, IEnumerable<CakeViewModel> cakes)
            : this()
        {
            this.Cakes = cakes;
            this.SearchResult = searchResult;
        }

        public string SearchResult { get; set; }

        public IEnumerable<CakeViewModel> Cakes { get; set; }
    }
}
