
using Hangfire;
using Ticket.EventHandler;
using Ticket.Events;

namespace App.Services
{
    public class EventPublisher
    {
        public async Task PublishAsync(StandAddedEvent standEvent)
        {
            BackgroundJob.Enqueue<StandAddedHandler>(handler => handler.HandleAsync(standEvent));
        }

        public async Task PublishAsync(ShowAddedEvent showEvent)
        {
            BackgroundJob.Enqueue<ShowAddedHandler>(handler => handler.HandleAsync(showEvent));
        }
    }
}
