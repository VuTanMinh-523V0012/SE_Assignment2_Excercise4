using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Data;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagement.Data
{
    public static class DataInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new OrderManagementContext(
                serviceProvider.GetRequiredService<DbContextOptions<OrderManagementContext>>()))
            {
                // Check if database already has data
                if (context.Items.Any() || context.Agents.Any() || context.Orders.Any())
                {
                    return; // Database has been seeded already
                }

                // Seed Users
                var users = new List<User>
                {
                    new User { UserName = "Admin User", Email = "admin@example.com", Password = "admin123" },
                    new User { UserName = "John Doe", Email = "john@example.com", Password = "password" },
                    new User { UserName = "Jane Smith", Email = "jane@example.com", Password = "password" },
                    new User { UserName = "Bob Johnson", Email = "bob@example.com", Password = "password" },
                    new User { UserName = "Alice Brown", Email = "alice@example.com", Password = "password" },
                    new User { UserName = "Michael Wilson", Email = "michael@example.com", Password = "password" },
                    new User { UserName = "Emily Davis", Email = "emily@example.com", Password = "password" },
                    new User { UserName = "David Miller", Email = "david@example.com", Password = "password" },
                    new User { UserName = "Sarah Jones", Email = "sarah@example.com", Password = "password" },
                    new User { UserName = "James Taylor", Email = "james@example.com", Password = "password" },
                    new User { UserName = "Jessica Clark", Email = "jessica@example.com", Password = "password" },
                    new User { UserName = "Robert White", Email = "robert@example.com", Password = "password" },
                    new User { UserName = "Karen Lee", Email = "karen@example.com", Password = "password" },
                    new User { UserName = "Thomas Martin", Email = "thomas@example.com", Password = "password" },
                    new User { UserName = "Locked User", Email = "locked@example.com", Password = "password" }
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();

                // Seed Items
                var items = new List<Item>
                {
                    new Item { ItemName = "T-Shirt", Size = "Small" },
                    new Item { ItemName = "T-Shirt", Size = "Medium" },
                    new Item { ItemName = "T-Shirt", Size = "Large" },
                    new Item { ItemName = "Jeans", Size = "30" },
                    new Item { ItemName = "Jeans", Size = "32" },
                    new Item { ItemName = "Jeans", Size = "34" },
                    new Item { ItemName = "Sneakers", Size = "8" },
                    new Item { ItemName = "Sneakers", Size = "9" },
                    new Item { ItemName = "Sneakers", Size = "10" },
                    new Item { ItemName = "Backpack", Size = "Regular" },
                    new Item { ItemName = "Laptop Bag", Size = "15 inch" },
                    new Item { ItemName = "Hat", Size = "One Size" },
                    new Item { ItemName = "Gloves", Size = "Small" },
                    new Item { ItemName = "Gloves", Size = "Medium" },
                    new Item { ItemName = "Gloves", Size = "Large" },
                    new Item { ItemName = "Socks", Size = "Small" },
                    new Item { ItemName = "Socks", Size = "Medium" },
                    new Item { ItemName = "Socks", Size = "Large" },
                    new Item { ItemName = "Jacket", Size = "Small" },
                    new Item { ItemName = "Jacket", Size = "Medium" }
                };

                context.Items.AddRange(items);
                await context.SaveChangesAsync();

                // Seed Agents
                var agents = new List<Agent>
                {
                    new Agent { AgentName = "John Smith", Address = "123 Main St, New York, NY" },
                    new Agent { AgentName = "Mary Johnson", Address = "456 Oak Ave, Los Angeles, CA" },
                    new Agent { AgentName = "Robert Brown", Address = "789 Pine Rd, Chicago, IL" },
                    new Agent { AgentName = "Patricia Davis", Address = "321 Elm St, Houston, TX" },
                    new Agent { AgentName = "Michael Wilson", Address = "654 Maple Dr, Miami, FL" },
                    new Agent { AgentName = "Linda Moore", Address = "987 Cedar Ln, Seattle, WA" },
                    new Agent { AgentName = "James Taylor", Address = "741 Birch Rd, Boston, MA" },
                    new Agent { AgentName = "Elizabeth Anderson", Address = "852 Spruce St, Denver, CO" },
                    new Agent { AgentName = "William Jackson", Address = "963 Willow Ave, Atlanta, GA" },
                    new Agent { AgentName = "Jennifer White", Address = "159 Oak St, Dallas, TX" },
                    new Agent { AgentName = "David Harris", Address = "357 Pine Dr, San Francisco, CA" },
                    new Agent { AgentName = "Barbara Martinez", Address = "246 Elm Rd, Phoenix, AZ" },
                    new Agent { AgentName = "Richard Robinson", Address = "753 Maple St, Philadelphia, PA" },
                    new Agent { AgentName = "Susan Lewis", Address = "951 Cedar Ave, San Diego, CA" },
                    new Agent { AgentName = "Joseph Clark", Address = "842 Birch Dr, Austin, TX" }
                };

                context.Agents.AddRange(agents);
                await context.SaveChangesAsync();

                // Seed Orders and OrderDetails
                var random = new Random();
                var orders = new List<Order>();

                // Create 30 orders
                for (int i = 1; i <= 30; i++)
                {
                    var order = new Order
                    {
                        OrderDate = DateTime.Now.AddDays(-random.Next(1, 60)),
                        AgentID = agents[random.Next(0, agents.Count)].AgentID,
                        OrderDetails = new List<OrderDetail>()
                    };

                    // Add 1 to 5 items to each order
                    int numberOfItems = random.Next(1, 6);
                    var selectedItems = new HashSet<int>();

                    for (int j = 0; j < numberOfItems; j++)
                    {
                        int itemId;
                        do
                        {
                            itemId = items[random.Next(0, items.Count)].ItemID;
                        } while (selectedItems.Contains(itemId));

                        selectedItems.Add(itemId);

                        var orderDetail = new OrderDetail
                        {
                            ItemID = itemId,
                            Quantity = random.Next(1, 11),
                            UnitAmount = (decimal)(random.Next(1000, 10000) / 100.0)
                        };

                        order.OrderDetails.Add(orderDetail);
                    }

                    orders.Add(order);
                }

                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();
            }
        }
    }
}