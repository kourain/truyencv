using TruyenCV.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

public static partial class RedisExtensions
{
    public static string ToRedisCache<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
    public static T FromRedisCacheToObject<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json)!;
    }
    /// <summary>
    /// Cache 1 Entity có key là id của Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="Entity"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns></returns>
    public static async Task AddOrUpdateInRedisAsync<T>(this IDistributedCache RedisCache, T Entity, double CacheMinutes) where T : BaseEntity
        => await RedisCache.AddOrUpdateInRedisAsync(Entity, $"{Entity.id}", CacheMinutes);
    /// <summary>
    /// Cache 1 Entity với điều kiện cụ thể, có key cụ thể
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="Entity"></param>
    /// <param name="Key"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns></returns>
    public static async Task AddOrUpdateInRedisAsync<T>(this IDistributedCache RedisCache, T Entity, object Key, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            await RedisCache.SetStringAsync($"{typeof(T).Name}:one:{Key}", Entity.ToRedisCache(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes), // Cache hết hạn sau 5 phút
                SlidingExpiration = TimeSpan.FromMinutes(CacheMinutes) // Gia hạn cache sau 5 phút
            });
        }
        catch (Exception ex)
        {
            Serilog.Log.Error($"Redis error in AddOrUpdateInRedisAsync: {ex.Message}");
        }
    }
    public static async Task RemoveFromRedisAsync<T>(this IDistributedCache RedisCache, T Entity) where T : BaseEntity
    {
        await RedisCache.RemoveAsync($"{typeof(T).Name}:one:{Entity.id}");
    }
    public static async Task RemoveFromRedisAsync<T>(this IDistributedCache RedisCache, long Key) where T : BaseEntity
    {
        await RedisCache.RemoveAsync($"{typeof(T).Name}:one:{Key}");
    }
    public static async Task RemoveFromRedisAsync<T>(this IDistributedCache RedisCache, string Key) where T : BaseEntity
    {
        await RedisCache.RemoveAsync($"{typeof(T).Name}:{Key}");
    }
    /// <summary>
    /// Get Cache 1 Entity có key là id của Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="QueryOne"></param>
    /// <param name="Key"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns></returns>
    public static async Task<T?> GetFromRedisAsync<T>(this IDistributedCache RedisCache, Task<T?> QueryOne, object Key, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            if (Key != null && await RedisCache.GetStringAsync($"{typeof(T).Name}:one:{Key}") is string redisValue)
            {
                return redisValue.FromRedisCacheToObject<T>();
            }
        }
        catch (Exception ex)
        {
            // Ghi log lỗi Redis nhưng không ảnh hưởng đến luồng chính
            Serilog.Log.Error($"Redis error in GetFromRedisAsync: {ex.Message}");
        }

        if (QueryOne == null) return default;
        var QueryResult = await QueryOne;

        if (QueryResult != null)
        {
            try
            {
                await RedisCache.AddOrUpdateInRedisAsync(QueryResult, Key, CacheMinutes);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error($"Redis error in AddOrUpdateInRedisAsync: {ex.Message}");
            }
        }

        return QueryResult;
    }
    /// <summary>
    /// Get Cache toàn bộ bảng, sử dụng rất hạn chế vì có thể dữ liệu quá lớn
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="QueryMany"></param>
    /// <param name="Key"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns>IEnumerable<T>? toàn bộ bảng</returns>
    public static async Task<IEnumerable<T>> GetFromRedisAsync<T>(this IDistributedCache RedisCache, Task<List<T>>? QueryMany, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            if (await RedisCache.GetStringAsync($"{typeof(T).Name}:all") is string redisValue)
            {
                IEnumerable<long> ids = redisValue.FromRedisCacheToObject<IEnumerable<long>>();
                return await Task.WhenAll(ids.Select(id => RedisCache.GetFromRedisAsync<T>(QueryOne: null, id, CacheMinutes)).Where(m => m is not null)) ?? [];
            }
            if (QueryMany == null) return [];
            var QueryResult = await QueryMany;
            if (QueryResult != null)
            {
                await RedisCache.AddOrUpdateInRedisAsync<T>(QueryResult, CacheMinutes);
                return QueryResult;
            }
            return [];
        }
        catch (Exception)
        {
            Serilog.Log.Error("Redis error in GetFromRedisAsync with pagination");
            return [];
        }
    }
    ///<summary>
    ///  Cache toàn bộ bảng, sử dụng rất hạn chế vì có thể dữ liệu quá lớn
    ///</summary>
    ///<typeparam name="T"></typeparam>
    ///<param name="RedisCache"></param>
    /// <param name="Entitys"></param>
    ///<param name="CacheMinutes"></param>
    public static async Task AddOrUpdateInRedisAsync<T>(this IDistributedCache RedisCache, IEnumerable<T> Entitys, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            await RedisCache.SetStringAsync($"{typeof(T).Name}:all", Entitys.Select(u => u.id).ToRedisCache(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes), // Cache hết hạn sau 5 phút
                SlidingExpiration = TimeSpan.FromMinutes(CacheMinutes) // Gia hạn cache sau 5 phút
            });
            await Task.WhenAll(Entitys.Select(entity => RedisCache.AddOrUpdateInRedisAsync(entity, CacheMinutes)));
        }
        catch (Exception)
        {
            Serilog.Log.Error("Redis error in AddOrUpdateInRedisAsync with pagination");
        }
    }
    /// <summary>
    /// Get Cache với phân trang, sử dụng key: many:pos:limit
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="QueryMany"></param>
    /// <param name="Pos"></param>
    /// <param name="Limit"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> GetFromRedisAsync<T>(this IDistributedCache RedisCache, Task<List<T>>? QueryMany, long Pos, long Limit, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            if (await RedisCache.GetStringAsync($"{typeof(T).Name}:many:{Pos}:{Limit}") is string redisValue)
            {
                IEnumerable<long> ids = redisValue.FromRedisCacheToObject<IEnumerable<long>>();
                return await Task.WhenAll(ids.Select(id => RedisCache.GetFromRedisAsync<T>(QueryOne: null, id, CacheMinutes)).Where(m => m is not null)) ?? [];
            }
            if (QueryMany == null) return [];
            var QueryResult = await QueryMany;
            if (QueryResult != null)
            {
                await RedisCache.AddOrUpdateInRedisAsync<T>(QueryResult, Pos, Limit);
                return QueryResult;
            }
            return [];
        }
        catch (Exception)
        {
            Serilog.Log.Error("Redis error in GetFromRedisAsync with pagination");
            return [];
        }
    }
    /// <summary>
    /// Cache với phân trang, sử dụng key: many:pos:limit
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="Entitys"></param>
    public static async Task AddOrUpdateInRedisAsync<T>(this IDistributedCache RedisCache, IEnumerable<T> Entitys, long Pos, long Limit, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            await RedisCache.SetStringAsync($"{typeof(T).Name}:many:{Pos}:{Limit}", Entitys.Select(u => u.id).ToRedisCache(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes), // Cache hết hạn sau 5 phút
                SlidingExpiration = TimeSpan.FromMinutes(CacheMinutes) // Gia hạn cache sau 5 phút
            });
            await Task.WhenAll(Entitys.Select(entity => RedisCache.AddOrUpdateInRedisAsync(entity, CacheMinutes)));
        }
        catch (Exception)
        {
            Serilog.Log.Error("Redis error in AddOrUpdateInRedisAsync with pagination");
        }
    }
    /// <summary>
    /// Get Cache với điều kiện cụ thể, có key cụ thể
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="QueryMany"></param>
    /// <param name="Key"></param>
    /// <param name="CacheMinutes"></param>
    /// <returns>
    /// Toàn bộ dữ liệu dạng mảng IEnumerable<T>? với Key tuỳ chỉnh
    /// </returns>
    public static async Task<IEnumerable<T>> GetFromRedisAsync<T>(this IDistributedCache RedisCache, Task<List<T>>? QueryMany, object Key, double CacheMinutes) where T : BaseEntity
    {
        if (await RedisCache.GetStringAsync($"{typeof(T).Name}:{Key}") is string redisValue)
        {
            IEnumerable<object> ids = redisValue.FromRedisCacheToObject<IEnumerable<object>>();
            return await Task.WhenAll(ids.Select(id => RedisCache.GetFromRedisAsync<T>(QueryOne: null, id, CacheMinutes)).Where(m => m is not null)) ?? [];
        }
        if (QueryMany == null) return [];
        var QueryResult = await QueryMany;
        if (QueryResult != null)
        {
            await RedisCache.AddOrUpdateInRedisAsync<T>(QueryResult, Key, CacheMinutes);
            return QueryResult;
        }
        return [];
    }
    /// <summary>
    /// Cache với điều kiện cụ thể, có key cụ thể
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="RedisCache"></param>
    /// <param name="Entitys"></param>
    /// <param name="Key"> string | int | long </param>
    /// <param name="CacheMinutes"></param>
    /// <returns></returns>
    public static async Task AddOrUpdateInRedisAsync<T>(this IDistributedCache RedisCache, IEnumerable<T> Entitys, object Key, double CacheMinutes) where T : BaseEntity
    {
        try
        {
            await RedisCache.SetStringAsync($"{typeof(T).Name}:{Key}", Entitys.ToRedisCache(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheMinutes), // Cache hết hạn sau 5 phút
                SlidingExpiration = TimeSpan.FromMinutes(CacheMinutes) // Gia hạn cache sau 5 phút
            });
        }
        catch (Exception ex)
        {
            Serilog.Log.Error("Redis error in AddOrUpdateInRedisAsync with pagination", ex);
        }
    }
}