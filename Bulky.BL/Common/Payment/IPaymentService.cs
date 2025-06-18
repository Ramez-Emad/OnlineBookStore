using Bulky.DataAccess.Entities;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Common.Payment
{
    public interface IPaymentService
    {
        Task<Session> PayOrder(IEnumerable<CartItem> cartItems,int orderHeaderId, string successUrl ,  string cancelUrl);
    }
}
