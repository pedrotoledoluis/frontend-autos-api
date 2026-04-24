using System.Net;
using System.Text;
using AutosWeb.Models;
using AutosWeb.Services;
using AutosWeb.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace AutosWeb.Tests.Services;

/// <summary>
/// Tests críticos de <see cref="AutosApiClient"/>.
/// Cubren el contrato HTTP con el backend: comportamiento ante 404 y
/// deserialización ante 200 OK. Si el API rompe estos contratos, la UI se
/// rompe silenciosamente, por eso se eligieron como casos críticos.
/// </summary>
public class AutosApiClientTests
{
    private static AutosApiClient CreateClient(FakeHttpMessageHandler handler)
    {
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.test.local/")
        };
        return new AutosApiClient(http, NullLogger<AutosApiClient>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_devuelve_null_cuando_la_api_responde_404()
    {
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        var client = CreateClient(handler);

        var result = await client.GetByIdAsync(id: 999);

        result.Should().BeNull("el cliente debe traducir 404 a null sin lanzar excepción");
        handler.LastRequest!.Method.Should().Be(HttpMethod.Get);
        handler.LastRequest.RequestUri!.AbsolutePath.Should().Be("/api/autos/999");
    }

    [Fact]
    public async Task GetByIdAsync_deserializa_correctamente_cuando_la_api_responde_200()
    {
        const string json = """
            {
              "id": 7,
              "marca": "Toyota",
              "modelo": "Corolla",
              "anio": 2023,
              "tipoAuto": "Sedan",
              "cantidadAsientos": 5,
              "color": "Blanco",
              "precio": 25000.50,
              "activo": true,
              "fechaCreacion": "2026-01-15T10:30:00Z"
            }
            """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
        var client = CreateClient(handler);

        var result = await client.GetByIdAsync(id: 7);

        result.Should().NotBeNull();
        result!.Id.Should().Be(7);
        result.Marca.Should().Be("Toyota");
        result.Modelo.Should().Be("Corolla");
        result.Anio.Should().Be(2023);
        result.TipoAuto.Should().Be("Sedan");
        result.CantidadAsientos.Should().Be(5);
        result.Color.Should().Be("Blanco");
        result.Precio.Should().Be(25000.50m);
        result.Activo.Should().BeTrue();
    }
}
