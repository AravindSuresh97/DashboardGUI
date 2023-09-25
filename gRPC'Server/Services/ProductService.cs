
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using gRPC_Server;
using gRPCServer.Data;
using gRPCServer.Entities;
using System.Security.Cryptography;
using System.Text;
using static gRPC_Server.ProductSrv;
using static gRPC_Server.ProductSrv.ProductSrvBase;
using Inventory = gRPCServer.Entities.Inventory;

public class ProductService : ProductSrv.ProductSrvBase
{
    private readonly ProductContext _context;

    public ProductService(ProductContext context)
    {
        _context = context;
    }

    public override Task<Inventorys> GetAll(gRPC_Server.Empty request, ServerCallContext context)
    {
        var response = new Inventorys();

        var products = (from c in _context.Inventory
                        select new gRPC_Server.Inventory
                        {
                            ProductID = c.Product_ID,
                            ProductName = c.Product_Name,
                            Quantity = c.Quantity,
                            DateRecieved = c.Date_received,
                            Cost = c.Cost
                        }).OrderBy(p => p.ProductName).ToArray();


             
        response.Items.AddRange(products);
        return Task.FromResult(response);

    }

    public override Task<gRPC_Server.LoginDetails> GetAllLoginDetails(gRPC_Server.Empty request, ServerCallContext context)
    {
        var response = new gRPC_Server.LoginDetails();

        var loginDetails = (from c in _context.LoginDetail
                        select new gRPC_Server.LoginDetail
                        {                       
                            Username = c.Username,
                            Passwordhash = c.PasswordHash,
                            PasswordSalt = c.PasswordSalt
                        }).OrderBy(p => p.Username).ToArray();



        response.Items.AddRange(loginDetails);
        return Task.FromResult(response);

    }

    public override Task<gRPC_Server.Inventory> GetById(ProductRowIdFilter request, ServerCallContext context)
    {
        var product = _context.Inventory.Where(w => w.Product_ID == request.ProductID).FirstOrDefault();
        var sProduct = new gRPC_Server.Inventory
        {
            ProductID = product.Product_ID,
            ProductName = product.Product_Name,
            Quantity = product.Quantity,
            DateRecieved = product.Date_received,
            Cost = product.Cost
        };

        return Task.FromResult(sProduct);

    }

    public override Task<BoolResponse> Post(gRPC_Server.Inventory request, ServerCallContext context)
    {
        var allProductDetails = GetAll(new gRPC_Server.Empty(), context).Result.Items;
        var matchedPdtName = allProductDetails.FirstOrDefault(pdt => pdt.ProductName == request.ProductName);
        var matchedPdtId = allProductDetails.FirstOrDefault(pdt => pdt.ProductID == request.ProductID);
        if (matchedPdtName==null && matchedPdtId==null)
        {
            var product = new gRPCServer.Entities.Inventory()
            {
                Product_Name = request.ProductName,
                Product_ID = request.ProductID,
                Quantity = request.Quantity,
                Date_received = request.DateRecieved,
                Cost = request.Cost
            };

            var response = _context.Inventory.Add(product);
            _context.SaveChanges();

            var resProduct = new gRPC_Server.Inventory()
            {
                ProductID = response.Entity.Product_ID,
                ProductName = response.Entity.Product_Name,
                Quantity = response.Entity.Quantity,
                DateRecieved = response.Entity.Date_received,
                Cost = response.Entity.Cost
            };


            //return Task.FromResult(resProduct);
            return Task.FromResult(new BoolResponse { Success = true });
        }
        else
        {
            return Task.FromResult(new BoolResponse { Success = false });
        }
    }

