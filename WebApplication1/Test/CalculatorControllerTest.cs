using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace YourNamespace.Tests
{
    public class CalculatorControllerTests
    {
        [Fact]
        public void GetFromJson_ReturnsOkObjectResult()
        {
            // Arrange
            var controller = new CalculatorController(null); // Подставьте настоящий CalcContext при необходимости

            // Act
            var result = controller.GetFromJson();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetFromXml_ReturnsOkObjectResult()
        {
            // Arrange
            var controller = new CalculatorController(null); // Подставьте настоящий CalcContext при необходимости

            // Act
            var result = controller.GetFromXml();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetFromSQLite_ReturnsOkObjectResult()
        {
            // Arrange
            var mockContext = new Mock<CalcContext>();
            mockContext.Setup(c => c.Calculators).Returns(new List<Calculators>());
            var controller = new CalculatorController(mockContext.Object);

            // Act
            var result = controller.GetFromSQLite();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void PerformCalculation_WithValidInput_ReturnsOkObjectResult()
        {
            // Arrange
            var inputModel = new CalculationInputModel { Operation = "+", Operand = 5 };
            var mockContext = new Mock<CalcContext>();
            mockContext.Setup(c => c.Calculators).Returns(new List<Calculators>());
            var controller = new CalculatorController(mockContext.Object);

            // Act
            var result = controller.PerformCalculation(inputModel);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        // Другие тесты по необходимости
    }
}

