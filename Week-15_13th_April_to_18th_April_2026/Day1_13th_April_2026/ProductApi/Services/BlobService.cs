using Azure.Storage.Blobs;

namespace ProductApi.Services;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobService(IConfiguration configuration)
    {
        _connectionString = configuration["BlobConnection"] ?? string.Empty;
        _containerName = configuration["BlobContainerName"] ?? "images";
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("BlobConnection is missing in configuration.");
        }

        var containerClient = new BlobContainerClient(_connectionString, _containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobClient.Uri.ToString();
    }
}
