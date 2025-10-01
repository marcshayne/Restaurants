using Restaurants.Domain.Contants;
using Restaurants.Domain.Entities;

namespace Restaurants.Domain.Interfaces;
public interface IRestaurantAuthorizationService
{
    bool Authorize(Restaurant restaurant, ResourceOperation resourceOperation);
}