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

    public async Task ValidateUser(string? username, string? password)
    {
        var ldapServer = configuration["LdapSettings:LdapServer"];
        var ldapPort = configuration.GetValue<int>("LdapSettings:LdapPort");
        var userDn = $"CN={username},CN=Users,DC=KADADANA,DC=LOCAL";
        var userContainer = configuration["LdapSettings:BaseDn"];
        var baseDn = configuration["LdapSettings:BaseDn"];


        var userDn2 = $"CN={username},CN={userContainer},{baseDn}";
        using var connection = new LdapConnection();


        try
        {
            await connection.ConnectAsync(ldapServer, ldapPort);
            await connection.BindAsync(userDn, password);
        }
        catch (LdapException ex)
        {
            System.Console.WriteLine("Ldap sunucusuna baglanirken bir hata olustu.\n" + ex.Message);
            throw;
        }


        if (connection.Bound)
        {
            UserModel.User.Username = username;
            UserModel.User.Password = password;
            UserModel.User.IsLoggedIn = true;
        }

    }
}
