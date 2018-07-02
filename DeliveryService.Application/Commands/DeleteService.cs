﻿using DeliveryService.Application.Core;
using MediatR;

namespace DeliveryService.Application.Commands
{
	public class UpdateService : IRequest<Response>
    {
		public int Id { get; private set;}
		public string Name { get; private set;}
		public int Time { get; private set;}
		public int Cost { get; private set;}
		      
		public UpdateService(int id, string name, int time, int cost)
        {
			Id = id;
            Name = name;
            Time = time;
            Cost = cost;
        }
  
    }
}