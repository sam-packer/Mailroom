using System.Data.Common;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;

namespace Mailroom;

public class DynamicLeaderRecoveryInterceptor : DbConnectionInterceptor
{
    private readonly ILogger<DynamicLeaderRecoveryInterceptor> _logger;

    public DynamicLeaderRecoveryInterceptor(ILogger<DynamicLeaderRecoveryInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task ConnectionFailedAsync(
        DbConnection connection,
        ConnectionErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        var ex = eventData.Exception;

        if (ex is PostgresException pgEx && pgEx.SqlState == "57P01")
        {
            _logger.LogWarning("Detected PostgreSQL shutdown (57P01). Clearing all Npgsql connection pools.");
            NpgsqlConnection.ClearAllPools();
        }

        if (ex is NpgsqlException npgsqlEx && npgsqlEx.InnerException is SocketException sockEx && sockEx.SocketErrorCode == SocketError.ConnectionRefused)
        {
            _logger.LogWarning("Detected 'Connection Refused' (SocketError 111). Clearing all Npgsql connection pools.");
            NpgsqlConnection.ClearAllPools();
        }

        await base.ConnectionFailedAsync(connection, eventData, cancellationToken);
    }
}