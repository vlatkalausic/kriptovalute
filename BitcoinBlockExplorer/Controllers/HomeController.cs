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
                var isNumeric = int.TryParse(search, out int n);
                if (isNumeric)
                {
                    JObject s = JObject.Parse(await blockchain.GetBlockHash(n));
                    search = s["result"].ToString();
                }
                //hash
                JObject getBlock = JObject.Parse(await blockchain.GetBlock(search, Verbosity.VerbosityTwo));
  
                if (string.IsNullOrEmpty(getBlock["error"].ToString()))
                {
                    Block block = new Block(getBlock);
                    return View("BlockDetails", block);
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
