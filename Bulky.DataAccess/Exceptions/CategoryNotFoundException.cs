using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Exceptions
{
    public sealed class CategoryNotFoundException(int id) : NotFoundException($"Category with id = {id} is not found")
    {
    }
}
