using System.ComponentModel;

namespace Ordering.Domain.Entities
{
    public enum OrderStatus
    {
        [Description("Received")]//think about these as temporary orders
        Received,

        [Description("Created")]
        Created,

        [Description("Pending")]
        Pending,

        [Description("Canceled")]
        Canceled,
        //PayOrder command is sent
        //OrderPayed event is sent
        [Description("Paid")]
        Paid,

        //time-boxed- if not paid in x mins, becomes awaiting payment
        [Description("AwaitingPayment")]
        AwaitingPayment,


        [Description("ReadyForShipping")]
        ReadyForShipping,

        [Description("Shipped")]
        Shipped,

        [Description("Delivered")]
        Delivered,

        [Description("Completed")]
        Completed,
        [Description("Refunded")]
        Refunded,
        [Description("AwaitingRefund")]
        AwaitingRefund

    }
}
