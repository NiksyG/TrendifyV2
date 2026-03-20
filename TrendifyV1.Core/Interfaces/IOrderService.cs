using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendifyV1.Core.Interfaces
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(Guid userId);
    }
}
