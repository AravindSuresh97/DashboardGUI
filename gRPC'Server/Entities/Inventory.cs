using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gRPCServer.Entities
{
    [Table("Inventory")]
    public class Inventory
{

        [Key] public string Product_ID { get; set; }
        public string Product_Name { get; set; }
        public int Quantity { get; set; }
        public string Date_received { get; set; }
        public int Cost { get; set; }
    }
}
