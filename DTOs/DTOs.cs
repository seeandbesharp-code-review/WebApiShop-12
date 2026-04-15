using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public record UserDTO(int UserId, [EmailAddress][Required] string UserName, string Password, [MinLength(3)] string FirstName, string LastName);
    public record ProductDTO(int ProductId, string ProductName, decimal Price, int CategoryId, string CategoryName, string Description);
    public record OrderDTO(int OrderId, DateOnly OrderDate, decimal OrderSum, int UserId, ICollection<OrderItemDTO> OrderItems);
    public record OrderItemDTO(int OrderItemId, int ProductId, int OrderId, int Quantity);
    public record CategoryDTO(int CategoryId, string CategoryName);
    public record LoginUserDTO([EmailAddress][Required] string UserName, [Required] string Password);
    public record PageResponseDTO(IEnumerable<ProductDTO>? data,int total, int page, int size, bool hasNaxt, bool hasPrevious, int numOfPages);
}

