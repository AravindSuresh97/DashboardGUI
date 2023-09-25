

using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Grpc.Net.Client;
using gRPC_Client;
using gRPCClient.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MiFare;
using MiFare.Classic;
using MiFare.Devices;
using Sydesoft.NfcDevice;
using System.Reflection.PortableExecutable;
using System.Text;
using static gRPC_Client.ProductSrv;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ClosedXML.Excel;


namespace gRPCClient.Controllers
{
    public class ProductController : Controller
    {
        Reader.NFCReader NFC = new Reader.NFCReader();
        private static ACR122U acr122u = new ACR122U();
        private readonly string GrpcChannelURL = "https://localhost:7254";

       
        // GET: ProductController
        public ActionResult Index()
        {
            DeinitializeReader();
            using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
            var client = new ProductSrvClient(channel);
            var products = client.GetAll(new gRPC_Client.Empty { });

            if (GlobalVariables.LoginFlag)
                return View(products);
            else
                return RedirectToAction("Login", "Product");
            
        }
     
        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            DeinitializeReader();
            if (GlobalVariables.LoginFlag)
                return View();
            else
                return RedirectToAction("Login", "Product");
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            DeinitializeReader();
            if (GlobalVariables.LoginFlag)
                return View();

            else
                return RedirectToAction("Login", "Product");
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Inventory product)
        {
            try
            {
                DeinitializeReader();
                using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                //await client.PostAsync(product);

                var val = await client.PostAsync(product);
                if (val.Success)
                {
                    GlobalVariables.ProductExists = false;
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    GlobalVariables.ProductExists = true;
                    return RedirectToAction(nameof(Create)); ;
                }

                //return RedirectToAction(nameof(Index));
            }
            catch
            {
                if (GlobalVariables.LoginFlag)
                    return View();
                else
                    return RedirectToAction("Login", "Product");
            }
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            DeinitializeReader();
            var channel = GrpcChannel.ForAddress(GrpcChannelURL);
            var client = new ProductSrvClient(channel);
            var product = await client.GetByIdAsync(new ProductRowIdFilter { ProductID = id });
            if (GlobalVariables.LoginFlag)
                return View(product);
            else
                return RedirectToAction("Login", "Product"); 
        }

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Inventory product)
        {
            try
            {
                DeinitializeReader();
                var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                await client.PutAsync(product);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductController/Delete/5
        public async Task<ActionResult> DeleteAsync(string id)
        {
            DeinitializeReader();
            var channel = GrpcChannel.ForAddress(GrpcChannelURL);
            var client = new ProductSrvClient(channel);
            var product = await client.GetByIdAsync(new ProductRowIdFilter { ProductID = id });
            if (GlobalVariables.LoginFlag)
                return View(product);
            else
                return RedirectToAction("Login", "Product");
           
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id, IFormCollection collection)
        {
            try
            {
                DeinitializeReader();
                var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                client.DeleteAsync(new ProductRowIdFilter { ProductID = id });
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Visualization()
        {
            DeinitializeReader();
            using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
            var client = new ProductSrvClient(channel);
            var products = client.GetAll(new gRPC_Client.Empty { });

            if (GlobalVariables.LoginFlag)
                return View(products);
            else
                return RedirectToAction("Login", "Product");
            
        }

        // GET: ProductController/Login
        public ActionResult Login()
        {
            DeinitializeReader();
            return View();
        }

        public ActionResult Logout()
        {
            DeinitializeReader();
            GlobalVariables.LoginFlag = false;
            return RedirectToAction(nameof(Index));
        }
        public ActionResult AddUser()
        {
            DeinitializeReader();
            if (GlobalVariables.LoginFlag)
            {
               return View();
            }
            else
                return RedirectToAction("Login", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginDetail product)
        {
            try
            {
                DeinitializeReader();
                using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                var flag = await client.LoginCheckAsync(product);
                if(flag.Success)
                {
                    GlobalVariables.LoginFlag = true;
                    GlobalVariables.WrongCreds = false;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    GlobalVariables.LoginFlag = false;
                    GlobalVariables.WrongCreds = true;
                    return RedirectToAction(nameof(Login));
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddUser(LoginDetail product)
        {
            try
            {
                DeinitializeReader();
                using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                var val = await client.PostLoginAsync(product);
                GlobalVariables.LoginFlag = true;
                if (val.Success)
                {
                    GlobalVariables.UsernameExists = false;
                    return RedirectToAction(nameof(Index));
                    
                }
                else
                {
                    GlobalVariables.UsernameExists = true;
                    return RedirectToAction(nameof(AddUser)); ;
                }
            }
            catch
            {
                return View();
            }
        }

        public  ActionResult ReadData()
        {
            try
            {
                if (GlobalVariables.LoginFlag)
                {
                    var UID = acr122u.GetUID;
                    acr122u.Init(false, 50, 4, 4, 200);  // NTAG213

                    acr122u.CardInserted += Acr122u_CardInserted;
                    acr122u.CardRemoved += Acr122u_CardRemoved;
                    GlobalVariables.ReadStart = true;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", "Product");
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
        public ActionResult WriteData()
        {
          
            if (GlobalVariables.LoginFlag)
                return View();

            else
                return RedirectToAction("Login", "Product");
        }
        public async Task<ActionResult> WriteData(Inventory writeProduct)
        {
            
                acr122u.Init(false, 50, 4, 4, 200);  

                return View(GlobalVariables.scanInv);
            
           

        }
        private async void Acr122u_CardInserted(PCSC.ICardReader reader)
        {
            if(GlobalVariables.ReadStart)
            {
                GlobalVariables.ReadStart = false;
                var Readerdata = Encoding.UTF8.GetString(acr122u.ReadData(reader));
                string str = Readerdata.TrimEnd('\0');
                string[] dataComponents = str.Split(';');

                // Ensure that there are enough components in the data (expected count is 5)
                if (dataComponents.Length == 6)
                {
                    string productId = dataComponents[0];
                    string productName = dataComponents[1];
                    string dateReceived = dataComponents[2];
                    string quantity = dataComponents[3];
                    string cost = dataComponents[4];
                    if (int.TryParse(quantity, out int quantityInt))
                    {
                        GlobalVariables.scanInv.Quantity = quantityInt;
                    }
                    if (int.TryParse(cost, out int costInt))
                    {
                        GlobalVariables.scanInv.Cost = costInt;
                    }
                    GlobalVariables.scanInv.ProductID = productId;
                    GlobalVariables.scanInv.ProductName = productName;
                    GlobalVariables.scanInv.DateRecieved = dateReceived;
                    var controller = new ProductController();
                    await controller.scanCreate(GlobalVariables.scanInv);



                }
                else
                {
                    //Error. Use a Valid Card
                    //No enough data in the card
                }
            }


        }
        private async static void Acr122u_CardInserted_Write(PCSC.ICardReader reader)
        {
            Console.WriteLine("NFC tag placed on reader.");
            Console.WriteLine("Unique ID: " + BitConverter.ToString(acr122u.GetUID(reader)).Replace("-", ""));
            string data = "9;Product 9;2023-06-30;24;150;";
            Console.WriteLine("Write data to NFC tag: " + data);
            bool ret = acr122u.WriteData(reader, Encoding.UTF8.GetBytes(data));
            Console.WriteLine("Write result: " + (ret ? "Success" : "Failed"));
        }
        private static void Acr122u_CardRemoved_Write()
        {
            Console.WriteLine("NFC tag removed.");
        }
        private void DeinitializeReader()
        {
            // Remove event handlers
            acr122u.CardInserted -= Acr122u_CardInserted;
            acr122u.CardRemoved -= Acr122u_CardRemoved;

        }

        private static void Acr122u_CardRemoved()
        {
            Console.WriteLine("NFC tag removed.");
            GlobalVariables.ReadStart = true;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task scanCreate(Inventory scanProduct)
        {
            try
            {
                
                    using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                    var client = new ProductSrvClient(channel);
                    var response = client.GetAll(new gRPC_Client.Empty { });
                    var allProducts = response.Items;
                    bool productExists = allProducts.Any(p => p.ProductName == scanProduct.ProductName);
                    if (productExists)
                    {
                        var matchingPdtQty = allProducts.FirstOrDefault(p => p.ProductName == scanProduct.ProductName).Quantity;
                        //scanProduct.Quantity = scanProduct.Quantity + matchingPdtQty;
                        //await client.PutAsync(scanProduct);
                        var updatedProduct = new Inventory
                        {
                            ProductName = scanProduct.ProductName,
                            Quantity = scanProduct.Quantity + matchingPdtQty,
                            Cost = scanProduct.Cost,
                            DateRecieved = scanProduct.DateRecieved,
                            ProductID = scanProduct.ProductID
                        };
                        await client.PutAsync(updatedProduct);

                    }
                    else
                    {
                        await client.PostAsync(scanProduct);
                    }
                    //return RedirectToAction("ReadData");
                    GlobalVariables.IsSuccess = true;
                
            }
            catch
            {
                //return RedirectToAction("ReadData");
            }
        }

        public ActionResult ExportToExcel()
        {
            if (GlobalVariables.LoginFlag)
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Inventory");

                // Add headers
                worksheet.Cell(1, 1).Value = "Product ID";
                worksheet.Cell(1, 2).Value = "Product Name";
                worksheet.Cell(1, 3).Value = "Quantity";
                worksheet.Cell(1, 4).Value = "Date Received";
                worksheet.Cell(1, 5).Value = "Cost";

                using var channel = GrpcChannel.ForAddress(GrpcChannelURL);
                var client = new ProductSrvClient(channel);
                var response = client.GetAll(new gRPC_Client.Empty { });
                var allProducts = response.Items;
                // Add data rows
                int row = 2;
                foreach (var product in allProducts)
                {
                    worksheet.Cell(row, 1).Value = product.ProductID;
                    worksheet.Cell(row, 2).Value = product.ProductName;
                    worksheet.Cell(row, 3).Value = product.Quantity;
                    worksheet.Cell(row, 4).Value = product.DateRecieved;
                    worksheet.Cell(row, 5).Value = product.Cost;
                    row++;
                }

           
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.Headers.Add("content-disposition", "attachment;filename=inventory.xlsx");

                // Save the workbook directly to the response stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.WriteTo(Response.Body);
                }

                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("Login", "Product");
            }
        }
    }

   
}

