using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.ClientModels;
using BlockM3.AEternity.SDK.Generated.Models;

namespace BlockM3.AEternity.SDK.Progress
{
    public class GetNameClaim : ITransactionProgress
    {
        public string TXHash => null;

        public async Task<(object result, bool done)> CheckForFinishAsync(object input, CancellationToken token = default(CancellationToken))
        {
            Claim cl = (Claim) input;
            try
            {
                NameEntry entry = await cl.Account.Client.GetNameIdAsync(cl.Domain, token).ConfigureAwait(false);
                cl.Pointers = entry.Pointers.ToList();
                cl.NameTtl = entry.Ttl;
                cl.Id = entry.Id;
                return (cl, true);
            }
            catch (ApiException e)
            {
                if (e.StatusCode == 404)
                    return (null, true);
                throw;
            }

        }
    }
}