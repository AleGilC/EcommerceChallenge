using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Dtos
{
    public record ProductCreateDto(string Name, string Description, decimal Price, int Stock);
    public record ProductUpdateDto(string Name, string Description, decimal Price, int Stock);
    public record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
}
