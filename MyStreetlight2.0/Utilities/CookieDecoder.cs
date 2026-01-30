using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;

namespace Streetlight2._0.Utilities
{
    public class CookieDecoder
    {
        private readonly IDataProtector _protector;

        public CookieDecoder(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", CookieAuthenticationDefaults.AuthenticationScheme, "v2");
        }

        public ClaimsPrincipal? DecodeCookie(string cookieValue)
        {
            try
            {
                var ticketDataFormat = new TicketDataFormat(_protector);
                var ticket = ticketDataFormat.Unprotect(cookieValue);
                return ticket?.Principal;
            }
            catch
            {
                return null; // invalid or tampered cookie
            }
        }
    }
}
