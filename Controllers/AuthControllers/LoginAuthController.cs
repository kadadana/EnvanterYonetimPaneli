using EnvanterYonetimPaneli.Models;
using Microsoft.AspNetCore.Mvc;
using Novell.Directory.Ldap;

namespace EnvanterYonetimPaneli.Controllers;

public class AuthController : Controller
{

    private readonly IConfiguration configuration;
    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<bool> ValidateUser(string? username, string? password)
    {
        var ldapServer = configuration["LdapSettings:LdapServer"];
        var ldapPort = configuration.GetValue<int>("LdapSettings:LdapPort");
        var baseDn = configuration["LdapSettings:BaseDn"];

        var userDn = $"CN={username},CN=Users,DC=KADADANA,DC=LOCAL";

        using var connection = new LdapConnection();
        try
        {
            await connection.ConnectAsync(ldapServer, ldapPort);
            await connection.BindAsync(userDn, password);
            if (!connection.Bound)
                return false;

            var filter = $"(sAMAccountName={username})";
            var results = await connection.SearchAsync(
            baseDn,
            LdapConnection.ScopeSub,
            filter,
            new[] { "memberOf" },
            false
        );

            await foreach (LdapEntry entry in results)
            {
                var attrSet = entry.GetAttributeSet();
                if (attrSet == null)
                    continue;

                if (!attrSet.ContainsKey("memberOf"))
                {
                    return false;
                }


                var memberOf = attrSet["memberOf"];

                foreach (var group in memberOf.StringValueArray)
                {
                    if (group.Contains("CN=Admins,"))
                        return true;
                }
            }

            return false;
        }
        catch (LdapException ex)
        {
            Console.WriteLine("Ldap sunucusuna baglanirken bir hata olustu.\n"
             + ex.Message);
            return false;
        }


    }
}
