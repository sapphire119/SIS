namespace CakeWeb.App.ViewModels.Orders
{
    public class AllViewModel
    {
        public AllViewModel(int id, string dateOfRegistration, decimal totalSumOfOrder)
        {
            this.Id = id;
            this.DateOfRegistration = dateOfRegistration;
            this.TotalSumOfOrder = totalSumOfOrder;
        }

        public int Id { get; set; }

        public string DateOfRegistration { get; set; }

        public decimal TotalSumOfOrder { get; set; }
    }
}
