using Catalog.API.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public interface IItemsRepository
    {
        Task<Item> GetItemAsync(Guid id);
        Task<IEnumerable<Item>> GetItemsAsync();
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item);
        Task DeleteItemAsync(Guid id);
    }
}