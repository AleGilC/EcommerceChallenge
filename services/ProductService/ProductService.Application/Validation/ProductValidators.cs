using FluentValidation;
using ProductService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.Validation
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        }
    }


    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        }
    }
}
