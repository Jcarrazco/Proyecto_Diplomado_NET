using System;
using System.Linq;
using ActivosApp.Models;
using Microsoft.Maui.Storage;

namespace ActivosApp.Services;

public class SessionService
{
    private const string TokenKey = "session_token";
    private const string UsuarioNombreKey = "session_usuario";

    public string? Token { get; private set; }
    public string? UsuarioNombre { get; private set; }
    public UserContextDto? UserContext { get; private set; }

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(Token);
    public bool IsTecnico => UserContext?.Roles.Any(r => r.Equals("Tecnico", StringComparison.OrdinalIgnoreCase)) == true;
    public bool IsAdministrador =>
        UserContext?.Roles.Any(r =>
            r.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
            r.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            r.Equals("Sistemas", StringComparison.OrdinalIgnoreCase)) == true ||
        UsuarioNombre?.Equals("admin@indigo.com", StringComparison.OrdinalIgnoreCase) == true;

    public SessionService()
    {
        var token = Preferences.Get(TokenKey, string.Empty);
        var usuario = Preferences.Get(UsuarioNombreKey, string.Empty);

        Token = string.IsNullOrWhiteSpace(token) ? null : token;
        UsuarioNombre = string.IsNullOrWhiteSpace(usuario) ? null : usuario;
    }

    public void SetSession(string token, string usuarioNombre)
    {
        Token = token;
        UsuarioNombre = usuarioNombre;
        Preferences.Set(TokenKey, token);
        Preferences.Set(UsuarioNombreKey, usuarioNombre);
    }

    public void SetUserContext(UserContextDto context)
    {
        UserContext = context;
    }

    public void Logout()
    {
        Token = null;
        UsuarioNombre = null;
        UserContext = null;
        Preferences.Remove(TokenKey);
        Preferences.Remove(UsuarioNombreKey);
    }
}
