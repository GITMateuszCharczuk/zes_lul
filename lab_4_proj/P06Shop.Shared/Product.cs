using System.ComponentModel.DataAnnotations;

namespace P06Shop.Shared
{
    //// data annotations stostosujemy gdy model nie jest współdzielony z innymi projektami
    //public class Product
    //{
    //    [Key]
    //    public int Code { get; set;}
    //    public int Id { get; set; }

    //    [MaxLength(50)] // zmapuje na nvarchar(50)
    //    public string Title { get; set; }
    //    public string Description { get; set; }

    //    public string Barcode { get; set; }

    //    public double Price { get; set; }

    //    public DateTime ReleaseDate { get; set; }

    //}

    // fluent api stosujemy gdy model jest współdzielony z innymi projektami
    public class Product 
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string Barcode { get; set; }

        public double Price { get; set; } // domyślnie mapuje na decimal(18,2)

        public DateTime ReleaseDate { get; set; }

    }
}
