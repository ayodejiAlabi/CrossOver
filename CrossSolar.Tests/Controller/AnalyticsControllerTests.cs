using CrossSolar.Controllers;
using CrossSolar.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CrossSolar.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;
namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        public AnalyticsControllerTests()
        {
            _AnalyticsController = new AnalyticsController(_AnalyticsRepositoryMock.Object, _panelRepositoryMock.Object);
        }
       
        private readonly AnalyticsController _AnalyticsController;

        private readonly Mock<IAnalyticsRepository> _AnalyticsRepositoryMock = new Mock<IAnalyticsRepository>();
        private readonly Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();

        private string PanelID = "1234rf";

        [Fact]
        public async Task GetPanelData()
        {
            //string PanelID = "123AS";
            try
            {
                var result = await _AnalyticsController.Get(PanelID);
                // Assert
                Assert.NotNull(result);

                var createdResult = result as CreatedResult;
                Assert.NotNull(createdResult);
                Assert.Equal(201, createdResult.StatusCode);
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        [Fact]
        public async Task GetPanelDataDaysResult()
        {
            //string PanelID = "123AS";
            try
            {
                var result = await _AnalyticsController.DayResults(PanelID);
                // Assert
                Assert.NotNull(result);

                var createdResult = result as CreatedResult;
                Assert.NotNull(createdResult);
                Assert.Equal(201, createdResult.StatusCode);
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        [Fact]
        public async Task PostPanelData()
        {
            try
            {
                var panelData = new OneHourElectricityModel
                {
                    Id = 123,
                    KiloWatt = 5000,
                    DateTime = DateTime.Now

                };

                // Arrange

                // Act
                var result = await _AnalyticsController.Post(PanelID, panelData);

                // Assert
                Assert.NotNull(result);

                var createdResult = result as CreatedResult;
                Assert.NotNull(createdResult);
                Assert.Equal(201, createdResult.StatusCode);
            }
            catch (Exception)
            {

                throw;
            }
         
        }
    }
}
