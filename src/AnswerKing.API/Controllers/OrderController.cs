using System.Collections.Generic;
using System.Threading.Tasks;
using AnswerKing.Services;
using AnswerKing.Services.DTOs;
using AnswerKing.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnswerKing.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IItemService _itemService;

        public OrderController(IOrderService orderService, IItemService itemService)
        {
            this._orderService = orderService;
            this._itemService = itemService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderDto>))]
        public async Task<IActionResult> GetAll()
        {
            var orderDtos = await this._orderService.GetAll();
            
            return this.Ok(orderDtos);
        }
        
        [HttpGet("{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int orderId)
        {
            var orderDto = await this._orderService.GetById(orderId);

            if (orderDto is null)
            {
                return this.NotFound();
            }

            return this.Ok(orderDto);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState.ValidationState);
            }

            var orderDto = await this._orderService.Create(createDto);

            return this.Ok(orderDto);
        }
        
        [HttpPost("{orderId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddItemToOrder(int orderId, int itemId)
        {
            var orderExists = await this._orderService.Exists(orderId);
            var itemExists = await this._itemService.Exists(itemId);
            if (!orderExists || !itemExists)
            {
                return this.NotFound();
            }
            
            if (!await this._orderService.AddItem(orderId, itemId))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }
        
        [HttpDelete("{orderId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItemFromOrder(int orderId, int itemId)
        {
            var orderExists = await this._orderService.Exists(orderId);
            var itemExists = await this._itemService.Exists(itemId);
            if (!orderExists || !itemExists)
            {
                return this.NotFound();
            }

            if (!await this._orderService.RemoveItem(orderId, itemId))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }

        [HttpPut("{orderId}/items/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderItem(int orderId, int itemId, [FromBody] OrderItemUpdateDto updateDto)
        {
            var orderExists = await this._orderService.Exists(orderId);
            var itemExists = await this._itemService.Exists(itemId);
            if (!orderExists || !itemExists)
            {
                return this.NotFound();
            }

            if (!await this._orderService.UpdateOrderItem(orderId, itemId, updateDto))
            {
                return this.BadRequest();
            }

            return this.NoContent();
        }
    }
}