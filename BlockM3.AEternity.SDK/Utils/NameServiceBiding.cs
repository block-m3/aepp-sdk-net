using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using BlockM3.AEternity.SDK.Generated.Models;

namespace BlockM3.AEternity.SDK.Utils
{
    public class NameServiceBiding
    {
        private static ulong[] baseRanges = {5702887, 3524578, 2178309, 1346269, 832040, 514229, 317811, 196418, 121393, 75025, 46368, 28657, 17711, 10946, 6765, 4181, 2584, 1597, 987, 610, 377, 233, 144, 89, 55, 34, 21, 13, 8, 5, 3};

        public static Dictionary<int,double> NameFees { get; } = new Dictionary<int, double>();


        static NameServiceBiding()
        {
            int init = 1;
            foreach (ulong baseRange in baseRanges)
                NameFees.Add(init++, baseRange*Constants.BaseConstants.NAME_FEE_MULTIPLIER);
        }

        public static BigInteger GetDefaultBidFee(string domain)
        {
            int length = GetDomainLength(domain);
            return new BigInteger(NameFees[length >= Constants.BaseConstants.MAX_BID_FEE_LENGTH ? Constants.BaseConstants.MAX_BID_FEE_LENGTH : length]);
        }

        public static BigInteger GetBidFee(string domain, ulong start_fee = Constants.BaseConstants.NAME_FEE, double increment=Constants.BaseConstants.NAME_FEE_BID_INCREMENT)
        {
            if (increment<Constants.BaseConstants.NAME_FEE_BID_INCREMENT)
                throw new ArgumentException($"Minimum increment percentage is {Constants.BaseConstants.NAME_FEE_BID_INCREMENT}");
            int length = GetDomainLength(domain);
            double fee = start_fee;
            if (start_fee <= Constants.BaseConstants.NAME_FEE)
            {
                fee =  NameFees[length >= Constants.BaseConstants.NAME_BID_MAX_LENGTH ? Constants.BaseConstants.NAME_BID_MAX_LENGTH : length];
            }
            fee = Math.Ceiling(fee*(1 + increment));
            return new BigInteger(fee);
        }

        public static ulong GetAuctionEndBlock(string domain, ulong claim_height)
        {
            int length = GetDomainLength(domain);
            if (length <= 4)
                return (62 * Constants.BaseConstants.NAME_BID_TIMEOUT_BLOCKS)+claim_height;
            if (length <= 8)
                return (31 * Constants.BaseConstants.NAME_BID_TIMEOUT_BLOCKS)+claim_height;
            if (length <= Constants.BaseConstants.NAME_BID_MAX_LENGTH)
                return Constants.BaseConstants.NAME_BID_TIMEOUT_BLOCKS + claim_height;
            return claim_height;
        }

        public static int GetDomainLength(string domain)
        {
            int length = domain.Replace(".chain","").Length;
            if (length==0)
                throw new ArgumentException($"Domain can't be null");
            return length;
        }


    }
}
