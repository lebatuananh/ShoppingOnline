using System.ComponentModel;

namespace ShoppingOnline.Data.Enum
{
    public enum PaymentMethod
    {
        [Description("Cash on delivery")]
        CashOnDelivery,

        [Description("Onlin Banking")]
        OnlinBanking,

        [Description("Payment Gateway")]
        PaymentGateway,

        [Description("Visa")]
        Visa,

        [Description("Master Card")]
        MasterCard,

        [Description("PayPal")]
        PayPal,

        [Description("Atm")]
        Atm
    }
}