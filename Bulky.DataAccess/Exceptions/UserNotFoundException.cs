using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Exceptions
{
    public sealed class UserNotFoundException(string id) :NotFoundException ($"User with id {id} Not Found")
    {
    }
}
