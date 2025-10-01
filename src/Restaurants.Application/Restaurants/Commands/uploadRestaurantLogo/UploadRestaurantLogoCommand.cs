
using MediatR;

namespace Restaurants.Application.Restaurants.Commands.uploadRestaurantLogo;
public class UploadRestaurantLogoCommand : IRequest
{
    public int RestaurantId { get; set; }
    public string FileName { get; set; } = default!;
    public Stream File { get; set; } = default!;
}
