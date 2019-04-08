using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace LifeCouple.WebApi.Common
{
    public class JwtPayloadInfo
    {
        public string EmailAddress { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string ExtReferenceId { get; private set; }

        public static JwtPayloadInfo Extract(IEnumerable<Claim> claims)
        {
            //eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJleHAiOjE1MzU0Mzk2MjMsIm5iZiI6MTUzNTQzNjAyMywidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9sb2dpbi5taWNyb3NvZnRvbmxpbmUuY29tLzZlY2VlZDllLTMyYTktNDk5My1hYjFmLWJhODM5ODlkZjc0YS92Mi4wLyIsInN1YiI6Ijk0N2JjODc0LTdmZjctNDQ5ZC04ZDA2LTUwZjEyNTUyNjQ2ZCIsImF1ZCI6IjljNTkxZjc5LTQ0OTUtNDQ4OS05ZTllLTAxMmZlOWJlMTUyZiIsIm5vbmNlIjoiZGVmYXVsdE5vbmNlIiwiaWF0IjoxNTM1NDM2MDIzLCJhdXRoX3RpbWUiOjE1MzU0MzYwMjMsIm9pZCI6Ijk0N2JjODc0LTdmZjctNDQ5ZC04ZDA2LTUwZjEyNTUyNjQ2ZCIsImZhbWlseV9uYW1lIjoiR3JpbXNrb2ctTG4iLCJnaXZlbl9uYW1lIjoiUGVyLUZuIiwiZW1haWxzIjpbInBncmltc2tvZ0BsaWZlY291cGxlLm5ldCJdLCJ0ZnAiOiJCMkNfMV9zdXNpIn0.dYmbds0tv1HJvDsAzGICX0j3wA4nCJbp6y62McNy9fatDitlyTHCFs1CCXhy-RR4neUJQ2knhnLJuwVbPOzOb5FFJXgIDI1uraHhyUkxak_HIVXtrliGjBAFwGKALWCnmJmgz_RMFEVkuHH77KrW2dzpoZX975epTIlPCZgeFn5ftMMWkooC1qmOtu5ZXX5d_t0w3Lh0A53W37xGRDg3xYHl4L9lNQn2V8p6n8CyHsT-89BY8Tu3arnGwoD4ypWqi-OFSIze3H7bJxHzTKsagMc6boaiVWf5I_BQfNDccOrhbZdlb8tlY_tW8T7QERKZIaF5CFDhETlFJUfvvbny8w
            /*
             {
              "exp": 1535439623,
              "nbf": 1535436023,
              "ver": "1.0",
              "iss": "https://login.microsoftonline.com/6eceed9e-32a9-4993-ab1f-ba83989df74a/v2.0/",
              "sub": "947bc874-7ff7-449d-8d06-50f12552646d",
              "aud": "9c591f79-4495-4489-9e9e-012fe9be152f",
              "nonce": "defaultNonce",
              "iat": 1535436023,
              "auth_time": 1535436023,
              "oid": "947bc874-7ff7-449d-8d06-50f12552646d",
              "family_name": "Grimskog-Ln",
              "given_name": "Per-Fn",
              "emails": [
                "pgrimskog@lifecouple.net"
              ],
              "tfp": "B2C_1_susi"
            }
        */
            var r = new JwtPayloadInfo
            {
                EmailAddress = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,

                //Typically return something like '84b2647f-e9d4-4e95-99d4-2e28992588ff'
                //this is most likely going to be the same to get the Azure B2C ObjectctId, which we refer to as 'external Id'. See more about this isn section 2 at https://grimskog.wordpress.com/2018/02/17/token-based-authentication-jwt-azure-active-directory-b2c/
                ExtReferenceId = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value,

                FirstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value, //optional value
                LastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value, //optional value
            };

            if (string.IsNullOrWhiteSpace(r.EmailAddress))
            {
                r.EmailAddress = claims.FirstOrDefault(c => c.Type == "emails")?.Value; //that is what is being passed in the AD B2C token
            }

            return r;

        }
    }
}
