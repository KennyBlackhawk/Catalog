using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Dtos
{
    public record ItemDto(Guid Id, string Name, string Decription, decimal Price, DateTimeOffset CreatedDate);

    public record CreateItemDto([Required]string Name, string Description, [Range(1, 1000)]decimal Price);

    public record UpdateItemDto([Required] string Name, string Description, [Range(1, 1000)] decimal Price);
}
