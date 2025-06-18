using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Exceptions
{
    public sealed class OrderHeaderNotFoundException(int id) : NotFoundException($"Order With id {id} Not Found.")
    {
    }
}
