﻿using CqrsDemo.Application.Commands;
using CqrsDemo.Infrastructure.Persistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Application.Handlers.Commands.Order
{
    public class CalculateDiscountHandler : IRequestHandler<CalculateDiscountCommand, decimal>
    {
        private readonly ReadDbContext _context;

        public CalculateDiscountHandler(ReadDbContext context)
        {
            _context = context;
        }
        public Task<decimal> Handle(CalculateDiscountCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Quantity switch
            {
                var q when q >= 100 => 0.5m,
                var q when q >= 50 => 0.3m,
                var q when q >= 10 => 0.1m,
                _ => 0m
            });
        }
    }
}
