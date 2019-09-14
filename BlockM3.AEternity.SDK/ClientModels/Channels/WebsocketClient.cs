using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.Extensions;
using BlockM3.AEternity.SDK.Utils;
using Nethereum.RLP;
using Newtonsoft.Json;
using StreamJsonRpc;
using Encoding = System.Text.Encoding;

namespace BlockM3.AEternity.SDK.ClientModels.Channels
{
    internal class WebsocketClient: IDisposable
    {
        internal Subject<ChannelMessage> _messages=new Subject<ChannelMessage>();
        internal Subject<ChannelEvent> _events=new Subject<ChannelEvent>();

        internal JsonRpc _rpc;

        internal CancellationTokenSource _src=new CancellationTokenSource();

        public IObservable<ChannelMessage> Messages => _messages.AsObservable();
        public IObservable<ChannelEvent> Events => _events.AsObservable();
        public Account Account { get; private set; }
        public string ChannelId { get; private set; }


        internal static async Task<WebsocketClient> CreateAsync(Account ac)
        {
            WebsocketClient ch=new WebsocketClient();
            ch.Account = ac;
            ch._rpc=new JsonRpc(await ac.Client.Configuration.GetWebSocketHandlerAsync(ch._src.Token));
            ch._rpc.AddLocalRpcTarget(ch, new JsonRpcTargetOptions {AllowNonPublicInvocation = true});
            ch._rpc.StartListening();
            return ch;
        }
        [JsonRpcMethod("channels.message")]
        private void RpcMessage(DataMessage data)
        {
            _messages.OnNext(data.Message);
        }

