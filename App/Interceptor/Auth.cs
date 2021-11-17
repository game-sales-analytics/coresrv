using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using Grpc.Core.Interceptors;
using Grpc.Core;


namespace App.Interceptors
{
    public class AuthInterceptor : Interceptor
    {
        private readonly ILogger<AuthInterceptor> logger;

        private readonly IAuthService authService;


        public AuthInterceptor(
            ILogger<AuthInterceptor> _logger,
            IAuthService _authService
        )
        {
            logger = _logger;
            authService = _authService;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
          TRequest request,
          ServerCallContext context,
          UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var authHeaders = context.RequestHeaders.GetAll("auth").ToList();
            if (authHeaders.Count != 1 || authHeaders.First().IsBinary)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "bad authentication header format receveied."));
            }

            var authToken = authHeaders.First();

            try
            {
                var isAuthenticated = await authService.IsUserAuthenticated(authToken.Value);
                if (!isAuthenticated)
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "authentication failed."));
                }

                return await continuation(request, context);
            }
            catch (ServiceFailureException ex)
            {
                logger.LogError(ex, "failed checking user authenticity using auth service", authHeaders);
                throw new RpcException(new Status(StatusCode.Internal, "internal error occurred. please try again later."));
            }
        }
    }
}