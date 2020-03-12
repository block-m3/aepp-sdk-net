using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.Generated.Models;
using BlockM3.AEternity.SDK.Progress;
using BlockM3.AEternity.SDK.Transactions.NameService;
using BlockM3.AEternity.SDK.Utils;

namespace BlockM3.AEternity.SDK.ClientModels
{
    public class Claim : BaseFluent
    {
        internal Claim(string domain, Account account) : base(account)
        {
            Domain = domain;
        }

        public string Domain { get; }
        public ulong NameTtl { get; internal set; }
        public bool IsAuction => NameServiceBiding.GetDomainLength(Domain ?? "") <= 12;


        private NamePointer PointerForKey(string key)
        {
            return Pointers?.FirstOrDefault(a => a.Key == key);
        }

        private string IdForKey(string key) => PointerForKey(key)?.Id;

        private void SetForKey(string key, string id)
        {
            switch (key)
            {
                case Constants.PointersNames.ACCOUNT_PUBKEY:
                    if (!id.StartsWith(Constants.ApiIdentifiers.ACCOUNT_PUBKEY))
                        throw new ArgumentException("Invalid Account Public Key");
                    break;
                case Constants.PointersNames.CONTRACT_PUBKEY:
                    if (!id.StartsWith(Constants.ApiIdentifiers.CONTRACT_PUBKEY))
                        throw new ArgumentException("Invalid Contract Public Key");
                    break;
                case Constants.PointersNames.ORACLE_PUBKEY:
                    if (!id.StartsWith(Constants.ApiIdentifiers.ORACLE_PUBKEY))
                        throw new ArgumentException("Invalid Oracle Public Key");
                    break;
                case Constants.PointersNames.CHANNEL:
                    if (!id.StartsWith(Constants.ApiIdentifiers.ORACLE_PUBKEY))
                        throw new ArgumentException("Invalid Channel");
                    break;
                default:
                    throw new ArgumentException("Invalid Id");
            }
            NamePointer p = PointerForKey(key);
            if (p == null)
            {
                p = new NamePointer { Key = key };
                Pointers.Add(p);
            }
            if (p.Id!=id)
                p.Id = id;
        }
        public string OraclePointer
        {
            get => IdForKey(Constants.PointersNames.ORACLE_PUBKEY);
            set => SetForKey(Constants.PointersNames.ORACLE_PUBKEY,value);
        } 
        public string AccountPointer
        {
            get => IdForKey(Constants.PointersNames.ACCOUNT_PUBKEY);
            set => SetForKey(Constants.PointersNames.ACCOUNT_PUBKEY,value);
        } 
        public string ContractPointer
        {
            get => IdForKey(Constants.PointersNames.CONTRACT_PUBKEY);
            set => SetForKey(Constants.PointersNames.CONTRACT_PUBKEY,value);
        } 
        public string ChannelPointer
        {
            get => IdForKey(Constants.PointersNames.CHANNEL);
            set => SetForKey(Constants.PointersNames.CHANNEL,value);
        } 
        internal List<NamePointer> Pointers { get; set; } = new List<NamePointer>();
        public string Id { get; internal set; }

        public async Task<InProgress<Claim>> BidDomainAsync(BigInteger bid_fee, ulong fee=Constants.BaseConstants.NAME_FEE, ulong name_ttl = Constants.BaseConstants.NAME_TTL, CancellationToken token = default(CancellationToken))
        {
            Account.ValidatePrivateKey();
            Account.Nonce++;
            await SignAndSendAsync(Account.Client.CreateNameClaimTransaction(Account.KeyPair.PublicKey, Domain, 0, bid_fee, fee, name_ttl), token).ConfigureAwait(false);
            return new InProgress<Claim>(new WaitForHash(this));
        }

        public BigInteger GetBidFee(ulong start_fee = Constants.BaseConstants.NAME_FEE, double increment = Constants.BaseConstants.NAME_FEE_BID_INCREMENT)
        {
            return NameServiceBiding.GetBidFee(Domain, start_fee, increment);
        }

        public ulong GetAuctionEndBlock(ulong claim_height)
        {
            return NameServiceBiding.GetAuctionEndBlock(Domain, claim_height);
        }
        public ulong GetAuctionEndBlock()
        {
            return GetAuctionEndBlock((ulong)Tx.BlockHeight);
        }
        public async Task<InProgress<Claim>> UpdateAsync(ulong name_ttl = Constants.BaseConstants.NAME_TTL, ulong client_ttl = Constants.BaseConstants.NAME_CLIENT_TTL, CancellationToken token = default(CancellationToken))
        {
            Account.ValidatePrivateKey();
            Account.Nonce++;
            await SignAndSendAsync(Account.Client.CreateNameUpdateTransaction(Account.KeyPair.PublicKey, Id, Account.Nonce, Account.Ttl, client_ttl, name_ttl, Pointers), token).ConfigureAwait(false);
            return new InProgress<Claim>(new WaitForHash(this));
        }

        public async Task<InProgress<bool>> RevokeAsync(CancellationToken token = default(CancellationToken))
        {
            Account.ValidatePrivateKey();
            Account.Nonce++;
            await SignAndSendAsync(Account.Client.CreateNameRevokeTransaction(Account.KeyPair.PublicKey, Id, Account.Nonce, Account.Ttl), token).ConfigureAwait(false);
            return new InProgress<bool>(new WaitForHash(this));
        }

        public async Task<InProgress<bool>> TransferAsync(string recipientPublicKey, CancellationToken token = default(CancellationToken))
        {
            Account.ValidatePrivateKey();
            Account.Nonce++;
            await SignAndSendAsync(Account.Client.CreateNameTransferTransaction(Account.KeyPair.PublicKey, Id, recipientPublicKey, Account.Nonce, Account.Ttl), token).ConfigureAwait(false);
            return new InProgress<bool>(new WaitForHash(this));
        }
    }
}