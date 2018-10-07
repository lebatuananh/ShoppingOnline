using System.ComponentModel;

namespace ShoppingOnline.Data.Enum
{
    public enum BillStatus
    {
        [Description("New bill")]
        New,

        [Description("In Progress")]
        InProgress,

        [Description("Returned")]
        Returned,

        [Description("Cancelled")]
        Cancelled,

        [Description("Completed")]
        Completed
    }
}