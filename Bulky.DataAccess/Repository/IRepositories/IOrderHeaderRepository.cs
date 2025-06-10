using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepositories
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);

    }
}
