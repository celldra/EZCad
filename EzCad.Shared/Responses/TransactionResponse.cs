using System.Text.Json.Serialization;

namespace EzCad.Shared.Responses;

public class TransactionResponse : BaseResponse
{
    [JsonPropertyName("balance")] public double Balance { get; set; }

    [JsonPropertyName("targetBalance")] public double TargetBalance { get; set; }

    [JsonPropertyName("transactionId")] public string TransactionId { get; set; }

    [JsonPropertyName("targetTransactionId")]
    public string TargetTransactionId { get; set; }
}