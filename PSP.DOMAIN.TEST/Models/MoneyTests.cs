using PSP.DOMAIN.Models;

namespace PSP.DOMAIN.TEST.Models
{
    public class MoneyTests
    {
        [Fact]
        public void Constructor_ValidInput_CreatsMoney()
        {
            var money = new Money(1050, "TRY");

            Assert.Equal(1050, money.Amount);
            Assert.Equal("TRY", money.Currency);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("try")]
        [InlineData("TRYX")]
        [InlineData("TR")]
        [InlineData("T1Y")]
        public void Constructor_InvalidCurrency_Throws(string currency)
        {
            Assert.Throws<ArgumentException>(() => new Money(100, currency));
        }

        [Fact]
        public void Add_SomeCurrency_ReturnsSum()
        {
            var a = new Money(100, "TRY");
            var b = new Money(50, "TRY");

            var result = a.Add(b);

            Assert.Equal(new Money(150, "TRY"), result);
        }

        [Fact]
        public void Add_SomeCurrency_DoesNotChangeOriginals()
        {
            var a = new Money(100, "TRY");
            var b = new Money(50, "TRY");

            var result = a.Add(b);

            Assert.Equal(100, a.Amount);
            Assert.Equal(50, b.Amount);
        }


        [Fact]
        public void Add_DifferentCurrency_Throws()
        {
            var a = new Money(100, "TRY");
            var b = new Money(50, "USD");
            Assert.Throws<ArgumentException>(() => a.Add(b));

        }

        [Fact]
        public void Equals_SameAmountAndCurrency_AreEqual()
        {
            var a = new Money(100, "TRY");
            var b = new Money(100, "TRY");
            Assert.Equal(a, b);
            Assert.True(a == b);
        }

        [Fact]
        public void Equals_DifferentCurrency_AreNotEqual()
        {
            Assert.NotEqual(new Money(100, "TRY"), new Money(100, "USD"));
        }

        [Fact]
        public void Add_Overflow_Throws()
        {
            var max = new Money(long.MaxValue, "TRY");
            var one = new Money(1, "TRY");

            Assert.Throws<OverflowException>(() => max.Add(one));
        }
    }
}