    public override Task<BoolResponse> PostLogin(gRPC_Server.LoginDetail request, ServerCallContext context)
    {
        var allLoginDetails = GetAllLoginDetails(new gRPC_Server.Empty(), context).Result.Items;
        var matchedItem = allLoginDetails.FirstOrDefault(login => login.Username == request.Username);
        if (matchedItem == null)
        {
            // Generate a random salt
            byte[] salt = GenerateSalt();

            // Hash the password using the salt
            byte[] passwordHash = GeneratePasswordHash(request.Passwordhash, salt);
            var product = new gRPCServer.Entities.LoginDetails()
            {
                Username = request.Username,
                //PasswordSalt = request.PasswordSalt,
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = Convert.ToBase64String(passwordHash)
            };

            var response = _context.LoginDetail.Add(product);
            _context.SaveChanges();

            var resProduct = new gRPC_Server.LoginDetail()
            {
                Username = response.Entity.Username,
                Passwordhash = response.Entity.PasswordHash,
                PasswordSalt = response.Entity.PasswordSalt
            };


            return Task.FromResult(new BoolResponse { Success = true });
        }
        else
        {
            return Task.FromResult(new BoolResponse { Success = false });
        }
    }
    public override Task<BoolResponse> LoginCheck(gRPC_Server.LoginDetail request, ServerCallContext context)
    {
        if(request.Username == null)
            return Task.FromResult(new BoolResponse { Success = false });

        var allLoginDetails = GetAllLoginDetails(new gRPC_Server.Empty(), context).Result.Items;


        var matchedItem = allLoginDetails.FirstOrDefault(login => login.Username == request.Username);
        if (matchedItem != null)
        {
            var enteredPass = request.Passwordhash;
            var matchedUsername = matchedItem.Username;
            var matchedHash = matchedItem.Passwordhash;
            var matchedSalt = matchedItem.PasswordSalt;
            byte[] storedSalt = Convert.FromBase64String(matchedSalt);
            byte[] storedPasswordHash = Convert.FromBase64String(matchedHash);
            byte[] toCheckPasswordHash=GeneratePasswordHash(request.Passwordhash, storedSalt);
            if (toCheckPasswordHash.SequenceEqual(storedPasswordHash) && matchedUsername == request.Username)
            {
                return Task.FromResult(new BoolResponse { Success = true });
            }
            else
            {
                return Task.FromResult(new BoolResponse { Success = false });
            }

        }
        else
        {
            return Task.FromResult(new BoolResponse { Success = false });
        }

    }


        // Function to generate a random salt
        private byte[] GenerateSalt()
    {
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        return salt;
    }

    // Function to generate the password hash using the salt
    private byte[] GeneratePasswordHash(string password, byte[] salt)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] passwordWithSaltBytes = new byte[passwordBytes.Length + salt.Length];

        Buffer.BlockCopy(passwordBytes, 0, passwordWithSaltBytes, 0, passwordBytes.Length);
        Buffer.BlockCopy(salt, 0, passwordWithSaltBytes, passwordBytes.Length, salt.Length);

        byte[] hashBytes;
        using (var sha256 = SHA256.Create())
        {
            hashBytes = sha256.ComputeHash(passwordWithSaltBytes);
        }
        return hashBytes;
    }
    public override Task<gRPC_Server.Inventory> Put(gRPC_Server.Inventory request, ServerCallContext context)
    {
        var product = _context.Inventory.Where(w => w.Product_ID == request.ProductID).FirstOrDefault();

        if (product == null)
            return Task.FromResult<gRPC_Server.Inventory>(null);

        product.Product_ID = request.ProductID;
        product.Product_Name = request.ProductName;
        product.Quantity = request.Quantity;
        product.Date_received = request.DateRecieved;
        product.Cost = request.Cost;

        _context.Update(product);
        _context.SaveChanges();

        var sProduct = new gRPC_Server.Inventory()
        {
            ProductID = request.ProductID,
            ProductName = request.ProductName,
            Quantity = request.Quantity,
            DateRecieved= request.DateRecieved,
            Cost = request.Cost
        };
        return Task.FromResult(sProduct);
    }

    public override Task<gRPC_Server.Empty> Delete(ProductRowIdFilter request, ServerCallContext context)
    {
        var product = _context.Inventory.Where(w => w.Product_ID == request.ProductID).FirstOrDefault();

        if (product == null)
            return Task.FromResult<gRPC_Server.Empty>(null);

        _context.Remove(product);
        _context.SaveChanges();

        return Task.FromResult<gRPC_Server.Empty>(new gRPC_Server.Empty());

    }
}