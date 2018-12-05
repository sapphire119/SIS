namespace CakeWeb.App.ViewModels.Cakes
{
    public class CakeViewModel
    {
        public CakeViewModel() { }

        public CakeViewModel(int cakeId, string cakeName, decimal cakePrice)
            : this()
        {
            this.CakeId = cakeId;
            this.CakeName = cakeName;
            this.CakePrice = cakePrice;
        }

        public int CakeId { get; set; }

        public string CakeName { get; set; }

        public decimal CakePrice { get; set; }
    }
}
