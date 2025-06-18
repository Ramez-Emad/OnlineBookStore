using Azure;
using Bulky.BL.Services.Orders;
using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using Stripe.Checkout;


namespace Bulky.BL.Common.Payment
{
    public class PaymentService(IOrderServices _orderServices) : IPaymentService
    {
        private readonly string domain = "https://localhost:7179/";
        
        public async Task<Session> PayOrder(IEnumerable<CartItem> cartItems, int orderHeaderId, string successUrl, string cancelUrl)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = successUrl ,
                CancelUrl = cancelUrl,
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            
            foreach (var item in cartItems)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Title
                        }
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionLineItem);             
               
            }

            var service = new SessionService();
            Session session = service.Create(options);

           await  _orderServices.UpdateStripePaymentID(orderHeaderId, session.Id, session.PaymentIntentId);

            return session;

        }
    }
}
