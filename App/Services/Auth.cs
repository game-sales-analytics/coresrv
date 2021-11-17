using System;
using Grpc.Net.Client;
using Grpc.Core;
using GSA.Grpc;
using System.Threading.Tasks;

namespace App
{
    public class AuthService : IAuthService
    {
        private readonly UsersService.UsersServiceClient client;

        public AuthService(string serviceAddress)
        {
            try
            {
                var channel = GrpcChannel.ForAddress(serviceAddress);
                client = new UsersService.UsersServiceClient(channel);
            }
            catch (System.Exception)
            {
                throw new ConnectionFailureException();
            }
        }

        async Task<bool> IAuthService.IsUserAuthenticated(string token)
        {
            try
            {
                var reply = await client.AuthenticateAsync(
                    new AuthenticateRequest
                    {
                        Token = token,
                    });
                return true;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unauthenticated)
            {
                return false;
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
            {
                throw new ServiceFailureException();
            }
        }
    }

    public class ConnectionFailureException : Exception { }

    public class ServiceFailureException : Exception { }
}