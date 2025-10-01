using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Restaurants.Domain.Contants;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using Restaurants.Domain.Interfaces;
using Restaurants.Domain.Repositories;

namespace Restaurants.Application.Restaurants.Commands.uploadRestaurantLogo;
public class UploadRestaurantLogoCommandHandler(ILogger<UploadRestaurantLogoCommandHandler> logger,
     IRestaurantsRepository restaurantsRepository,
     IRestaurantAuthorizationService restaurantAuthorizationService,
     IBlobStorageService blobStorageService) : IRequestHandler<UploadRestaurantLogoCommand>
{
    public async Task Handle(UploadRestaurantLogoCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("uploading logo for restaurant with id: {RestaurantId}", request.RestaurantId);
        var restaurant = await restaurantsRepository.GetByIdAsync(request.RestaurantId);
        if (restaurant is null)
            throw new NotFoundException(nameof(Restaurant), request.RestaurantId.ToString());

        if (!restaurantAuthorizationService.Authorize(restaurant, ResourceOperation.Update))
            throw new ForbidenException();

        var logoUrl = await blobStorageService.UploadToBlobAsync(request.File, request.FileName);
        restaurant.LogoUrl = logoUrl;

        await restaurantsRepository.SaveChanges();

    }
}
