using Catalog.API;
using Catalog.API.Controllers;
using Catalog.API.Dtos;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemControllerTests
    {
        private readonly Mock<IItemsRepository> repositoryStub = new();
        private readonly Mock<ILogger<ItemController>> loggerStub = new();
        private readonly Random ran = new();

        [Fact]
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            //Arrange
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Item)null);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(Guid.NewGuid());

            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            //Arrange
            var expectedItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedItem);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.GetItemAsync(expectedItem.Id);

            //Assert
            result.Value.Should().BeEquivalentTo(expectedItem.AsDto());
        }

        [Fact]
        public async Task GetItemAsync_WithExistingItems_ReturnsAllItems()
        {
            //Arrange
            var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            List<ItemDto> convertedItems = new();
            expectedItems.ToList().ForEach(m => convertedItems.Add(m.AsDto()));

            repositoryStub.Setup(repo => repo.GetItemsAsync())
                .ReturnsAsync(expectedItems);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var actualItems = await controller.GetItemsAsync();

            //Assert
            actualItems.Should().BeEquivalentTo(convertedItems.AsEnumerable<ItemDto>());
        }

        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnsCreatedItem()
        {
            //Arrange
            var itemToCreate = new CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), ran.Next(1000));

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.CreateItemAsync(itemToCreate);

            //Assert
            var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;

            itemToCreate.Should().BeEquivalentTo(
                createdItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );

            createdItem.Id.Should().NotBeEmpty();
            createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1000));
        }

        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            Item existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
               .ReturnsAsync(existingItem);
            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            //Arrange
            Item existingItem = CreateRandomItem();
            repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
               .ReturnsAsync(existingItem);

            var itemId = existingItem.Id;

            var controller = new ItemController(repositoryStub.Object, loggerStub.Object);

            //Act
            var result = await controller.DeleteItemAsync(itemId);

            //Assert
            result.Should().BeOfType<NoContentResult>();
        }

        private Item CreateRandomItem()
        {
            return new Item()
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Price = ran.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
