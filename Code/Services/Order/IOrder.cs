using SalesOrder.Models;
using SalesOrder.Services.Order.Models;
using SalesOrder.Models;

namespace SalesOrder.Services.Order
{
    public interface IOrder
    {
        DataBaseActionResult Save(OrderModel payload, bool IsUpdate = false);
        DataBaseActionResult GetSelectCust(string Term);
        List<OrderModel> ListOrder(DateTime? Date = null, string keyword = "");
        OrderModel OrderById(long id);
        DataBaseActionResult Delete(long Id, bool IsItem = false);
    }
}
