using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Commands
{
    public class VerifyOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }
}
