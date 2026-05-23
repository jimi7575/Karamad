using System.Text.Json;
using Catalog.Application.Abstractions;
using Catalog.Application.Books;
using StackExchange.Redis;

namespace Catalog.Infrastructure.Caching;

public sealed class RedisBookCache(IConnectionMultiplexer redis) : IBookCache
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<BookDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(Key(id));
        return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<BookDto>(value!);
    }

    public Task SetAsync(BookDto book, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _database.StringSetAsync(Key(book.Id), JsonSerializer.Serialize(book), ttl);

    public Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
        => _database.KeyDeleteAsync(Key(id));

    private static string Key(Guid id) => $"catalog:books:{id}";
}
