using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Exceptions
{
    public sealed class CompanyNotFoundException(int id) : NotFoundException($"Company with id = {id} is not found")
    {
    }
}
