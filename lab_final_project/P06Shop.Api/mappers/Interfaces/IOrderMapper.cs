using P06Shop.Api.Models;
using P06Shop.Api.Models.Dto;

namespace P06Shop.Api.Mappers.Interfaces
{
    public interface IOrderMapper
    {
        GetOrderDTO MapToGetOrderDTO(Order order);
        IEnumerable<GetOrderDTO> MapToGetOrderDTOs(IEnumerable<Order> orders);
        Order MapCreateDTOToOrder(CreateOrderDTO createDTO, string userId, List<OrderItem> orderItems, decimal totalAmount);
        Order MapUpdateDTOToOrder(string id, UpdateOrderDTO updateDTO, Order existingOrder, List<OrderItem> orderItems, decimal totalAmount);
        GetOrderItemDTO MapToGetOrderItemDTO(OrderItem orderItem);
        IEnumerable<GetOrderItemDTO> MapToGetOrderItemDTOs(IEnumerable<OrderItem> orderItems);
    }
} 