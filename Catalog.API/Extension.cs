using Catalog.API.Dtos;
using Catalog.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API
{
    public static class Extension
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
