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


        [Fact]
        public void Subtract_SameCurrency_ReturnsDifference()
        {
            var money = new Money(1000, "TRY");
            var money2 = new Money(300, "TRY");
            var expected = new Money(700, "TRY");
            var subtractedVal = money.Subtract(money2);

            Assert.Equal(expected, subtractedVal);
        }

        [Fact]
        public void Subtract_DifferentCurrency_Throws()
        {
            var money = new Money(1000, "TRY");
            var money2 = new Money(300, "USD");

            Assert.Throws<ArgumentException>(() => money.Subtract(money2));
        }

        [Fact]
        public void IsGreaterThan_LargerAmount_ReturnsTrue()
        {
            var money = new Money(1000, "TRY");
            var money2 = new Money(300, "TRY");

            Assert.True(money.IsGreaterThan(money2));
        }

        [Fact]
        public void IsGreaterThan_EqualAmount_ReturnsFalse()
        {
            var money = new Money(500, "TRY");
            var money2 = new Money(500, "TRY");

            Assert.False(money.IsGreaterThan(money2));
        }
        [Fact]
        public void IsGreaterThan_DifferentCurrency_Throws()
        {
            var money = new Money(500, "TRY");
            var money2 = new Money(100, "USD");

            Assert.Throws<ArgumentException>(() => money.IsGreaterThan(money2));
        }
    }
}
