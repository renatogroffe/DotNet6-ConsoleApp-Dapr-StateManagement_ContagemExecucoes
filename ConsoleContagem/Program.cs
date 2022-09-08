using Dapr.Client;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

logger.Information("**** Testes com .NET + Dapr ****");

const string storeName = "redisstatestore";
const string keyContagem = "Contador";
const string keyUltimaExecucao = "UltimaExecucao";

using var daprClient = new DaprClientBuilder().Build();

var ultimaExecucao = await daprClient.GetStateAsync<string>(storeName, keyUltimaExecucao);
if (!String.IsNullOrWhiteSpace(ultimaExecucao))
    logger.Information($"Ultima execucao = {ultimaExecucao}");

var contador = await daprClient.GetStateAsync<int>(storeName, keyContagem);
logger.Information("Obtendo ultima contagem de execucoes...");

await daprClient.SaveStateAsync(storeName, keyUltimaExecucao, DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"));
logger.Information("Registrando horario da execucao...");

while (true)
{
    contador++;
    logger.Information($"Valor atual da contagem = {contador}");

    await daprClient.SaveStateAsync(storeName, keyContagem, contador);
    await Task.Delay(1000);
}