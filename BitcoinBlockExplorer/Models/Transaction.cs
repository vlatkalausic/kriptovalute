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
        public int blockh { get; set; }
        public int conf { get; set; }

        public decimal fee { get; set; }

        public DateTime time { get; set; }

        public string status { get; set; }
        public string includedInBlock { get; set; }
        public Transaction(JObject json, Block block, List<Vin> _vin) //details
        {

            txid = json["result"]["txid"].ToString();
            hash = json["result"]["hash"].ToString();
            size = Int32.Parse(json["result"]["size"].ToString());
            weight = Int32.Parse(json["result"]["weight"].ToString());


            if (block != null)
            {
                time = block.time;
                status = "Confirmed";
                includedInBlock = "Yes";
                blockh = block.height;
                conf = block.confirmations;
            }
            else
            {
                status = "Unconfirmed";
                includedInBlock = "No";
                conf = 0;

            }
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
                    v.n= Int32.Parse(json["result"]["vout"][i]["n"].ToString());

                    vout.Add(v);
                }

            }
            ukupnoIzlaz = ukupnoIzlaztr;


            //vin
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
            public ScriptPubKey PKscript { get; set; }
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

