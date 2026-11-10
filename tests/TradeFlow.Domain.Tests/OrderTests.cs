using TradeFlow.Domain.Enums;

namespace TradeFlow.Domain.Tests
{
    public class OrderTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Create_InvalidQuantity_ThrowsException(decimal quantity)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                Order.Create(
                    "AAPL",
                    OrderSide.Buy,
                    OrderType.Market,
                    quantity));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_InvalidSymbol_ThrowsException(string? symbol)
        {
            Assert.Throws<ArgumentException>(() =>
                Order.Create(
                    symbol!,
                    OrderSide.Buy,
                    OrderType.Market,
                    10m));
        }

        public static TheoryData<decimal?> InvalidLimitPrices => new()
        {
            null,
            0m,
            -100m
        };

        [Theory]
        [MemberData(nameof(InvalidLimitPrices))]
        public void Create_LimitOrderWithInvalidPrice_ThrowsException(
            decimal? limitPrice)
        {
            Assert.Throws<ArgumentException>(() =>
                Order.Create(
                    "AAPL",
                    OrderSide.Buy,
                    OrderType.Limit,
                    10m,
                    limitPrice));
        }

        [Fact]
        public void Create_MarketOrderWithLimitPrice_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
                Order.Create(
                    "AAPL",
                    OrderSide.Buy,
                    OrderType.Market,
                    10m,
                    250m));
        }

        //Market Order
        [Fact]
        public void Create_ValidMarketOrder_ReturnsPendingOrder()
        {
            // Arrange
            const string symbol = "AAPL";
            const decimal quantity = 10m;

            // Act
            var order = Order.Create(
                symbol,
                OrderSide.Buy,
                OrderType.Market,
                quantity);

            // Assert
            Assert.NotNull(order);
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal("AAPL", order.Symbol);
            Assert.Equal(OrderSide.Buy, order.Side);
            Assert.Equal(OrderType.Market, order.Type);
            Assert.Equal(10m, order.Quantity);
            Assert.Null(order.LimitPrice);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0m, order.FilledQuantity);
            Assert.True(order.CreatedAt <= DateTimeOffset.UtcNow);
        }

        //Limit Order
        [Fact]
        public void Create_ValidLimitOrder_ReturnsPendingOrder()
        {
            // Arrange
            const decimal limitPrice = 250m;

            // Act
            var order = Order.Create(
                "MSFT",
                OrderSide.Sell,
                OrderType.Limit,
                5m,
                limitPrice);

            // Assert
            Assert.NotEqual(Guid.Empty, order.Id);
            Assert.Equal("MSFT", order.Symbol);
            Assert.Equal(OrderSide.Sell, order.Side);
            Assert.Equal(OrderType.Limit, order.Type);
            Assert.Equal(5m, order.Quantity);
            Assert.Equal(limitPrice, order.LimitPrice);
            Assert.Equal(OrderStatus.Pending, order.Status);
            Assert.Equal(0m, order.FilledQuantity);
        }

        [Theory]
        [InlineData("aapl", "AAPL")]
        [InlineData(" AAPL ", "AAPL")]
        [InlineData("msft", "MSFT")]
        public void Create_ValidSymbol_NormalizesSymbol(string input, string expected)
        {
            // Act
            var order = Order.Create(
                input,
                OrderSide.Buy,
                OrderType.Market,
                10m);

            // Assert
            Assert.Equal(expected, order.Symbol);
        }
    }
}
