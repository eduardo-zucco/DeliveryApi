using System.Text.Json.Serialization;
using DeliveryApi.Dominio.Entidades;
using DeliveryApi.Middleware;
using DeliveryApi.Repositorios;
using DeliveryApi.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializa enums (como TipoVeiculo e StatusPedido) como strings amigáveis no JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositórios em memória (Singleton: mantém o estado consistente durante a execução da aplicação)
builder.Services.AddSingleton<IRepositorio<ItemCardapio>, RepositorioItemCardapioMemoria>();
builder.Services.AddSingleton<IClienteRepositorio, RepositorioClienteMemoria>();
builder.Services.AddSingleton<IRepositorio<Entregador>, RepositorioEntregadorMemoria>();
builder.Services.AddSingleton<IRepositorio<Pedido>, RepositorioPedidoMemoria>();

// Serviços de Aplicação
builder.Services.AddSingleton<CardapioServico>();
builder.Services.AddSingleton<ClienteServico>();
builder.Services.AddSingleton<EntregadorServico>();
builder.Services.AddSingleton<PedidoServico>();
builder.Services.AddSingleton<RelatorioServico>();

var app = builder.Build();

// Middleware global de tratamento de exceções (captura 404, 400, 409 e 500 sem derrubar a API)
app.UseMiddleware<TratamentoErrosMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
