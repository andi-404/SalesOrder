using System.ComponentModel.DataAnnotations;

namespace SalesOrder.Services.Order.Models
{
    public class OrderModel
    {
        public OrderModel()
        {
            ListItem = new List<ItemModel>();
        }

        public long? SoOrderId { get; set; }
        //[Required(ErrorMessage = "This field is required")]
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? ComCutomerId { get; set; }
        public string ComCutomerName { get; set; }
        public string Address { get; set; }

        public List<ItemModel> ListItem { get; set; }
    }

    public class ItemModel
    {
        public long? SoItemId { get; set; }
        public long? SoOrderId { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
