using BitcoinRpc;
using BitcoinRpc.CoreRPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBlockExplorer.Models
{
    public class Block
    {
        public string hash { get; set; }
        public int confirmations { get; set; }
        public DateTime time { get; set; }
        public string miner { get; set; }
        public int height { get; set; }
        public int nTx { get; set; }
        public double difficulty { get; set; }
        public string merkleroot { get; set; }
        public long version { get; set; }
        public string versionHex { get; set; }
        public long bits { get; set; }
        public long weight { get; set; }
        public long size { get; set; }
        public long nonce { get; set; }
        public List<Transaction> transactions { get; set; }
        public decimal transactionVolume { get; set; }
        public decimal blockReward { get; set ;}
        public decimal feeReward { get; set; }
        public string previousblockhash { get; set; }
        public string nextblockhash { get; set; }
        
        public Block(JObject json, List<Transaction> t)
        {
            hash = json["result"]["hash"].ToString();
            confirmations= Int32.Parse(json["result"]["confirmations"].ToString());
            time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(json["result"]["time"].ToString())).UtcDateTime.ToLocalTime();
            miner = json["result"]["tx"][0]["vout"][0]["scriptPubKey"]["addresses"][0].ToString();
            height = Int32.Parse(json["result"]["height"].ToString());
            nTx= Int32.Parse(json["result"]["nTx"].ToString());
            difficulty= double.Parse(json["result"]["difficulty"].ToString());
            merkleroot= json["result"]["merkleroot"].ToString();
            version= long.Parse(json["result"]["version"].ToString());
            versionHex = "0x"+json["result"]["versionHex"].ToString();
            bits= Convert.ToInt64(json["result"]["bits"].ToString(), 16);
            weight= long.Parse(json["result"]["weight"].ToString());
            size= long.Parse(json["result"]["size"].ToString());
            nonce = long.Parse(json["result"]["nonce"].ToString());
            transactions = t; //coinbase???????????????

        }
        public Block(JObject json)
        {
            hash = json["result"]["hash"].ToString();
            confirmations = Int32.Parse(json["result"]["confirmations"].ToString());
            time = DateTimeOffset.FromUnixTimeSeconds(long.Parse(json["result"]["time"].ToString())).UtcDateTime.ToLocalTime();
            miner = json["result"]["tx"][0]["vout"][0]["scriptPubKey"]["addresses"][0].ToString();
            height = Int32.Parse(json["result"]["height"].ToString());
            nTx = Int32.Parse(json["result"]["nTx"].ToString());
            difficulty = double.Parse(json["result"]["difficulty"].ToString());
            merkleroot = json["result"]["merkleroot"].ToString();
            version = long.Parse(json["result"]["version"].ToString());
            versionHex = "0x" + json["result"]["versionHex"].ToString();
            bits = Convert.ToInt64(json["result"]["bits"].ToString(), 16);
            weight = long.Parse(json["result"]["weight"].ToString());
            size = long.Parse(json["result"]["size"].ToString());
            nonce = long.Parse(json["result"]["nonce"].ToString());
            

        }
    }
}
