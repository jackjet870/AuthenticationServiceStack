using AuthServiceStack.AuthModel.EF;
using System;

namespace AuthServiceStack.AuthProvider.EF
{
    public class SimpleFreshTokenProvider : Microsoft.Owin.Security.Infrastructure.IAuthenticationTokenProvider
    {
        private AuthRepository repo;
        //public SimpleFreshTokenProvider(string connection)
        //{
        //    this.config = connection;
        //}
        public SimpleFreshTokenProvider(AuthRepository repo)
        {
            this.repo = repo;
        }
        public void Create(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task CreateAsync(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenCreateContext context)
        {
            var clientId = context.Ticket.Properties.Dictionary["as:client_id"];
            if (string.IsNullOrEmpty(clientId)) return;
            var refreshTokenId = Guid.NewGuid().ToString("n");

            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");
            var token = new AuthModel.EF.Models.RefreshToken
            {
                Id = core.Helper.GetHash(refreshTokenId),
                ClientId = clientId,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime)),
                IssuedUtc = DateTime.UtcNow,
                Subject = context.Ticket.Identity.Name
            };
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;
            token.ProtectedTicket = context.SerializeTicket();
            var result = await repo.AddRefreshToken(token);
            if (result)
            {
                context.SetToken(refreshTokenId);
            }

        }

        public void Receive(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async System.Threading.Tasks.Task ReceiveAsync(Microsoft.Owin.Security.Infrastructure.AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            string hashedTokenId = core.Helper.GetHash(context.Token);


            var refreshToken = await repo.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Get protectedTicket from refreshToken class
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                var result = await repo.RemoveRefreshToken(hashedTokenId);
            }

        }
    }
}