        [JsonRpcMethod("channels.sign.initiator_sign")]
        private Task SignInitiator(SignMessage data)
        {
            return SignAndReturn("channels.initiator_sign", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.responder_sign")]
        private Task SignResponder(SignMessage data)
        {
            return SignAndReturn("channels.responder_sign", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.update")]
        private Task SignUpdate(SignMessage data)
        {
            return SignAndReturn("channels.update", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.deposit_tx")]
        private Task SignDepositTx(SignMessage data)
        {
            return SignAndReturn("channels.deposit_tx", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.deposit_ack")]
        private Task SignDepositAck(SignMessage data)
        {
            return SignAndReturn("channels.deposit_ack", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.withdraw_tx")]
        private Task SignWithdrawTx(SignMessage data)
        {
            return SignAndReturn("channels.withdraw_tx", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.withdraw_ack")]
        private Task SignWithdrawAck(SignMessage data)
        {
            return SignAndReturn("channels.withdraw_ack", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.shutdown_sign")]
        private Task SignShutdownSign(SignMessage data)
        {
            return SignAndReturn("channels.shutdown_sign", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.shutdown_sign_ack")]
        private Task SignShutdownSignAck(SignMessage data)
        {
            return SignAndReturn("channels.shutdown_sign_ack", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.close_solo_sign")]
        private Task SignShutdownSoloSign(SignMessage data)
        {
            return SignAndReturn("channels.close_solo_sign", data.SignedTx);
        }
        [JsonRpcMethod("channels.sign.settle_sign")]
        private Task SignSettleSign(SignMessage data)
        {
            return SignAndReturn("channels.settle_sign", data.SignedTx);
        }
        private Task SignAndReturn(string obj, string tx)
        {
            string sign = CoSign(tx);
            return _rpc.InvokeAsync(obj, new SignMessage {SignedTx = sign});
        }

        private string CoSign(string tx)
        {
            byte[] signedraw = Utils.Encoding.DecodeCheckWithIdentifier(tx);
            RLPCollection collection = RLP.Decode(signedraw) as RLPCollection;
            RLPCollection sublist = collection[2] as RLPCollection;
            byte[] rawtx = collection[3].RLPData;
            byte[] signatures = sublist[0].RLPData;
            byte[] networkData = System.Text.Encoding.UTF8.GetBytes(Account.Client.Configuration.Network);
            byte[] txAndNetwork = networkData.Concatenate(rawtx);
            byte[] resign = Encoding.UTF8.GetBytes(Utils.Encoding.EncodeCheck(Signing.Sign(txAndNetwork, Account.KeyPair.PrivateKey), Constants.ApiIdentifiers.SIGNATURE));
            signatures = signatures.Concatenate(resign);
            return Utils.Encoding.EncodeSignedTransaction(rawtx, signatures);

        }

        public async Task<ChannelResult<List<AccountBalance>>> GetBalancesAsync(params string[] accounts)
        {

            return await WrapError(async ()=>
            {
                BalancesMessage msg = await _rpc.InvokeAsync<BalancesMessage>("channels.get.balances", accounts.ToList()).ConfigureAwait(false);
                List<AccountBalance> ls = new List<AccountBalance>(msg.AccountBalances);
                return new ChannelResult<List<AccountBalance>>(ls);
            });
            
        }

        private ChannelResult ProcessJsonRpcError(RemoteInvocationException e, ChannelResult t)
        {
            t.Error=new ChannelError();
            t.Error.GeneralCode = e.ErrorCode;
            t.Error.GeneralMessage = e.Message;
            DataError dt = JsonConvert.DeserializeObject<DataError>(e.ErrorData.ToString());
            t.Error.LocalCode = dt?.Code ?? 0;
            t.Error.LocalMessage = dt?.Message;
            return t;
        }
        private ChannelResult<T> ProcessJsonRpcError<T>(RemoteInvocationException e, ChannelResult <T> t)
        {
            ProcessJsonRpcError(e, (ChannelResult) t);
            return t;
        }
        private Task<ChannelResult> WrapError(Func<Task<ChannelResult>> f)
        {
            try
            {
                return f();
            }
            catch (RemoteInvocationException e)
            {
                return Task.FromResult(ProcessJsonRpcError(e, new ChannelResult())); 
            }
        }
        private Task<ChannelResult<T>> WrapError<T>(Func<Task<ChannelResult<T>>> f)
        {
            try
            {
                return f();
            }
            catch (RemoteInvocationException e)
            {
                return Task.FromResult(ProcessJsonRpcError<T>(e, new ChannelResult<T>())); 
            }
        }
        public Task SendMessageAsync(string message, string recipient)
        {
            return _rpc.InvokeAsync("channels.message", message, recipient);
        }
        public Task LeaveAsync()
        {
            return _rpc.InvokeAsync("channels.leave");
        }
        public Task ShutdownAsync()
        {
            return _rpc.InvokeAsync("channels.shutdown");
        }
        public Task SettleAsync()
        {
            return _rpc.InvokeAsync("channels.settle");
        }
        [JsonRpcMethod("channels.info")]
        private void RpcInfo(Channel.InfoPars<Channel.DataEvent> parms)
        {
            if (ChannelId == null && !string.IsNullOrEmpty(parms.ChannelId))
                ChannelId = parms.ChannelId;
            _events.OnNext(new ChannelEvent {ChannelId = parms.ChannelId, Event = parms.Data?.Event ?? ""});
        }
        [JsonRpcMethod("channels")]
        public void Dispose()
        {
            _src.Cancel();
            _src?.Dispose();
            _src = null;
            _rpc?.Dispose();
            _rpc = null;
        }

    }
    public class DataMessage
    {
        [JsonProperty("message")]
        public ChannelMessage Message { get; set; }
    }        
    
    public class ChannelEvent
    {
        public string ChannelId { get; set; }
        public string Event { get; set; }
    }
    public class ChannelMessage
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }
        [JsonProperty("from")]
        public string From { get; set; }
        [JsonProperty("to")]
        public string To { get; set; }
        [JsonProperty("info")]
        public string Info { get; set; }
    }
    
    public class SignMessage
    {
        [JsonProperty("signed_tx")]
        public string SignedTx { get; set; }
    }
    public class ChannelResult
    {
        public bool Success { get; set; }
        public ChannelError Error { get; set; }
    }

    public class ChannelResult<T> : ChannelResult
    {
        public T Result { get; set; }

        public static implicit operator T(ChannelResult<T> a) => a.Result;

        public ChannelResult(T value)
        {
            Result = value;
        }

        public ChannelResult()
        {

        }
    }

    public class ChannelError
    {
        public int GeneralCode { get; set; }

        public string GeneralMessage { get; set; }

        public int LocalCode { get; set; }

        public string LocalMessage { get; set; }
    }

    public class DataError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }
    public class BalancesMessage
    {
        [JsonProperty("accounts")]
        public List<AccountBalance> AccountBalances { get; set; }
    }

    public class AccountBalance
    {
        [JsonProperty("account")]
        public string Account { get; set; }
        [JsonProperty("balance")]
        public BigInteger Balance { get; set; }

    }
}
