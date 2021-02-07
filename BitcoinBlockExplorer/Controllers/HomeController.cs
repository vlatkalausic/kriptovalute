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
                    
                    ViewBag.USD = Decimal.Round(decimal.Parse(JObject.Parse(x)["data"]["amount"].ToString(), NumberStyles.Currency, CultureInfo.GetCultureInfo("en-US")),8, MidpointRounding.AwayFromZero);

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
                        string rawTx =  JObject.Parse(await rawTransaction.GetRawTransaction(txid))["result"].ToString();
                        JObject getrawtransaction = JObject.Parse(await rawTransaction.DecodeRawTransaction(rawTx));
                        

                        //vin
                        int br2 = getrawtransaction["result"]["vin"].Count();
                        List<Vin> vin = new List<Vin>();
                       
                        if (i == 0)
                        {
                            Vin v = new Vin();
                            ScriptSig ss = new ScriptSig();
                            v.scriptSig = ss;

                            v.txid ="COINBASE";
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
                                string rawTx2 =  JObject.Parse(await rawTransaction2.GetRawTransaction(v.txid))["result"].ToString();
                                JObject getrawtransaction2 = JObject.Parse(await rawTransaction.DecodeRawTransaction(rawTx2));
                                v.value = Decimal.Round(decimal.Parse(getrawtransaction2["result"]["vout"][v.vout]["value"].ToString(), System.Globalization.NumberStyles.Any), 8, MidpointRounding.AwayFromZero);
                                
                                v.address = getrawtransaction2["result"]["vout"][v.vout]["scriptPubKey"]["addresses"][0].ToString(); 

                                vin.Add(v);
                            }
                        }

                        Transaction t = new Transaction(getrawtransaction,vin);
                        transactions.Add(t);

                    }
                    
                    Block block = new Block(getBlock, transactions);


                    return View("BlockDetails", block); //viewbag za usd
                    /*transakcije
                     
                     
                     int getblockcount = Int32.Parse(JObject.Parse(await blockchain.GetBlockCount()).ToString());
                    foreach (int i in Enumerable.Range(0, getblockcount + 1)) 
                    {
                        JObject s = JObject.Parse(await blockchain.GetBlockHash(i));
                        string blockhash = s["result"].ToString();
                        JObject getBlockk = JObject.Parse(await blockchain.GetBlock(blockhash, Verbosity.VerbosityOne));
                        
                        int brtrans= getBlockk["result"]["tx"].Count();
                        foreach (int j in Enumerable.Range(0, brtrans))
                        {
                            //search ce bit
                            if ("7c4694ee86401ca39acdd859f25b439d533c9d5f3cab53c280d72506febfecaf" == getBlockk["result"]["tx"][j].ToString())
                            {
                                RawTransaction rawTransaction = new RawTransaction(bitcoinClient);
                                string rawTx = JObject.Parse(await rawTransaction.GetRawTransaction("7c4694ee86401ca39acdd859f25b439d533c9d5f3cab53c280d72506febfecaf"))["result"].ToString();
                                JObject getrawtransaction = JObject.Parse(await rawTransaction.DecodeRawTransaction(rawTx));
                                Block block1 = new Block(getBlockk);
                                Transaction t = new Transaction(getrawtransaction, block1);
                                //return view tr details
                            }
                        }
                        //return view, block =null ako ne nade
                    }
                        */
                }


            }
            
            return View();
            //return RedirectToAction("");//za gresku nema prikaza
            //return RedirectToAction("Action", new { id = 99 });
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
