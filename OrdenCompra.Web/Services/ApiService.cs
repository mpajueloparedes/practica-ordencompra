using Newtonsoft.Json;
using OrdenCompra.Core.DTOs.Auth;
using OrdenCompra.Core.DTOs.Request;
using OrdenCompra.Core.DTOs.Response;
using System.Net.Http.Headers;
using System.Text;

namespace OrdenCompra.Web.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly IConfiguration _configuration;
    private string? _token;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        var apiBaseUrl = _configuration["ApiSettings:BaseUrl"] ?? "https://localhost:60914";
        _httpClient.BaseAddress = new Uri(apiBaseUrl);
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void EstablecerToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public void LimpiarToken()
    {
        _token = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<LoginResponse>(responseContent);
            }

            _logger.LogWarning("Error en login: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al realizar login");
            return null;
        }
    }

    public async Task<ListaOrdenesResponse?> ObtenerOrdenesAsync(FiltroOrdenesRequest filtros)
    {
        try
        {
            var queryString = BuildQueryString(filtros);
            var response = await _httpClient.GetAsync($"api/ordenes?{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ListaOrdenesResponse>(content);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener órdenes");
            return null;
        }
    }

    public async Task<OrdenResponse?> ObtenerOrdenPorIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/ordenes/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OrdenResponse>(content);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener orden por ID: {Id}", id);
            return null;
        }
    }

    public async Task<OrdenResponse?> CrearOrdenAsync(CrearOrdenRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/ordenes", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<OrdenResponse>(responseContent);
            }
            else
            {
                dynamic? obj = JsonConvert.DeserializeObject<dynamic>(responseContent);
                if (obj == null)
                {
                    throw new Exception("No se pudo interpretar la respuesta del servidor.");
                }
                string mensaje = obj.mensaje != null ? (string)obj.mensaje : "Error desconocido.";
                throw new Exception(mensaje);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear orden." + ex.Message);
            throw;
        }
    }

    public async Task<OrdenResponse?> ActualizarOrdenAsync(ActualizarOrdenRequest request)
    {
        try
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/ordenes/{request.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<OrdenResponse>(responseContent);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar orden");
            return null;
        }
    }

    public async Task<bool> EliminarOrdenAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/ordenes/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar orden: {Id}", id);
            return false;
        }
    }

    public async Task<IEnumerable<OrdenResponse>?> ObtenerMisOrdenesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/ordenes/mis-ordenes");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<OrdenResponse>>(content);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener mis órdenes");
            return null;
        }
    }

    private static string BuildQueryString(FiltroOrdenesRequest filtros)
    {
        var queryParams = new List<string>
        {
            $"Pagina={filtros.Pagina}",
            $"TamanoPagina={filtros.TamanoPagina}",
            $"OrdenarPor={filtros.OrdenarPor}",
            $"OrdenarDireccion={filtros.OrdenarDireccion}"
        };

        if (!string.IsNullOrEmpty(filtros.Cliente))
            queryParams.Add($"Cliente={Uri.EscapeDataString(filtros.Cliente)}");

        if (filtros.FechaInicio.HasValue)
            queryParams.Add($"FechaInicio={filtros.FechaInicio.Value:yyyy-MM-dd}");

        if (filtros.FechaFin.HasValue)
            queryParams.Add($"FechaFin={filtros.FechaFin.Value:yyyy-MM-dd}");

        return string.Join("&", queryParams);
    }
}
