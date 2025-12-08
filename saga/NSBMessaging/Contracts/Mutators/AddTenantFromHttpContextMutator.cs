using Microsoft.AspNetCore.Http;
using NServiceBus.MessageMutator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Mutators
{
    public class AddTenantFromHttpContextMutator : IMutateOutgoingTransportMessages
    {
        private readonly IHttpContextAccessor _http;

        public AddTenantFromHttpContextMutator(IHttpContextAccessor http) => _http = http;

        public Task MutateOutgoing(MutateOutgoingTransportMessageContext context)
        {

            var tenantId = _http.HttpContext?.Request.Headers["TenantId"].ToString();

            context.OutgoingHeaders["TenantId"] = string.IsNullOrEmpty(tenantId) ? "Default" : tenantId;
            return Task.CompletedTask;
        }
    }
}
