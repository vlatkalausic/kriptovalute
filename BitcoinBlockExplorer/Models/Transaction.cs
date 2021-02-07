using BitcoinRpc;
using BitcoinRpc.CoreRPC;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
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
        public decimal ukupnoIzlaz { get; set; }
        public decimal ukupnoUlaz { get; set; }
        
        public decimal fee { get; set; }



        public Transaction(JObject json, Block block)
        {

            txid = json["result"]["txid"].ToString();
            hash = json["result"]["hash"].ToString();
            int br = json["result"]["vout"].Count();
            foreach (int i in Enumerable.Range(0, br))
            {

            }
            //block null znaci nije dodana
        }
        public Transaction(JObject json, List<Vin> _vin)//summary
        {

            txid = json["result"]["txid"].ToString();
            hash = json["result"]["hash"].ToString();

            int br = json["result"]["vout"].Count();
            decimal ukupnoIzlaztr = 0;
            vout = new List<Vout>();

            foreach (int i in Enumerable.Range(0, br))
            {
                var s = json["result"]["vout"][i]["value"].ToString();
                decimal value = Decimal.Round(decimal.Parse(json["result"]["vout"][i]["value"].ToString(), System.Globalization.NumberStyles.Any), 8, MidpointRounding.AwayFromZero);



                if (value > 0)
                {
                    ukupnoIzlaztr += value;

                    int bradresa = json["result"]["vout"][i]["scriptPubKey"]["addresses"].Count();
                    List<string> a = new List<string>();
                    foreach (int j in Enumerable.Range(0, bradresa))
                    {
                        string address = json["result"]["vout"][i]["scriptPubKey"]["addresses"][j].ToString();
                        a.Add(address);
                    }

                    Vout v = new Vout();
                    v.value = value;
                    ScriptPubKey spk = new ScriptPubKey();
                    spk.addresses = new List<string>();
                    foreach (string add in a)
                    {
                        spk.addresses.Add(add);

                    }
                    v.scriptPubKey = spk;

                    vout.Add(v);
                }

            }
            ukupnoIzlaz = ukupnoIzlaztr;
            
            
            decimal ukupnoUlaztr = 0;

            vin = _vin;
            int br2 = _vin.Count();

            foreach (int i in Enumerable.Range(0, br2))
            {
                ukupnoUlaztr += _vin[i].value;
            }

            ukupnoUlaz = ukupnoUlaztr;

            if (_vin[0].coinbase == true)
            {
                 fee = 0;
            }
            else
            {
                fee = ukupnoUlaz - ukupnoIzlaz;
            }

            
        }
        public class Vin
        {
            public string txid { get; set; }
            public int vout { get; set; }
            public ScriptSig scriptSig { get; set; }
            public object sequence { get; set; }
            public bool coinbase { get; set; }
            public string address { get; set; }
            public decimal value { get; set; }
        }
        public class ScriptSig
        {
            public string asm { get; set; }
            public string hex { get; set; }
        }
        public class Vout
        {
            public decimal value { get; set; }
            public int n { get; set; }
            public bool spent { get; set; }
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
}

