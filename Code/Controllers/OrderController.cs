using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SalesOrder.Models;
using System.Text.Json.Serialization;
using WebApplication1.Models;
using SalesOrder.Services.Order;
using SalesOrder.Services.Order.Models;

namespace SalesOrder.Controllers
{
    [Route("")]
    public class OrderController : Controller
    {
        private readonly IOrder Order;

        public OrderController(IOrder order)
        {
            Order = order;
        }
        public IActionResult Index(DateTime? date, string keyword)
        {
            ViewBag.ListOrder = Order.ListOrder(date, keyword);
            return View();
        }

        [Route("order-add")]
        public IActionResult Add()
        {
            ViewBag.Customer = Order.GetSelectCust("").Data;
            return View(new OrderModel());
        }

        [Route("order-update")]
        public IActionResult Update(long id)
        {
            ViewBag.Customer = Order.GetSelectCust("").Data;
            OrderModel DtOrder = Order.OrderById(id);
            return View(DtOrder);
        }

        [HttpPost]
        [Route("order-submit")]
        public IActionResult Submit(OrderModel payload)
        {
            DataBaseActionResult Result = new DataBaseActionResult();

            try
            {
                Result = Validate(payload);

                if (Result.IsSuccess)
                {
                    Result = Order.Save(payload);
                }
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message.ToString();
            }

            return Json(Result);
        }

        [HttpPost]
        [Route("order-list")]
        public IActionResult ListOrdder(DateTime? Date, string Keyword)
        {
            DataBaseActionResult Result = new DataBaseActionResult();

            try
            {
                var DtOrder = Order.ListOrder(Date, Keyword);

                Result.IsSuccess = true;
                Result.Data = DtOrder;
            }
            catch (Exception ex)
            {
               Result.Message = ex.Message.ToString();
            }

            return Json(Result);
        }

        [HttpPost]
        [Route("order-update")]
        public IActionResult Update(OrderModel payload)
        {
            DataBaseActionResult Result = new DataBaseActionResult();

            try
            {
                Result = Validate(payload);

                if (Result.IsSuccess)
                {
                    Result = Order.Save(payload, true);
                }
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message.ToString();
            }

            return Json(Result);
        }

        private DataBaseActionResult Validate(OrderModel payload)
        {
            DataBaseActionResult Result = new DataBaseActionResult();

            if (string.IsNullOrEmpty(payload.OrderNo))
            {
                Result.Message = "Order Number is required";
                return Result;
            }

            if (payload.OrderDate == null)
            {
                Result.Message = "Order Date is required";
                return Result;
            }

            if (payload.ComCutomerId == null)
            {
                Result.Message = "Customer is required";
                return Result;
            }

            if (string.IsNullOrEmpty(payload.Address))
            {
                Result.Message = "Address is required";
                return Result;
            }

            Result.IsSuccess = true;

            return Result;
        }

        [Route("order-delete")]
        public IActionResult Delete(long id)
        {
            return Json(Order.Delete(id));
        }

        [Route("order-delete-item")]
        public IActionResult DeleteItem(long id)
        {
            return Json(Order.Delete(id, true));
        }
    }
}
