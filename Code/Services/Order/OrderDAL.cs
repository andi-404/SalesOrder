using Microsoft.Data.SqlClient;
using SalesOrder.Models;
using System.Data;
using WebApplication1.Models;
using SalesOrder.Services.Order.Models;
using System.Net;

namespace SalesOrder.Services.Order
{
    public class OrderDAL : IOrder
    {
        private readonly IConfiguration Configuration;
        private readonly string ConnectionString;
        public OrderDAL(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = configuration.GetConnectionString("DB");
        }

        public DataBaseActionResult Save(OrderModel payload, bool IsUpdate)
        {
            DataBaseActionResult result = new DataBaseActionResult();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand(IsUpdate ? "[dbo].[stp_OrderUpdate]" : "[dbo].[stp_OrderSubmit]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@OrderDate", payload.OrderDate);
                    sqlCommand.Parameters.AddWithValue("@Address", payload.Address);
                    sqlCommand.Parameters.AddWithValue("@OrderNo", payload.OrderNo);
                    sqlCommand.Parameters.AddWithValue("@CutomerID", payload.ComCutomerId);
                    sqlCommand.Parameters.Add(SetItem(payload));

                    if (IsUpdate)
                    {
                        sqlCommand.Parameters.AddWithValue("@SoOrderId", payload.SoOrderId);

                    }

                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            result.IsSuccess = Convert.ToBoolean(Convert.ToInt32(dataReader["Kode"]));
                            result.Message = dataReader["Message"].ToString();
                        }
                    }

                    sqlConnection.Close();
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }

            return result;

        }

        public DataBaseActionResult Delete(long Id, bool IsItem)
        {
            DataBaseActionResult Result = new DataBaseActionResult();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand(IsItem ? "[dbo].[stp_OrderDeleteItem]" : "[dbo].[stp_OrderDelete]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", Id);
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Result.IsSuccess = Convert.ToBoolean(Convert.ToInt32(dataReader["Kode"]));
                            Result.Message = dataReader["Message"].ToString();
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Result.Message = ex.Message;
            }

            return Result;
        }

        public List<OrderModel> ListOrder(DateTime? Date, string keyword)
        {
            List<OrderModel> ListOrder = new List<OrderModel>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[dbo].[stp_OrderGet]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Date", Date == null ? DBNull.Value : Date);
                    sqlCommand.Parameters.AddWithValue("@Keyword", string.IsNullOrEmpty(keyword) ? DBNull.Value : keyword);


                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            ListOrder.Add(new OrderModel()
                            {
                                SoOrderId = dataReader["SO_ORDER_ID"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["SO_ORDER_ID"]),
                                OrderNo = dataReader["ORDER_NO"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["ORDER_NO"]).Trim(),
                                OrderDate = dataReader["ORDER_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataReader["ORDER_DATE"]),
                                Address = dataReader["ADDRESS"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["ADDRESS"]).Trim(),
                                ComCutomerName = dataReader["CUSTOMER_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["CUSTOMER_NAME"]).Trim()
                            });
                        }
                    }
                    sqlConnection.Close();

                }

            }
            catch (Exception ex)
            {
                return ListOrder;
            }

            return ListOrder;

        }

        public OrderModel OrderById(long id)
        {
            OrderModel Result = new OrderModel();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[dbo].[stp_OrderGetById]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", id);


                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Result.SoOrderId = dataReader["SO_ORDER_ID"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["SO_ORDER_ID"]);
                            Result.OrderNo = dataReader["ORDER_NO"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["ORDER_NO"]).Trim();
                            Result.OrderDate = dataReader["ORDER_DATE"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dataReader["ORDER_DATE"]);
                            Result.Address = dataReader["ADDRESS"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["ADDRESS"]).Trim();
                            Result.ComCutomerName = dataReader["CUSTOMER_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["CUSTOMER_NAME"]).Trim();
                            Result.ComCutomerId = dataReader["COM_CUSTOMER_ID"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["COM_CUSTOMER_ID"]);
                        }
                    }
                    sqlConnection.Close();

                }

                Result.ListItem = OrderItemById((long)Result.SoOrderId);

            }
            catch (Exception ex)
            {
                return Result;
            }

            return Result;
        }

        public List<ItemModel> OrderItemById(long id)
        {
            List<ItemModel> Result = new List<ItemModel>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[dbo].[stp_OrderItemByIdOrder]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", id);

                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Result.Add(new ItemModel()
                            {
                                SoOrderId = id,
                                SoItemId = dataReader["SO_ITEM_ID"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["SO_ITEM_ID"]),
                                ItemName = dataReader["ITEM_NAME"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["ITEM_NAME"]).Trim(),
                                Quantity = dataReader["QUANTITY"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["QUANTITY"]),
                                Price = dataReader["PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["PRICE"])
                            });
                        }
                    }
                    sqlConnection.Close();

                }
            }
            catch (Exception ex)
            {
                return Result;
            }

            return Result;
        }

        public DataBaseActionResult GetSelectCust(string Term)
        {
            DataBaseActionResult Result = new DataBaseActionResult();
            List<SelectModel> Select2 = new List<SelectModel>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                using (SqlCommand sqlCommand = new SqlCommand("[dbo].[stp_GetCustomer]", sqlConnection))
                {
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            Select2.Add(new SelectModel()
                            {
                                id = dataReader["id"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["id"]),
                                text = dataReader["text"] == DBNull.Value ? string.Empty : Convert.ToString(dataReader["text"]).Trim()
                            });
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

            Result.IsSuccess = true;
            Result.Message = "Get Select2";
            Result.Data = Select2;

            return Result;
        }

        private SqlParameter SetItem(OrderModel model)
        {
            SqlParameter udt;

            try
            {
                using (DataTable dtFiles = new DataTable())
                {
                    dtFiles.Columns.Add("SO_ITEM_ID", typeof(long));
                    dtFiles.Columns.Add("SO_ORDER_ID", typeof(long));
                    dtFiles.Columns.Add("ITEM_NAME", typeof(string));
                    dtFiles.Columns.Add("QUANTITY", typeof(string));
                    dtFiles.Columns.Add("PRICE", typeof(string));

                    int i = 1;

                    foreach (ItemModel item in model.ListItem)
                    {
                        dtFiles.Rows.Add(
                            item.SoItemId,
                            item.SoOrderId,
                            item.ItemName,
                            item.Quantity,
                            item.Price
                        );

                        i++;
                    }
                    udt = new SqlParameter("@Detail", SqlDbType.Structured)
                    {
                        TypeName = "dbo.UDT_OrderItem",
                        Value = dtFiles
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }

            return udt;
        }
    }
}
