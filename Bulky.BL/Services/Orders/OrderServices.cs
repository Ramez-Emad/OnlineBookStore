using Bulky.DataAccess.Entities;
using Bulky.DataAccess.Exceptions;
using Bulky.DataAccess.Repository.Orders;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using Bulky.DataAccess.UnitOfWork.UnitOfWork.UnitOfWork;
using Bulky.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.BL.Services.Orders
{
    public class OrderServices(IUnitOfWork _unitOfWork) : IOrderServices
    {
        public async Task CreateOrder(OrderHeader orderHeader, Cart cart, bool IsCompany)
        {
            orderHeader.OrderDate = DateTime.Now;
            orderHeader.OrderTotal = cart.TotalCost;

            if (IsCompany == false)
            {
                orderHeader.PaymentStatus = SD.PaymentStatusPending;
                orderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                orderHeader.OrderStatus = SD.StatusApproved;
            }

            await _unitOfWork.OrderHeaderRepository.AddAsync(orderHeader);
            await _unitOfWork.SaveChangesAsync(); 

            foreach (var item in cart.Items)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = orderHeader.Id,
                    Price = item.Price,
                    Count = item.Quantity
                };
                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderHeader>> GetAllOrdersAsync()
        {
            return await _unitOfWork.OrderHeaderRepository.GetAllAsync(order => order.ApplicationUser);
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByIdAsync(int id)
        {
            var items = await _unitOfWork.OrderDetailRepository.GetAllAsync(o => o.Product);
            items = items.Where(x => x.OrderHeaderId == id);
            return items;         
        }

        public async Task<OrderHeader> GetOrderHeaderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(id, order => order.ApplicationUser);
            if (order == null)
                throw new OrderHeaderNotFoundException(id);
            return order;
        }

        public async Task UpdateOrderHeader(OrderHeader orderHeader)
        {
            if (orderHeader == null)
                throw new BadRequestException(["Order Header data is required."]);

            var orderHeaderFromDb = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(orderHeader.Id);

            if (orderHeaderFromDb == null)
                throw new OrderHeaderNotFoundException(orderHeader.Id);

            orderHeaderFromDb.Name = orderHeader.Name ?? orderHeaderFromDb.Name;
            orderHeaderFromDb.PhoneNumber = orderHeader.PhoneNumber ?? orderHeaderFromDb.PhoneNumber;
            orderHeaderFromDb.StreetAddress = orderHeader.StreetAddress ?? orderHeaderFromDb.StreetAddress;
            orderHeaderFromDb.City = orderHeader.City ?? orderHeaderFromDb.City;
            orderHeaderFromDb.State = orderHeader.State ?? orderHeaderFromDb.State;
            orderHeaderFromDb.PostalCode = orderHeader.PostalCode ?? orderHeaderFromDb.PostalCode;

            if (!string.IsNullOrEmpty(orderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = orderHeader.Carrier;
            }
            if (!string.IsNullOrEmpty(orderHeader.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = orderHeader.TrackingNumber;
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task ShipOrder(OrderHeader orderHeader)
        {
            if (orderHeader == null)
                throw new BadRequestException(["Order Header data is required."]);

            var orderHeaderFromDb = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(orderHeader.Id);

            if (orderHeaderFromDb == null)
                throw new OrderHeaderNotFoundException(orderHeader.Id);

            orderHeaderFromDb.TrackingNumber = orderHeader.TrackingNumber;
            orderHeaderFromDb.Carrier = orderHeader.Carrier;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _unitOfWork.OrderHeaderRepository.Update(orderHeaderFromDb);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = await _unitOfWork.OrderHeaderRepository.GetByIdAsync(id);
            if (orderFromDb != null)
            {
                if (!string.IsNullOrEmpty(sessionId))
                {
                    orderFromDb.SessionId = sessionId;
                }
                if (!string.IsNullOrEmpty(paymentIntentId))
                {
                    orderFromDb.PaymentIntentId = paymentIntentId;
                    orderFromDb.PaymentDate = DateTime.Now;
                }
            }
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
