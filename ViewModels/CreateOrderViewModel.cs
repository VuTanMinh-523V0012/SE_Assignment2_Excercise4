using OrderManagement.Models;
using System;
using System.Collections.Generic;

namespace OrderManagement.ViewModels
{
    // For creating a new order with order details
    public class CreateOrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    // For editing an existing order with order details
    public class EditOrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }

    // For displaying best selling items
    public class BestItemViewModel
    {
        public Item Item { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // For displaying items purchased by an agent
    public class ItemByAgentViewModel
    {
        public string ItemName { get; set; }
        public string ItemSize { get; set; }
        public int Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }

    // For displaying agents who purchased a specific item
    public class AgentByItemViewModel
    {
        public string AgentName { get; set; }
        public string AgentAddress { get; set; }
        public int Quantity { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}