using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Exceptions
{
    public sealed class CartNotFoundException(string id) : NotFoundException($"Cart with id = {id} is not found")
    {
    }
}
