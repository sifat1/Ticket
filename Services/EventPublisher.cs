
using Hangfire;
using Ticket.EventHandler;
using Ticket.Events;

namespace App.Services{
public class EventPublisher
{
    public void PublishAsync(StandAddedEvent standEvent)
    {
        BackgroundJob.Enqueue<StandAddedHandler>(handler => handler.HandleAsync(standEvent));
    }

    public void PublishAsync(ShowAddedEvent showEvent)
    {
        BackgroundJob.Enqueue<ShowAddedHandler>(handler => handler.HandleAsync(showEvent));
    }
}
}
