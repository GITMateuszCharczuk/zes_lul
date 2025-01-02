using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using P06Shop.Api.Data;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using P06Shop.Api.Mappers.Interfaces;

namespace P06Shop.Api.Services
{
    public class OrderService
    {
        private readonly MongoDbContext _context;
        private readonly IOrderMapper _orderMapper;

        public OrderService(MongoDbContext context, IOrderMapper orderMapper)
        {
            _context = context;
            _orderMapper = orderMapper;
        }

        public async Task<(bool Success, IEnumerable<GetOrderDTO>? Orders, string? ErrorMessage)> GetOrders()
        {
            try
            {
                var orders = await _context.Orders.Find(_ => true).ToListAsync();
                var orderDtos = _orderMapper.MapToGetOrderDTOs(orders);
                return (true, orderDtos, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving orders: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetOrderDTO? Order, string? ErrorMessage)> GetOrder(string id, string userId, bool isAdmin)
        {
            try
            {
                var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (order == null)
                    return (false, null, "Order not found");

                if (order.UserId != userId && !isAdmin)
                    return (false, null, "Forbidden");

                var orderDto = _orderMapper.MapToGetOrderDTO(order);
                return (true, orderDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving order: {ex.Message}");
            }
        }

        public async Task<(bool Success, IEnumerable<GetOrderDTO>? Orders, string? ErrorMessage)> GetUserOrders(string userId, string currentUserId, bool isAdmin)
        {
            try
            {
                if (userId != currentUserId && !isAdmin)
                    return (false, null, "Forbidden");

                var orders = await _context.Orders.Find(x => x.UserId == userId).ToListAsync();
                var orderDtos = _orderMapper.MapToGetOrderDTOs(orders);
                return (true, orderDtos, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while retrieving user orders: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetOrderDTO? Order, string? ErrorMessage)> CreateOrder(CreateOrderDTO orderDto, string currentUserId)
        {
            try
            {
                string userId = !string.IsNullOrEmpty(orderDto.UserId) ? orderDto.UserId : currentUserId;

                var user = await _context.Users.Find(x => x.Id == userId).FirstOrDefaultAsync();
                if (user == null)
                    return (false, null, "Invalid user ID");

                decimal total = 0;
                var orderItems = new List<OrderItem>();
                foreach (var item in orderDto.Items)
                {
                    var product = await _context.Products.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                    if (product == null)
                        return (false, null, $"Product with ID {item.ProductId} not found");
                    
                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtOrder = (decimal)product.Price
                    };
                    orderItems.Add(orderItem);
                    total += orderItem.PriceAtOrder * orderItem.Quantity;
                }
                
                var order = _orderMapper.MapCreateDTOToOrder(orderDto, userId, orderItems, total);
                await _context.Orders.InsertOneAsync(order);

                var update = Builders<User>.Update.Push(u => u.OrderIds, order.Id);
                await _context.Users.UpdateOneAsync(u => u.Id == order.UserId, update);

                var createdOrderDto = _orderMapper.MapToGetOrderDTO(order);
                return (true, createdOrderDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while creating order: {ex.Message}");
            }
        }

        public async Task<(bool Success, GetOrderDTO? Order, string? ErrorMessage)> UpdateOrder(string id, UpdateOrderDTO updateDto, string userId, bool isAdmin)
        {
            try
            {
                var existingOrder = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (existingOrder == null)
                    return (false, null, "Order not found");

                if (existingOrder.UserId != userId && !isAdmin)
                    return (false, null, "Forbidden");

                decimal total = 0;
                var orderItems = new List<OrderItem>();
                foreach (var item in updateDto.Items)
                {
                    var product = await _context.Products.Find(x => x.Id == item.ProductId).FirstOrDefaultAsync();
                    if (product == null)
                        return (false, null, $"Product with ID {item.ProductId} not found");

                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        PriceAtOrder = (decimal)product.Price
                    };
                    orderItems.Add(orderItem);
                    total += orderItem.PriceAtOrder * orderItem.Quantity;
                }

                var updatedOrder = _orderMapper.MapUpdateDTOToOrder(id, updateDto, existingOrder, orderItems, total);
                var result = await _context.Orders.ReplaceOneAsync(x => x.Id == id, updatedOrder);
                if (result.ModifiedCount == 0)
                    return (false, null, "Order not found");

                var updatedOrderDto = _orderMapper.MapToGetOrderDTO(updatedOrder);
                return (true, updatedOrderDto, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Internal server error while updating order: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateOrderStatus(string id, string status)
        {
            try
            {
                var update = Builders<Order>.Update.Set(o => o.Status, status);
                var result = await _context.Orders.UpdateOneAsync(x => x.Id == id, update);

                if (result.ModifiedCount == 0)
                    return (false, "Order not found");

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error while updating order status: {ex.Message}");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteOrder(string id, string userId, bool isAdmin)
        {
            try
            {
                var order = await _context.Orders.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (order == null)
                    return (false, "Order not found");

                if (order.UserId != userId && !isAdmin)
                    return (false, "Forbidden");

                var update = Builders<User>.Update.Pull(u => u.OrderIds, id);
                await _context.Users.UpdateOneAsync(u => u.Id == order.UserId, update);

                await _context.Orders.DeleteOneAsync(x => x.Id == id);

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Internal server error while deleting order: {ex.Message}");
            }
        }
    }
} 