using System.Collections.Generic;

namespace BlockM3.AEternity.SDK.Integration.Tests
{
    public static class TestConstants
    {
        /** @see https://testnet.contracts.aepps.com/ */
        public const string TestnetAccountPrivateKey = "a7a695f999b1872acb13d5b63a830a8ee060ba688a478a08c6e65dfad8a01cd70bb4ed7927f97b51e1bcb5e1340d12335b2a2b12c8bc5221d63c4bcb39d41e61";

        public const string TestnetURL = "https://sdk-testnet.aepps.com/v2";

        public const string TestContractSourceCode = "contract Identity =\n  entrypoint main(x : int) = x";

        public const string TestContractFunction = "main";

        public const string TestContractFunctionSophiaType = "int";

        public const string TestContractFunctionParam = "42";

        public const string TestContractByteCode = "cb_+GZGA6Abk28ISckRxfWDHCo6FYfNzlAYCRW6TBDvQZ2BYQUmH8C4OZ7+RNZEHwA3ADcAGg6CPwEDP/64F37sADcBBwcBAQCWLwIRRNZEHxFpbml0EbgXfuwRbWFpboIvAIU0LjIuMADEu3fg";

        public const string TestContractCallData = "cb_KxFE1kQfP4oEp9E=";

        public const string BinaryTxDevnet = "f8b02a01a10175ee9825ad630963482bb1939a212a0e535883c6b4d7804e40287e1f556da27201b868f8664603a01b936f0849c911c5f5831c2a3a1587cdce50180915ba4c10ef419d816105261fc0b8399efe44d6441f00370037001a0e823f01033ffeb8177eec0037010707010100962f021144d6441f11696e697411b8177eec116d61696e822f0085342e322e3000830500038703e739b70768008207d000008203e8844190ab00872b1144d6441f3f";

        public const string EncodedServiceCall = "cb_KxG4F37sG1Q/+F7e";

        public const string EncodedServiceCallAnswer = "cb_AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACr8s/aY";

        public const string ServiceCallAnswerJSON = "{\"type\":\"word\",\"value\":42}";

        public const string BENEFICIARY_PRIVATE_KEY = "79816BBF860B95600DDFABF9D81FEE81BDB30BE823B17D80B9E48BE0A7015ADF";

        public const string DOMAIN = "aeternity";

        public const string NAMESPACE = ".chain";

        public const string PaymentSplitterACI = "{\"contract\":{\"event\":{\"variant\":[{\"AddingInitialRecipients\":[]},{\"RecipientAdded\":[\"address\",\"int\"]},{\"AddressUpdated\":[\"address\",\"address\"]},{\"UpdatingAllRecipients\":[]},{\"PaymentReceivedAndSplitted\":[\"address\",\"int\",\"int\"]}]},\"functions\":[{\"arguments\":[{\"name\":\"recipientConditions'\",\"type\":{\"map\":[\"address\",\"int\"]}}],\"name\":\"init\",\"payable\":false,\"returns\":\"PaymentSplitter.state\",\"stateful\":false},{\"arguments\":[],\"name\":\"getOwner\",\"payable\":false,\"returns\":\"address\",\"stateful\":false},{\"arguments\":[],\"name\":\"getRecipientsCount\",\"payable\":false,\"returns\":\"int\",\"stateful\":false},{\"arguments\":[{\"name\":\"who'\",\"type\":\"address\"}],\"name\":\"isRecipient\",\"payable\":false,\"returns\":\"bool\",\"stateful\":false},{\"arguments\":[{\"name\":\"who'\",\"type\":\"address\"}],\"name\":\"getWeight\",\"payable\":false,\"returns\":\"int\",\"stateful\":false},{\"arguments\":[],\"name\":\"getTotalAmountSplitted\",\"payable\":false,\"returns\":\"int\",\"stateful\":false},{\"arguments\":[],\"name\":\"payAndSplit\",\"payable\":true,\"returns\":{\"tuple\":[]},\"stateful\":true},{\"arguments\":[{\"name\":\"newOwner'\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"payable\":false,\"returns\":{\"tuple\":[]},\"stateful\":true},{\"arguments\":[{\"name\":\"oldAddress'\",\"type\":\"address\"},{\"name\":\"newAddress'\",\"type\":\"address\"}],\"name\":\"updateAddress\",\"payable\":false,\"returns\":{\"tuple\":[]},\"stateful\":true},{\"arguments\":[{\"name\":\"recipients'\",\"type\":{\"map\":[\"address\",\"int\"]}}],\"name\":\"updateRecipientConditions\",\"payable\":false,\"returns\":{\"tuple\":[]},\"stateful\":true}],\"name\":\"PaymentSplitter\",\"payable\":true,\"state\":{\"record\":[{\"name\":\"owner\",\"type\":\"address\"},{\"name\":\"recipientConditions\",\"type\":{\"map\":[\"address\",\"int\"]}},{\"name\":\"totalAmountSplitted\",\"type\":\"int\"}]},\"type_defs\":[]}}";

        public static List<string> TestContractFunctionParams = new List<string> {TestContractFunctionParam};

        public static int NUM_TRIALS_DEFAULT = 70;
    }
}