using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.Generated.Models;
using BlockM3.AEternity.SDK.Sophia;
using Newtonsoft.Json.Linq;

namespace BlockM3.AEternity.SDK.ClientModels
{
    public class ContractReturn
    {
        internal ContractReturn(ContractCallObject obj, string txinfo, Contract c)
        {
            CallerId = obj?.CallerId;
            Height = (long?) obj?.Height ?? -1;
            ContractId = obj?.ContractId;
            GasPrice = obj?.GasPrice ?? 0;
            GasUsed = obj?.GasUsed ?? 0;
            RawLog = obj?.Log.ToList() ?? new List<Event>();
            Events = SophiaMapper.GetEvents(c, RawLog);
            TXInfo = txinfo;
        }

        public string CallerId { get; }

        public long Height { get; }

        public string ContractId { get; }

        public BigInteger GasPrice { get; }

        public ulong GasUsed { get; }

        public List<Event> RawLog { get; }

        public List<Models.Event> Events { get; }

        public string TXInfo { get; }
    }

    public class ContractReturn<T> : ContractReturn
    {
        private ContractReturn(ContractCallObject obj, string txinfo, T value, Contract c) : base(obj, txinfo, c)
        {
            ReturnValue = value;
        }

        public T ReturnValue { get; }

        internal static async Task<ContractReturn<T>> CreateAsync(Account account, Contract c, string function, ContractCallObject obj, string txinfo, CancellationToken token)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ReturnType))
            {
                Function f = c.Functions.First(a => a.Name == function);
                JObject ret = null;
                if (obj.ReturnValue != "cb_Xfbg4g==")
                    ret = (await account.Client.DecodeDataAsync(obj.ReturnValue, f.OutputType.MapName, token).ConfigureAwait(false)).Data as JObject;
                if (ret == null)
                    return new ContractReturn<T>(obj, txinfo, default(T), c);
                return new ContractReturn<T>(obj, txinfo, f.OutputType.Deserialize<T>(ret.Value<string>("value")), c);
            }

            return new ContractReturn<T>(obj, txinfo, default(T), c);
        }
    }
}