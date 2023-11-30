namespace WebApiTemplate.Services.Infrastructure.MessageQueue
{
	using MassTransit;

	public class MessageService : IMessageService
	{
		private readonly IBus bus;
		public MessageService(IBus bus)
		{
			this.bus = bus;
		}

		public async Task SendMessage<T>(T obj) where T : class
		{
			await this.bus.Publish(obj);
		}
	}
}
