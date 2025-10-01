using Restaurants.Domain.Interfaces;
using Azure.Storage.Blobs;


using Microsoft.Extensions.Options;
using Restaurants.Infrastructure.Configuration;
using Azure.Storage.Sas;

namespace Restaurants.Infrastructure.Storage;
public class BlobStorageService(IOptions<BlobStorageSettings> blobStorageSettingsOptions) : IBlobStorageService
{
    private readonly BlobStorageSettings _blobStorageSettings = blobStorageSettingsOptions.Value;
    public async Task<string> UploadToBlobAsync(Stream data, string fileName)
    {
        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_blobStorageSettings.LogosContainerName);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(data);

        var blobUrl = blobClient.Uri.ToString();

        return blobUrl;
    }

    public string? GetBlobSasUrl(string? blobUrl)
    {
        if (blobUrl == null) return null;

        var sasBuilder = new BlobSasBuilder()
        {
            BlobContainerName = _blobStorageSettings.LogosContainerName,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow,
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30),
            BlobName = GetBlobNameFromUrl(blobUrl)     // "logos/logo.png"
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var blobServiceClient = new BlobServiceClient(_blobStorageSettings.ConnectionString);

        var sasToken = sasBuilder
            .ToSasQueryParameters(new Azure.Storage.StorageSharedKeyCredential(
                blobServiceClient.AccountName, _blobStorageSettings.AccountKey))
            .ToString();

        // bloburl : https://yrestaurantssadev.blob.core.windows.net/logos/logo2.png
        // sas: sp=r&st=2025-09-30T17:28:48Z&se=2025-10-01T01:43:48Z&spr=https&sv=2024-11-04&sr=b&sig=TLYc%2B2JIBD6BKUgJ%2BeAf%2FuDxMjY3pJGymlmn5qYaOBk%3D

        return  $"{blobUrl}?{sasToken}";

    }

    private string GetBlobNameFromUrl(string blobUrl)
    {
        var uri = new Uri(blobUrl);
        return uri.Segments.Last();


    }


}
