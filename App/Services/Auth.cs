using System;
using Grpc.Net.Client;
using Grpc.Core;
using GSA.Grpc;


namespace App
{
    public class AuthService
    {
        public AuthService(string serviceUrl, string token)
        {
            using var channel = GrpcChannel.ForAddress(serviceUrl);
            var client = new UsersService.UsersServiceClient(channel);
            try
            {
                var reply = client.Authenticate(
                    new AuthenticateRequest
                    {
                        Token = token,
                    });
                Console.WriteLine("User: " + reply.AuthenticatedUser);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Unauthenticated)
            {
                Console.WriteLine("bad token.");
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Internal)
            {
                Console.WriteLine("service failed");
            }
        }
    }
}