using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.Progress;
using BlockM3.AEternity.SDK.Utils;

namespace BlockM3.AEternity.SDK.ClientModels
{
    public class PreClaim : BaseFluent
    {
        internal PreClaim(string domain, Account account) : base(account)
        {
            Salt = Crypto.GenerateNamespaceSalt();
            Domain = domain;
        }

        public string Domain { get; }

        public BigInteger Salt { get; }


        [Obsolete]
        public async Task<InProgress<Claim>> ClaimDomainAsync(ulong bid_fee=Constants.BaseConstants.NAME_FEE, ulong fee=Constants.BaseConstants.FEE, ulong name_ttl = Constants.BaseConstants.NAME_TTL, CancellationToken token = default(CancellationToken))
        {
            Account.ValidatePrivateKey();
            Account.Nonce++;
            Claim cl = new Claim(Domain, Account);
            if (bid_fee == Constants.BaseConstants.NAME_FEE)
                bid_fee = (ulong) NameServiceBiding.GetDefaultBidFee(Domain);
            await cl.SignAndSendAsync(Account.Client.CreateNameClaimTransaction(Account.KeyPair.PublicKey, Domain, Salt, bid_fee, fee, name_ttl), token).ConfigureAwait(false);
            return new InProgress<Claim>(new WaitForHash(cl));
        }
    }
}