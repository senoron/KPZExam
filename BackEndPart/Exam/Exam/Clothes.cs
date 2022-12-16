using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
namespace Exam
{
	public class Clothes
	{
		[Key]
		public int Id { get; set; }
		public String Name { get; set; }
		public double Price { get; set; }
		public String Brand { get; set; }
		public int Year { get; set; }
		public String Size { get; set; }
		public DateTime SellingStartDate { get; set; }
		public int Quantity { get; set; }
		public int Discount { get; set; }
	}
}

