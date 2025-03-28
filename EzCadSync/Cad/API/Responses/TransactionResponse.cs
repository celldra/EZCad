using Newtonsoft.Json;

namespace EzCadSync.Api.Responses;

public class TransactionResponse : BaseResponse
{
    [JsonProperty("balance")] public double Balance { get; set; }

    [JsonProperty("targetBalance")] public double TargetBalance { get; set; }

    [JsonProperty("transactionId")] public string TransactionId { get; set; }

    [JsonProperty("targetTransactionId")] public string TargetTransactionId { get; set; }
}