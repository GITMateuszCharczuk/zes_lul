using P06Shop.Api.Mappers.Interfaces;
using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;
using MongoDB.Bson;

namespace P06Shop.Api.Mappers
{
    public class OrderMapper : IOrderMapper
    {
        public GetOrderDTO MapToGetOrderDTO(Order order)
        {
            return new GetOrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                Items = MapToGetOrderItemDTOs(order.Items).ToList(),
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };
        }

        public IEnumerable<GetOrderDTO> MapToGetOrderDTOs(IEnumerable<Order> orders)
        {
            return orders.Select(MapToGetOrderDTO);
        }

        public Order MapCreateDTOToOrder(CreateOrderDTO createDTO, string userId, List<OrderItem> orderItems, decimal totalAmount)
        {
            return new Order
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                Items = orderItems,
                TotalAmount = totalAmount,
                Status = "Pending",
                OrderDate = DateTime.UtcNow
            };
        }

        public Order MapUpdateDTOToOrder(string id, UpdateOrderDTO updateDTO, Order existingOrder, List<OrderItem> orderItems, decimal totalAmount)
        {
            return new Order
            {
                Id = id,
                UserId = existingOrder.UserId,
                Items = orderItems,
                TotalAmount = totalAmount,
                Status = updateDTO.Status ?? existingOrder.Status,
                OrderDate = existingOrder.OrderDate
            };
        }

        public GetOrderItemDTO MapToGetOrderItemDTO(OrderItem orderItem)
        {
            return new GetOrderItemDTO
            {
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity,
                PriceAtOrder = orderItem.PriceAtOrder
            };
        }

        public IEnumerable<GetOrderItemDTO> MapToGetOrderItemDTOs(IEnumerable<OrderItem> orderItems)
        {
            return orderItems.Select(MapToGetOrderItemDTO);
        }
    }
} 