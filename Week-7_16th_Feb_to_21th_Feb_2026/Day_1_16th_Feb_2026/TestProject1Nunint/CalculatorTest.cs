using NUnit.Framework;
using CalculatorApp;

namespace TestProject1Nunit
{
    [TestFixture]
    internal class CalculatorTest
    {
        private Calculator calc;

        [SetUp]
        public void Setup()
        {
            calc = new Calculator();
        }

        [Test]
        public void Add_Test()
        {
            int result = calc.Add(10, 5);
            Assert.That(result,Is.EqualTo(15));
        }

        [Test]
        public void Subtract_Test()
        {
            int result = calc.Substract(10, 5);
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void Multiply_Test()
        {
            int result = calc.Multiply(10, 5);
            Assert.That(result, Is.EqualTo(50));
        }

        [Test]
        public void Divide_Test()
        {
            int result = (int)calc.Divide(10, 5);
            Assert.That(result, Is.EqualTo(2));
        }
    }
}