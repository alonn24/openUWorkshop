﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupBuyingLib.Model;
using GroupBuyingLib.Model.OrderLib;
using GroupBuyingLib.Model.ProductLib;

namespace GroupBuyingLib.DAL
{
    public class OrderDAL
    {
        /// <summary>
        /// Convert row to order
        /// </summary>
        /// <param name="row"></param>
        /// <param name="buyer"></param>
        /// <param name="product"></param>
        /// <returns></returns>
        public static Order FromRow(DataRow row, User buyer, Product product)
        {
            Order returnOrder = new Order(
                (int)row["OrderId"],
                buyer,
                product);
            returnOrder.Date = (DateTime)row["OrderDate"];
            returnOrder.Quantity = (int)row["Quantity"];

            return returnOrder;
        }

        /// <summary>
        /// Get buyer orders
        /// </summary>
        /// <returns></returns>
        public List<Order> GetUserOrders(string userName)
        {
            List<Order> order = getAllOrders();
            return order.Where(o => o.User.UserName == userName).ToList();
        }

        /// <summary>
        /// Get merchant orders
        /// </summary>
        /// <param name="merchant"></param>
        /// <returns></returns>
        public List<Order> GetMerchantOrders(string merchant) {
            List<Order> orders = getAllOrders();
            return orders.Where(o => o.Product.Seller.UserName == merchant).ToList();
        }

        public int orderProducts(OrderRequest order) {
            // Add product to db
            Object[] parameters = new Object[] {
                order.Buyer, order.ProductId, order.Quantity, order.OrderDate
            };
            var res = DataProvider.Instance.executeCommand("INSERT INTO Orders" +
                " ([Buyer], [ProductId], [Quantity], [OrderDate])" +
                " VALUES (@p0, @p1, @p2, @p3)", parameters);
            return (int)res;
        }

        private List<Order> getAllOrders()
        {
            List<Order> orders = new List<Order>(); // Return value

            // Get tables
            DataTable Products = ProductDAL.getActiveProductsTable();
            DataTable Users = DataProvider.Instance.getTable("Users");
            DataTable Orders = DataProvider.Instance.getTable("Orders");

            // Get orders with users
            var query = from seller in Users.AsEnumerable()
                        from buyer in Users.AsEnumerable()
                        from product in Products.AsEnumerable()
                        from order in Orders.AsEnumerable()
                        where seller.Field<String>("UserName") == product.Field<String>("Seller") &&
                        buyer.Field<String>("UserName") == order.Field<String>("Buyer") &&
                        product.Field<int>("ProductId") == order.Field<int>("ProductId")
                        select new
                        {
                            Product = product,
                            Seller = seller,
                            Buyer = buyer,
                            Order = order
                        };
            // Create objects
            foreach (var queryObj in query)
            {
                User seller = UserDAL.FromRow(queryObj.Seller);
                Product product = ProductDAL.FromRow(queryObj.Product, seller);
                User buyer = UserDAL.FromRow(queryObj.Buyer);
                Order order = FromRow(queryObj.Order, buyer, product);
                orders.Add(order);
            }
            return orders;
        }
    }
}
