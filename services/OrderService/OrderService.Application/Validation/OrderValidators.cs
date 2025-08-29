using FluentValidation;
using OrderService.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Validation
{
    public class OrderItemCreateValidator : AbstractValidator<OrderItemCreateDto>
    {
        public OrderItemCreateValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0);
            RuleFor(x => x.Quantity).GreaterThan(0);
            RuleFor(x => x.UnitPrice).GreaterThan(0);
        }
    }


    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleForEach(x => x.Items).SetValidator(new OrderItemCreateValidator());
        }
    }


    public class OrderStatusUpdateValidator : AbstractValidator<OrderStatusUpdateDto>
    {
        public OrderStatusUpdateValidator()
        {
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
