using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbLibrary.ModelsDb
{
    [Table(name: "product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("type")]
        public string Type { get; set; }

        [Column("size")]
        public int Size { get; set; }

        [Column("color")]
        public string Color { get; set; }

        [Column("amount")]
        public int Amount { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("sold")]
        public int Sold { get; set; }// проданно
    }
}
