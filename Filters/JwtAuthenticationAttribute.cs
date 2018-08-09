using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using PEW_API.Models;

namespace PEW_API.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {
        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }



        private static bool ValidateToken(string token, out Claims claimsList)
        {
            claimsList = new Claims();

            var simplePrinciple = JwtManager.GetPrincipal(token);
            var identity = simplePrinciple?.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            claimsList.User = identity.FindFirst("user")?.Value;
            claimsList.Pass = identity.FindFirst("pass")?.Value;
            claimsList.Cedula = identity.FindFirst("cedula")?.Value;
            claimsList.Nombre = identity.FindFirst("nombre")?.Value;
            claimsList.Ano = Convert.ToInt32(identity.FindFirst("ano")?.Value);
            claimsList.IdColegio = Convert.ToInt32(identity.FindFirst("idColegio")?.Value);
            claimsList.Idioma = Convert.ToInt32(identity.FindFirst("idioma")?.Value);
            claimsList.Colegio = identity.FindFirst("colegio")?.Value;
            claimsList.IdxMaestro = Convert.ToInt32(identity.FindFirst("idxMaestro")?.Value);
            claimsList.Bloqueado = Convert.ToInt32(identity.FindFirst("bloqueando")?.Value);
            claimsList.TipoMaestro = identity.FindFirst("tipoMaestro")?.Value;
            claimsList.Periodo = Convert.ToInt32(identity.FindFirst("periodo")?.Value);

            if (string.IsNullOrEmpty(claimsList.User)
                || string.IsNullOrEmpty(claimsList.Pass)
                || string.IsNullOrEmpty(claimsList.Cedula)
                || string.IsNullOrEmpty(claimsList.Nombre)
                || string.IsNullOrEmpty(claimsList.Ano.ToString())
                || string.IsNullOrEmpty(claimsList.IdColegio.ToString())
                || string.IsNullOrEmpty(claimsList.Idioma.ToString())
                //|| string.IsNullOrEmpty(claimsList.Colegio.ToString())
                || string.IsNullOrEmpty(claimsList.IdxMaestro.ToString())
                || string.IsNullOrEmpty(claimsList.Bloqueado.ToString())
                || string.IsNullOrEmpty(claimsList.TipoMaestro)
                || string.IsNullOrEmpty(claimsList.Periodo.ToString()))
                return false;

            // More validation to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            if (ValidateToken(token, out Claims claimsList))
            {
                // based on username to get more information from database in order to build local identity
                var claims = new List<Claim>
                {
                    new Claim("user", claimsList.User),
                    new Claim("pass", claimsList.Pass),
                    new Claim("cedula", claimsList.Cedula),
                    new Claim("nombre", claimsList.Nombre),
                    new Claim("ano", claimsList.Ano.ToString()),
                    new Claim("idColegio", claimsList.IdColegio.ToString()),
                    new Claim("idioma", claimsList.Idioma.ToString()),
                    new Claim("colegio", claimsList.Colegio.ToString()),
                    new Claim("idxMaestro", claimsList.IdxMaestro.ToString()),
                    new Claim("bloqueado", claimsList.Bloqueado.ToString()),
                    new Claim("tipoMaestro", claimsList.TipoMaestro.ToString()),
                    new Claim("periodo", claimsList.Periodo.ToString()),
                    // Add more claims if needed: Roles, ...
                };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}