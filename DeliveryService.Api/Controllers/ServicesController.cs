﻿using DeliveryService.Application.Commands;
using DeliveryService.Application.Domain.Interfaces;
using DeliveryService.Application.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryService.Api.Controllers
{
    [Route("api/[controller]")]
	public class ServiceController : Controller
    {
		private readonly IMediator mediator;
		private readonly IServiceRepository serviceRepository;      
		private readonly IRouteRepository routeRepository;
        
		public ServiceController(
			IMediator mediator, 
			IServiceRepository serviceRepository,
			IRouteRepository routeRepository
		)
        {
            this.mediator = mediator;
			this.serviceRepository = serviceRepository;
			this.routeRepository = routeRepository;
        }

		[HttpGet]
		public IActionResult Get(){
			try
   			{
				var result = serviceRepository.Query().ToList();

				result.ForEach(x =>
				{
					x.AddRoutes(routeRepository.Query().Where(r => r.ServiceOriginId == x.Id).ToList());
				});

				return Ok(result);
		    }
			catch (System.Exception ex)
			{
				return BadRequest(ex.Message);
			}         
		}



		[HttpGet]
        [Route("{id}")]
		public async Task<IActionResult> GetById(int  id) {
            try
            {
				var result = await serviceRepository.GetAsync(id);

				return Ok(result);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }      

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateService command)
        {
            var response = await mediator.Send(command).ConfigureAwait(false);

            if (response.Errors.Any())
            {
                return BadRequest(response.Errors);
            }
            
            return Ok(response.Result);
        }

		[HttpPut]
		public async Task<IActionResult> UpdateService([FromBody] UpdateService command)
        {
            var response = await mediator.Send(command).ConfigureAwait(false);

            if (response.Errors.Any())
            {
                return BadRequest(response.Errors);
            }

            return Ok(response.Result);
        }

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteService(int id)
        {
			var response = await mediator.Send(new DeleteService(id)).ConfigureAwait(false);

            if (response.Errors.Any())
            {
                 BadRequest(response.Errors);
            }

            return Ok(response.Result);
        }

		[HttpGet]
		[Route("GetShortestPath/{originId}/{destinationId}")]
		public async Task<IActionResult> GetShortestPath(int originId, int destinationId)
        {
			var response = await mediator.Send(new GetShortestPath(originId,destinationId)).ConfigureAwait(false);

            if (response.Errors.Any())
            {
                BadRequest(response.Errors);
            }

            return Ok(response.Result);
        }
    }
}
