using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wonton.Common;
using Wonton.CrossUI.Web.RPC;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Wonton.CrossUI.Web.Rpc
{
    public class FPGARpcService : FPGARpc.FPGARpcBase
    {
        private readonly ILogger<FPGARpcService> _logger;
        private readonly FPGAManager _manager;

        public FPGARpcService(ILogger<FPGARpcService> logger, FPGAManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        public override Task<FPGARpcResponse> WriteReadFPGA(FPGARpcRequest request, ServerCallContext context)
        {
            return base.WriteReadFPGA(request, context);
        }
    }
}
