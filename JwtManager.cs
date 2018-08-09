using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using PEW_API.Models;

namespace PEW_API
{
    public static class JwtManager
    {
        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static string GenerateToken(Session session, int expireMinutes = 480)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;

 
            var tokenDescriptor = new SecurityTokenDescriptor();
            
            if(session.IsFamilia)
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new[]
                        {
                            new Claim("user" , session.Usuario),
                            new Claim("pass", session.Password),
                            new Claim("cedula", session.FamilyMembers[0].Cedula),
                            new Claim("nombre", session.FamilyMembers[0].NombreCompleto),
                            new Claim("ano", session.Student.Ano.ToString()),
                            new Claim("idColegio", session.Student.IdColegio.ToString()),
                            new Claim("idioma", session.Student.Idioma.ToString()),
                            new Claim("colegio", session.Student.Colegio),
                            new Claim("idxMaestro", session.FamilyMembers[0].IdxEstudiante.ToString()),
                            new Claim("bloqueado", session.Student.Bloqueado.ToString()),
                            new Claim("tipoMaestro", "F"),
                            new Claim("periodo", session.Student.Periodo.ToString())
                        });
            }
            else
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new[]
                        {
                            new Claim("user" , session.Usuario),
                            new Claim("pass", session.Password),
                            new Claim("cedula", session.Student.Cedula),
                            new Claim("nombre", session.Student.Nombre),
                            new Claim("ano", session.Student.Ano.ToString()),
                            new Claim("idColegio", session.Student.IdColegio.ToString()),
                            new Claim("idioma", session.Student.Idioma.ToString()),
                            new Claim("colegio", session.Student.Colegio),
                            new Claim("idxMaestro", session.Student.IdxMaestro.ToString()),
                            new Claim("bloqueado", session.Student.Bloqueado.ToString()),
                            new Claim("tipoMaestro", session.Student.TipoMaestro),
                            new Claim("periodo", session.Student.Periodo.ToString())
                        });
            }

            tokenDescriptor.Expires = now.AddMinutes(Convert.ToInt32(expireMinutes));
            tokenDescriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature);
            
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);

                return principal;
            }

            catch (Exception)
            {
                return null;
            }
        }
    }
}