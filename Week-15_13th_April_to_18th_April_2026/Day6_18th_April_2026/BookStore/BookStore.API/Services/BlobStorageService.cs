using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace BookStore.API.Services;

public sealed class BlobStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<BlobStorageService> _logger;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadBookImageAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        var connectionString = _configuration["AzureBlob:ConnectionString"];
        var containerName = _configuration["AzureBlob:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException("Azure Blob configuration is missing. Set AzureBlob:ConnectionString and AzureBlob:ContainerName.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = ".bin";
        }

        var blobName = $"books/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

        var containerClient = new BlobContainerClient(connectionString, containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: cancellationToken);

        _logger.LogInformation("Uploaded image to blob storage: {BlobName}", blobName);
        return await GetAccessibleImageUrlAsync(blobClient.Uri.ToString(), cancellationToken);
    }

    public async Task<string> GetAccessibleImageUrlAsync(string? imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return string.Empty;
        }

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
        {
            return imageUrl;
        }

        if (!uri.Host.Contains("blob.core.windows.net", StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl;
        }

        if (uri.Query.Contains("sig=", StringComparison.OrdinalIgnoreCase))
        {
            return imageUrl;
        }

        var connectionString = _configuration["AzureBlob:ConnectionString"];
        var containerName = _configuration["AzureBlob:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(containerName))
        {
            return imageUrl;
        }

        var blobPath = uri.AbsolutePath.TrimStart('/');
        if (blobPath.StartsWith(containerName + "/", StringComparison.OrdinalIgnoreCase))
        {
            blobPath = blobPath[(containerName.Length + 1)..];
        }

        if (string.IsNullOrWhiteSpace(blobPath))
        {
            return imageUrl;
        }

        var containerClient = new BlobContainerClient(connectionString, containerName);
        var blobClient = containerClient.GetBlobClient(blobPath);

        if (!blobClient.CanGenerateSasUri)
        {
            return imageUrl;
        }

        var sasBuilder = new BlobSasBuilder(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddYears(5))
        {
            BlobContainerName = containerName,
            BlobName = blobPath,
            Resource = "b"
        };

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }
}
