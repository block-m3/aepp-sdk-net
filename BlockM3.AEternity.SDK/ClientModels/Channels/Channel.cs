using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlockM3.AEternity.SDK.ClientModels.Channels;
using BlockM3.AEternity.SDK.Extensions;
using BlockM3.AEternity.SDK.Utils;

using Nethereum.RLP;
using Newtonsoft.Json;
using StreamJsonRpc;
using Encoding = System.Text.Encoding;

namespace BlockM3.AEternity.SDK.ClientModels
{
    public class Channel : IDisposable
    {

        internal class InfoPars<T>
        {
            [JsonProperty("channel_id")]
            public string ChannelId { get; set; }
            
            [JsonProperty("data")]
            public T Data { get; set; }

        }



        internal class STx
        {
            [JsonProperty("signed_tx")]
            public string SignedTx { get; set; }
        }
        internal class DataEvent
        {
            [JsonProperty("event")]
            public string Event { get; set; }
        }






        public bool IsInitiator => _initiator;
        public bool IsResponder => !_initiator;

        private bool _initiator;

        public string OtherPublicKey { get; private set; }



        public Account Account { get; private set; }



        public string ChannelId { get; private set; }



    

        
      
    }
}
