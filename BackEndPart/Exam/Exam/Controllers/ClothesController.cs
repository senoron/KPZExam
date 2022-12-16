using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace Exam.Controllers
{

    [ApiController]
    [Route("[controller]/")]
    public class ClothesController : ControllerBase
    {

        private ILogger<ClothesController> _logger;
        private static String connectionString = "Host=localhost;Username=andrushchak;Password=123456;Database=ClothesShop";
        private static String connectionStringEntity = "Server=localhost;User Id=andrushchak;Password=123456;Database=ClothesShop;";
        private NpgsqlConnection connection = new NpgsqlConnection(connectionString);

        public ClothesController(ILogger<ClothesController> logger)
        {
            _logger = logger;
        }


        [HttpGet(Name = "GetProducts")]
        public IEnumerable<Clothes> Get()
        {
            connection.Open();
            bool update = false;

            var sql = "SELECT * FROM public.\"Clothes\"";

            using var cmd = new NpgsqlCommand(sql, connection);

            using NpgsqlDataReader reader = cmd.ExecuteReader();
            var clothes = new List<Clothes>();
            while (reader.Read())
            {
                Clothes c = new Clothes();
                c.Id = !reader.IsDBNull(0) ? reader.GetInt32(0) : 0;
                c.Name = !reader.IsDBNull(1) ? reader.GetString(1) : null;
                c.Price = !reader.IsDBNull(2) ? reader.GetDouble(2) : 0;
                c.Brand = !reader.IsDBNull(3) ? reader.GetString(3) : null;
                c.Year = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0;
                c.Size = !reader.IsDBNull(5) ? reader.GetString(5) : null;
                c.SellingStartDate = !reader.IsDBNull(6) ? reader.GetDateTime(6) : DateTime.Now;
                c.Quantity = !reader.IsDBNull(7) ? reader.GetInt32(7) : 0;
                c.Discount = !reader.IsDBNull(8) ? reader.GetInt32(8) : 0;

                int disc = CalculateDiscount(c.SellingStartDate);
                if (disc != c.Discount)
                {
                    update = true;
                    c.Discount = disc;
                    if (disc > 0)
                    {
                        c.Price -= Math.Round((c.Price * (((double)disc) / 100)), 2);
                    }
                }
                clothes.Add(c);
            }
            connection.Close();

            if (update) Put(JsonConvert.SerializeObject(clothes));

            return clothes;
        }

        [HttpPut(Name = "UpdateClothes")]
        public IEnumerable<Clothes> Put(String jsonData)
        {
            Console.WriteLine(jsonData);
            List<Clothes> clothes = JsonConvert.DeserializeObject<List<Clothes>>(jsonData);

            connection.Open();

            clothes.ForEach(el =>
            {
                if (el.Id != 0 && el.Id != null)
                {
                    string commandText = $"UPDATE public.\"Clothes\" SET \"Name\" = @name, \"Price\" = @price, \"Brand\" = @brand, \"Year\" = @year, \"Size\" = @size, \"SellingStartDate\" = @startDate, \"Quantity\" = @quantity, \"Discount\" = @discount WHERE \"Id\" = @id";
                    using var cmd = new NpgsqlCommand(commandText, connection);
                    cmd.Parameters.AddWithValue("id", el.Id);
                    cmd.Parameters.AddWithValue("name", el.Name);
                    cmd.Parameters.AddWithValue("price", el.Price);
                    cmd.Parameters.AddWithValue("brand", el.Brand);
                    cmd.Parameters.AddWithValue("year", el.Year);
                    cmd.Parameters.AddWithValue("size", el.Size);
                    cmd.Parameters.AddWithValue("startDate", el.SellingStartDate);
                    cmd.Parameters.AddWithValue("quantity", el.Quantity);
                    cmd.Parameters.AddWithValue("discount", CalculateDiscount(el.SellingStartDate));

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    string commandText = $"INSERT INTO public.\"Clothes\" (\"Name\", \"Price\", \"Brand\", \"Year\", \"Size\", \"SellingStartDate\", \"Quantity\", \"Discount\") VALUES (@name, @price, @brand, @year, @size, @startDate, @quantity, @discount)";
                    using var cmd = new NpgsqlCommand(commandText, connection);
                    cmd.Parameters.AddWithValue("name", el.Name);
                    cmd.Parameters.AddWithValue("price", el.Price);
                    cmd.Parameters.AddWithValue("brand", el.Brand);
                    cmd.Parameters.AddWithValue("year", el.Year);
                    cmd.Parameters.AddWithValue("size", el.Size);
                    cmd.Parameters.AddWithValue("startDate", el.SellingStartDate);
                    cmd.Parameters.AddWithValue("quantity", el.Quantity);
                    cmd.Parameters.AddWithValue("discount", el.Discount == 0 ? CalculateDiscount(el.SellingStartDate) : el.Discount);

                    cmd.ExecuteNonQuery();
                }
            });

            connection.Close();

            return Get();
        }

        [HttpDelete(Name = "DeleteClothes")]
        public void Delete(int Id)
        {
            connection.Open();

            string commandText = $"DELETE FROM public.\"Clothes\" WHERE \"Id\"=(@id)";
            using var cmd = new NpgsqlCommand(commandText, connection);
            cmd.Parameters.AddWithValue("id", Id);


            cmd.ExecuteNonQuery();

            connection.Close();
        }

        private int CalculateDiscount(DateTime date)
        {
            DateTime today = DateTime.Today;
            int count = ((date.Year - today.Year) * 12) + date.Month - today.Month;

            if(count < 0)
            {
                count = Math.Abs(count);

                if (count > 2 && count < 4) return 10;
                if (count > 4 && count < 6) return 20;
                if (count > 6 && count < 8) return 30;
                if (count > 8 && count < 10) return 40;
                if (count > 10 && count < 12) return 50;
                else return 80;
            }

            return 0;

        }
    }
}

