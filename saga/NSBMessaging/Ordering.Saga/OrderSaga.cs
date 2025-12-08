using Contracts.Commands;
using Contracts.Events;
using Ordering.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Saga
{
    public class OrderSaga :
        Saga<OrderSagaData>,
        IAmStartedByMessages<OrderCreated>,
        IHandleMessages<OrderPaid>,
        IHandleMessages<CancelationRequested>,
        IHandleMessages<OrderCanceled>,
        IHandleMessages<RefundOrder>,
        IHandleMessages<OrderCompleted>,
        IHandleTimeouts<PaymentTimeoutExpired>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
        {
            mapper.MapSaga(s => s.OrderId)
                .ToMessage<OrderCreated>(m => m.OrderId)
                .ToMessage<OrderPaid>(m => m.OrderId)
                .ToMessage<CancelationRequested>(m=>m.OrderId)
                .ToMessage<OrderCanceled>(m=>m.OrderId)
                .ToMessage<RefundOrder>(m=>m.OrderId)
                .ToMessage<OrderCompleted>(m=>m.OrderId);
        }

        public async Task Handle(OrderCreated message, IMessageHandlerContext context)
        {
            Data.Amount = message.TotalAmount;
            Data.CreatedAt = message.CreatedAt;
            Data.OrderStatus = Domain.Entities.OrderStatus.Pending;
            Data.CurrentState = nameof(OrderStatus.Pending);
            Console.WriteLine($"[OrderSaga] OrderCreated {message.OrderId}, amount {message.TotalAmount}");
            await RequestTimeout(context, TimeSpan.FromSeconds(30), new PaymentTimeoutExpired { OrderId = Data.OrderId });
        }


        public async Task Handle(OrderPaid message, IMessageHandlerContext context)
        {

            if (Data.OrderStatus is OrderStatus.Canceled or OrderStatus.Completed)
                return;

            Data.PaidAt = DateTime.UtcNow;
            Data.Amount = message.AmountPaid;
            Data.IsBilled = false;
            Data.OrderStatus = OrderStatus.Paid;
            Data.CurrentState = nameof(OrderStatus.Paid);

            Console.WriteLine($"[OrderSaga] Paid {message.OrderId} amount {message.AmountPaid}");
            await context.Publish(new InvoiceNeeded()
            {
                OrderId = Data.OrderId,
                TotalAmount = Data.Amount,
                VAT = Math.Round(Data.Amount * 0.21m, 2)
            });
        }

        public Task Timeout(PaymentTimeoutExpired state, IMessageHandlerContext context)
        {

            if (Data.OrderStatus is OrderStatus.Paid or OrderStatus.Canceled or OrderStatus.Completed)
                return Task.CompletedTask;

            Console.WriteLine($"[OrderSaga] Payment timeout for {state.OrderId}. Moving to AwaitingPayment.");
            Data.OrderStatus = OrderStatus.AwaitingPayment;
            Data.CurrentState = nameof(OrderStatus.AwaitingPayment);

            return Task.CompletedTask;

        }

        public async Task Handle(CancelationRequested message, IMessageHandlerContext context)
        {

            if (Data.OrderStatus == OrderStatus.Canceled)
                return;

            Console.WriteLine($"[OrderSaga] Cancel request for {message.OrderId}. Current: {Data.OrderStatus}");

            Data.CanceledAt = DateTime.UtcNow;
            Data.OrderStatus = OrderStatus.Canceled;
            Data.CurrentState = nameof(OrderStatus.Canceled);

            await context.Publish(new OrderCanceled { OrderId = Data.OrderId });


            await Task.CompletedTask;
        }

        public async Task Handle(OrderCanceled message, IMessageHandlerContext context)
        {

            if (Data.PaidAt.HasValue && Data.OrderStatus == OrderStatus.Canceled)
            {
                Data.OrderStatus = OrderStatus.AwaitingRefund;
                Data.CurrentState = nameof(OrderStatus.AwaitingRefund);
                await context.SendLocal(new RefundOrder { OrderId = Data.OrderId });
            }

        }

        public async Task Handle(RefundOrder message, IMessageHandlerContext context)
        {
            Data.OrderStatus = OrderStatus.Refunded;
            Data.CurrentState = nameof(OrderStatus.Refunded);
            await context.Publish(new OrderCompleted { OrderId = Data.OrderId });
        }

        public Task Handle(OrderCompleted message, IMessageHandlerContext context)
        {

            Data.OrderStatus = OrderStatus.Completed;
            Data.CurrentState = nameof(OrderStatus.Completed);

            Console.WriteLine($"[OrderSaga] Completed {message.OrderId}");
            MarkAsComplete();
            return Task.CompletedTask;
        }
    }
}
