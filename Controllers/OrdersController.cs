using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Data;
using OrderManagement.Models;
using OrderManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OrderManagementContext _context;

        public OrdersController(OrderManagementContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var orderManagementContext = _context.Orders.Include(o => o.Agent);
            return View(await orderManagementContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "AgentName");
            ViewData["ItemID"] = new SelectList(_context.Items, "ItemID", "ItemName");

            var viewModel = new CreateOrderViewModel
            {
                Order = new Order
                {
                    OrderDate = DateTime.Now,
                    Status = "Pending",
                    TotalAmount = 0
                },
                OrderDetails = new List<OrderDetail> { new OrderDetail() }
            };

            return View(viewModel);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderViewModel viewModel)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Calculate total amount
                decimal totalAmount = 0;
                foreach (var detail in viewModel.OrderDetails)
                {
                    if (detail.ItemID > 0 && detail.Quantity > 0)
                    {
                        detail.TotalAmount = detail.Quantity * detail.UnitAmount;
                        totalAmount += detail.TotalAmount;
                    }
                }

                // Set the total amount for the order
                viewModel.Order.TotalAmount = totalAmount;

                // Create the order
                _context.Add(viewModel.Order);
                await _context.SaveChangesAsync();

                // Create the order details
                foreach (var detail in viewModel.OrderDetails)
                {
                    if (detail.ItemID > 0 && detail.Quantity > 0)
                    {
                        detail.OrderID = viewModel.Order.OrderID;
                        _context.Add(detail);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = viewModel.Order.OrderID });
            }

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "AgentName", viewModel.Order.AgentID);
            ViewData["ItemID"] = new SelectList(_context.Items, "ItemID", "ItemName");
            return View(viewModel);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            var viewModel = new EditOrderViewModel
            {
                Order = order,
                OrderDetails = order.OrderDetails.ToList()
            };

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "AgentName", order.AgentID);
            ViewData["ItemID"] = new SelectList(_context.Items, "ItemID", "ItemName");
            return View(viewModel);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditOrderViewModel viewModel)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != viewModel.Order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate total amount
                    decimal totalAmount = 0;
                    foreach (var detail in viewModel.OrderDetails)
                    {
                        if (detail.ItemID > 0 && detail.Quantity > 0)
                        {
                            detail.TotalAmount = detail.Quantity * detail.UnitAmount;
                            totalAmount += detail.TotalAmount;
                        }
                    }

                    // Set the total amount for the order
                    viewModel.Order.TotalAmount = totalAmount;

                    // Update order
                    _context.Update(viewModel.Order);

                    // Get existing details
                    var existingDetails = await _context.OrderDetails
                        .Where(od => od.OrderID == id)
                        .ToListAsync();

                    // Delete details that are not in the updated list
                    foreach (var existingDetail in existingDetails)
                    {
                        if (!viewModel.OrderDetails.Any(d => d.ID == existingDetail.ID))
                        {
                            _context.OrderDetails.Remove(existingDetail);
                        }
                    }

                    // Update or add details
                    foreach (var detail in viewModel.OrderDetails)
                    {
                        if (detail.ItemID > 0 && detail.Quantity > 0)
                        {
                            detail.OrderID = id;
                            detail.TotalAmount = detail.Quantity * detail.UnitAmount;

                            if (detail.ID == 0)
                            {
                                _context.Add(detail);
                            }
                            else
                            {
                                _context.Update(detail);
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(viewModel.Order.OrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = viewModel.Order.OrderID });
            }

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "AgentName", viewModel.Order.AgentID);
            ViewData["ItemID"] = new SelectList(_context.Items, "ItemID", "ItemName");
            return View(viewModel);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Agent)
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // First delete all order details
            var orderDetails = await _context.OrderDetails
                .Where(od => od.OrderID == id)
                .ToListAsync();

            foreach (var detail in orderDetails)
            {
                _context.OrderDetails.Remove(detail);
            }

            // Then delete the order
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Print/5
        public async Task<IActionResult> Print(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Item)
                .FirstOrDefaultAsync(m => m.OrderID == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Report
        public IActionResult Report()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: Orders/BestItems
        public async Task<IActionResult> BestItems()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var bestItems = await _context.OrderDetails
                .GroupBy(od => od.ItemID)
                .Select(g => new BestItemViewModel
                {
                    Item = _context.Items.FirstOrDefault(i => i.ItemID == g.Key),
                    TotalQuantity = g.Sum(od => od.Quantity),
                    TotalAmount = g.Sum(od => od.TotalAmount)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            return View(bestItems);
        }

        // GET: Orders/ItemsByAgent/5
        public async Task<IActionResult> ItemsByAgent(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                ViewData["Agents"] = new SelectList(_context.Agents, "AgentID", "AgentName");
                return View(new List<ItemByAgentViewModel>());
            }

            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }

            var itemsByAgent = await _context.Orders
                .Where(o => o.AgentID == id)
                .Join(_context.OrderDetails,
                      o => o.OrderID,
                      od => od.OrderID,
                      (o, od) => new { Order = o, OrderDetail = od })
                .Join(_context.Items,
                      combined => combined.OrderDetail.ItemID,
                      i => i.ItemID,
                      (combined, i) => new ItemByAgentViewModel
                      {
                          ItemName = i.ItemName,
                          ItemSize = i.Size,
                          Quantity = combined.OrderDetail.Quantity,
                          UnitAmount = combined.OrderDetail.UnitAmount,
                          TotalAmount = combined.OrderDetail.TotalAmount,
                          OrderDate = combined.Order.OrderDate
                      })
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            ViewData["AgentName"] = agent.AgentName;
            ViewData["Agents"] = new SelectList(_context.Agents, "AgentID", "AgentName", id);

            return View(itemsByAgent);
        }

        // GET: Orders/AgentsByItem/5
        public async Task<IActionResult> AgentsByItem(int? id)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                ViewData["Items"] = new SelectList(_context.Items, "ItemID", "ItemName");
                return View(new List<AgentByItemViewModel>());
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var agentsByItem = await _context.OrderDetails
                .Where(od => od.ItemID == id)
                .Join(_context.Orders,
                      od => od.OrderID,
                      o => o.OrderID,
                      (od, o) => new { OrderDetail = od, Order = o })
                .Join(_context.Agents,
                      combined => combined.Order.AgentID,
                      a => a.AgentID,
                      (combined, a) => new AgentByItemViewModel
                      {
                          AgentName = a.AgentName,
                          AgentAddress = a.Address,
                          Quantity = combined.OrderDetail.Quantity,
                          UnitAmount = combined.OrderDetail.UnitAmount,
                          TotalAmount = combined.OrderDetail.TotalAmount,
                          OrderDate = combined.Order.OrderDate
                      })
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            ViewData["ItemName"] = item.ItemName;
            ViewData["Items"] = new SelectList(_context.Items, "ItemID", "ItemName", id);

            return View(agentsByItem);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}