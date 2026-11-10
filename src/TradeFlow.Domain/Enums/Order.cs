using System;
using System.Collections.Generic;
using System.Text;

namespace TradeFlow.Domain.Enums
{
    public sealed class Order
    {
        public Guid Id { get; private set; }

        public string Symbol { get; private set; } = string.Empty;

        public OrderSide Side { get; private set; }

        public OrderType Type { get; private set; }

        public decimal Quantity { get; private set; }

        public decimal? LimitPrice { get; private set; }

        public OrderStatus Status { get; private set; }

        public decimal FilledQuantity { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        private Order(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? limitPrice)
        {
            Id = Guid.NewGuid();
            Symbol = symbol;
            Side = side;
            Type = type;
            Quantity = quantity;
            LimitPrice = limitPrice;
            Status = OrderStatus.Pending;
            FilledQuantity = 0;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Order Create(string symbol, OrderSide side, OrderType type, decimal quantity, decimal? limitPrice = null)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentException(
                    "Symbol is required.", nameof(symbol));

            if (!Enum.IsDefined(side))
                throw new ArgumentOutOfRangeException(nameof(side));

            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type));

            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity), "Quantity must be positive.");

            if (type == OrderType.Limit &&
                (!limitPrice.HasValue || limitPrice.Value <= 0))
            {
                throw new ArgumentException(
                    "Limit orders require a positive limit price.",
                    nameof(limitPrice));
            }

            if (type == OrderType.Market && limitPrice.HasValue)
            {
                throw new ArgumentException(
                    "Market orders must not have a limit price.",
                    nameof(limitPrice));
            }

            return new Order(
                symbol.Trim().ToUpperInvariant(),
                side,
                type,
                quantity,
                limitPrice);
        }
    }
}
