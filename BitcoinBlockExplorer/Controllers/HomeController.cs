using BitcoinBlockExplorer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BitcoinRpc;
using BitcoinRpc.CoreRPC;
using Newtonsoft.Json.Linq;
using BitcoinRpc.Enums;
using BitcoinRpc.RequestModels.Blockchain;
using System.Net.Http;
using static BitcoinBlockExplorer.Models.Transaction;
using System.Globalization;

namespace BitcoinBlockExplorer.Controllers
{
    public class HomeController : Controller
    {
        public BitcoinClient bitcoinClient;
        public Blockchain blockchain;
        public HomeController()
        {
            string[] lines = System.IO.File.ReadAllLines(@"C:\Users\Asus\Desktop\accessData.txt");

            bitcoinClient = new BitcoinClient(lines[0],lines[1]);
            blockchain = new Blockchain(bitcoinClient);
        }
        
        public async Task<ActionResult> Index(string search=null)
        {
            if (!string.IsNullOrEmpty(search))
            {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.coinbase.com/v2/prices/spot?currency=USD");


                if (response.IsSuccessStatusCode)
                {
                    var x = response.Content.ReadAsStringAsync().Result;

                    ViewBag.USD = Decimal.Round(decimal.Parse(JObject.Parse(x)["data"]["amount"].ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US")), 8, MidpointRounding.AwayFromZero);

                }


                var isNumeric = int.TryParse(search, out int n);
                if (isNumeric)
                {
                    JObject s = JObject.Parse(await blockchain.GetBlockHash(n));
                    search = s["result"].ToString();
                }
                //hash block
                JObject getBlock = JObject.Parse(await blockchain.GetBlock(search, Verbosity.VerbosityTwo));
                if (string.IsNullOrEmpty(getBlock["error"].ToString()))
                {

                    List<Transaction> transactions = new List<Transaction>();
                    int br = getBlock["result"]["tx"].Count();
                    foreach (int i in Enumerable.Range(0, br))
                    {
                        string txid = getBlock["result"]["tx"][i]["txid"].ToString();
                        RawTransaction rawTransaction = new RawTransaction(bitcoinClient);
                        string rawTx = JObject.Parse(await rawTransaction.GetRawTransaction(txid))["result"].ToString();
                        JObject getrawtransaction = JObject.Parse(await rawTransaction.DecodeRawTransaction(rawTx));


                        //vin
                        int br2 = getrawtransaction["result"]["vin"].Count();
                        List<Vin> vin = new List<Vin>();

                        if (i == 0)
                        {
                            Vin v = new Vin();
                            ScriptSig ss = new ScriptSig();
                            v.scriptSig = ss;

                            v.txid = "COINBASE";
                            v.address = "COINBASE";
                            v.coinbase = true;

                            vin.Add(v);

                        }
                        else
                        {
                            foreach (int j in Enumerable.Range(0, br2))
                            {
                                Vin v = new Vin();
                                ScriptSig ss = new ScriptSig();
                                v.scriptSig = ss;
                                v.txid = getrawtransaction["result"]["vin"][j]["txid"].ToString();
                                v.coinbase = false;
                                v.vout = Int32.Parse(getrawtransaction["result"]["vin"][j]["vout"].ToString());


                                RawTransaction rawTransaction2 = new RawTransaction(bitcoinClient);
                                string rawTx2 = JObject.Parse(await rawTransaction2.GetRawTransaction(v.txid))["result"].ToString();
                                JObject getrawtransaction2 = JObject.Parse(await rawTransaction.DecodeRawTransaction(rawTx2));
                                v.value = Decimal.Round(decimal.Parse(getrawtransaction2["result"]["vout"][v.vout]["value"].ToString(), System.Globalization.NumberStyles.Any), 8, MidpointRounding.AwayFromZero);

                                v.address = getrawtransaction2["result"]["vout"][v.vout]["scriptPubKey"]["addresses"][0].ToString();

                                vin.Add(v);
                            }
                        }

                        Transaction t = new Transaction(getrawtransaction, vin);
                        transactions.Add(t);

                    }

                    Block block = new Block(getBlock, transactions);

                    return View("BlockDetails", block);
                    
                }

                //hash transakcije
                

                RawTransaction rawTransaction3 = new RawTransaction(bitcoinClient);
                string rawTx3 = JObject.Parse(await rawTransaction3.GetRawTransaction(search))["result"].ToString();
                JObject getrawtransaction3 = JObject.Parse(await rawTransaction3.DecodeRawTransaction(rawTx3,true));
                
                if (string.IsNullOrEmpty(getrawtransaction3["error"].ToString())){

                    Block block1 = null;
                    if (search == "cf373da6b9081993a2acc011e9fdcc47fc38c138bd3a7b1e9749762a54fc9251" || search=="7c4694ee86401ca39acdd859f25b439d533c9d5f3cab53c280d72506febfecaf")
                    {
                        JObject s = JObject.Parse(await blockchain.GetBlockHash(999999));
                        string blockhash = s["result"].ToString();
                        JObject getBlockk = JObject.Parse(await blockchain.GetBlock(blockhash, Verbosity.VerbosityOne));
                        block1 = new Block(getBlockk);
                    }
                    else
                    {
                        int getblockcount = Int32.Parse(JObject.Parse(await blockchain.GetBlockCount())["result"].ToString());
                   
                        // blok 1934411, transakcija 
                        //5413e73e1a5f0213c3abfb7f4a50ca2c6e662760f95620a0795e894ecf1b68be
                        while (getblockcount + 1 >= 0 && block1 == null)
                        {
                            JObject s = JObject.Parse(await blockchain.GetBlockHash(getblockcount));
                            string blockhash = s["result"].ToString();
                            JObject getBlockk = JObject.Parse(await blockchain.GetBlock(blockhash, Verbosity.VerbosityOne));

                            int brtrans = getBlockk["result"]["tx"].Count();
                            foreach (int j in Enumerable.Range(0, brtrans))
                            {

                                if (search == getBlockk["result"]["tx"][j].ToString())
                                {

                                    Block blockza1 = new Block(getBlockk);
                                    block1 = blockza1;
                                    break;

                                }
                            }
                            getblockcount -= 1;

                        }

                    }
                    //vin

                    int br3 = getrawtransaction3["result"]["vin"].Count();
                    List<Vin> vinstrans=new List<Vin>();
                    Vin v2 = new Vin();
                    ScriptSig ss2 = new ScriptSig();
               
                    foreach (int z in Enumerable.Range(0, br3))
                    {
                        //JObject novi = new JObject(getrawtransaction3["result"]["vin"][z].ToString());
                        String strodJ = getrawtransaction3["result"]["vin"][z].ToString();
                        
                        if (strodJ.IndexOf("coinbase") > -1)
                        {
                            
                            v2.txid = "COINBASE";
                            v2.address = "COINBASE";
                            v2.coinbase = true;
                            v2.value = 0;
                            

                            vinstrans.Add(v2);
                        }
                        else
                        {

                            v2.coinbase = false;
                        
                            v2.txid = getrawtransaction3["result"]["vin"][z]["txid"].ToString();
                            v2.vout = Int32.Parse(getrawtransaction3["result"]["vin"][z]["vout"].ToString());


                            RawTransaction rawTr = new RawTransaction(bitcoinClient);
                            string rT= JObject.Parse(await rawTr.GetRawTransaction(v2.txid))["result"].ToString();
                            JObject getrawtransaction4 = JObject.Parse(await rawTr.DecodeRawTransaction(rT));

                            v2.value = Decimal.Round(decimal.Parse(getrawtransaction4["result"]["vout"][v2.vout]["value"].ToString(), System.Globalization.NumberStyles.Any), 8, MidpointRounding.AwayFromZero);

                            v2.address = getrawtransaction4["result"]["vout"][v2.vout]["scriptPubKey"]["addresses"][0].ToString();
                            vinstrans.Add(v2);
                        }
                       
                       
                    }
                    Transaction transaction = new Transaction(getrawtransaction3, block1, vinstrans);
                    return View("TransactionDetails", transaction);
                }
                ViewBag.message = "NEMA PODATAKA!";
            }
            int recentb = Int32.Parse(JObject.Parse(await blockchain.GetBlockCount())["result"].ToString());
            ViewBag.rb = recentb;
            var getrawmempool = JObject.Parse(await blockchain.GetRawMempool(ReturnFormat.ArrayOfTransactionIds));
            var ddd = await blockchain.GetRawMempool(ReturnFormat.ArrayOfTransactionIds);
            int count = getrawmempool["result"].Count();
            List<string> listart = new List<string>();
            foreach (int c in Enumerable.Range(0, count))
            {
                listart.Add(getrawmempool["result"][c].ToString());
            }
            ViewBag.rt= listart;
            return View();
        }
      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
