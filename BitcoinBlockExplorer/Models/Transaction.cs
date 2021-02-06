using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BitcoinBlockExplorer.Models
{
    public class Transaction
    {
        public string txid { get; set; }
        public string hash { get; set; }
        public int version { get; set; }
        public int size { get; set; }
        public int vsize { get; set; }
        public int weight { get; set; }
        public int locktime { get; set; }
        public List<Vin> vin { get; set; }
        public List<Vout> vout { get; set; }
    }
    public class Vin
    {
        public string txid { get; set; }
        public int vout { get; set; }
        public ScriptSig scriptSig { get; set; }
        public object sequence { get; set; }
        public bool coinbase { get; set; }
    }
    public class ScriptSig
    {
        public string asm { get; set; }
        public string hex { get; set; }
    }
    public class Vout
    {
        public double value { get; set; }
        public int n { get; set; }
        public ScriptPubKey scriptPubKey { get; set; }
    }
    public class ScriptPubKey
    {
        public string asm { get; set; }
        public string hex { get; set; }
        public int reqSigs { get; set; }
        public string type { get; set; }
        public List<string> addresses { get; set; }
    }

}
