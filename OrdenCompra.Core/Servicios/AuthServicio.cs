using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OrdenCompra.Core.Constantes;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.Entidades;
using OrdenCompra.Core.Excepciones;
using OrdenCompra.Core.Interfaces;
using OrdenCompra.Core.Interfaces.Servicios;
using BCrypt.Net;

namespace OrdenCompra.Core.Servicios;

public class AuthServicio : IAuthServicio
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly int _jwtExpirationHours;

    public AuthServicio(IUnitOfWork unitOfWork, string jwtSecret = "twA7f!z9LxP2@cVn6RbTuE#0mJdYqKw4Z",
        string jwtIssuer = "OrdenCompraAPI", int jwtExpirationHours = 24)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _jwtSecret = jwtSecret;
        _jwtIssuer = jwtIssuer;
        _jwtExpirationHours = jwtExpirationHours;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            // Buscar usuario por nombre de usuario
            var usuario = await _unitOfWork.Usuarios.ObtenerPorNombreUsuarioAsync(request.NombreUsuario);

            if (usuario == null || !usuario.EstaActivo)
            {
                return new LoginResponse
                {
                    EsExitoso = false,
                    MensajeError = MensajesError.CredencialesInvalidas
                };
            }

            // Validar contraseña
            if (!await ValidarPasswordAsync(request.Password, usuario.PasswordHash))
            {
                return new LoginResponse
                {
                    EsExitoso = false,
                    MensajeError = MensajesError.CredencialesInvalidas
                };
            }

            // Generar token
            var token = GenerarTokenJwtAsync(usuario);
            var fechaExpiracion = DateTime.UtcNow.AddHours(_jwtExpirationHours);

            return new LoginResponse
            {
                EsExitoso = true,
                Token = token,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                Rol = usuario.Rol,
                FechaExpiracion = fechaExpiracion
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                EsExitoso = false,
                MensajeError = MensajesError.ErrorInterno,
                MensajeErrorDetalle = ex.Message
            };
        }
    }

    public string GenerarTokenJwtAsync(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.NombreUsuario),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Rol),
            new("jti", Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwtExpirationHours),
            Issuer = _jwtIssuer,
            Audience = _jwtIssuer,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ValidarPasswordAsync(string password, string hash)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return await Task.FromResult(BCrypt.Net.BCrypt.HashPassword(password));
    }

    public async Task<Usuario?> ObtenerUsuarioPorTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtIssuer,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            return await _unitOfWork.Usuarios.ObtenerPorIdAsync(userId);
        }
        catch
        {
            return null;
        }
    }

    public bool ValidarToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtIssuer,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TienePermisoAsync(int usuarioId, string accion)
    {
        var usuario = await _unitOfWork.Usuarios.ObtenerPorIdAsync(usuarioId);
        if (usuario == null || !usuario.EstaActivo) return false;

        // Los administradores tienen todos los permisos
        if (usuario.Rol == "Administrador") return true;

        // Lógica específica de permisos por acción
        return accion switch
        {
            "CrearOrden" => true, // Todos los usuarios pueden crear órdenes
            "EliminarOrden" => usuario.Rol == "Administrador",
            "ModificarOrden" => true, // Los usuarios pueden modificar sus propias órdenes
            "VerTodasLasOrdenes" => usuario.Rol == "Administrador",
            _ => false
        };
    }
}
