using dem1k.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;
using System.Threading.Tasks;

namespace dem1k
{
    public static class DbHelper
    {
        private static string connectionString = "Host=localhost;Port=5432;Database=dem;Username=postgres;Password=Password";

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public static List<ProductViewModel> GetProducts()
        {
            List<ProductViewModel> list = new List<ProductViewModel>();

            string sql = @"
                SELECT p.id, p.article, p.name, c.name as category, p.description, 
                       m.name as manufacturer, s.name as supplier, p.price, 
                       u.name as unit, p.stock_quantity, p.discount_percent, p.image_path
                FROM products p
                LEFT JOIN categories c ON p.category_id = c.id
                LEFT JOIN manufacturers m ON p.manufacturer_id = m.id
                LEFT JOIN suppliers s ON p.supplier_id = s.id
                LEFT JOIN units u ON p.unit_id = u.id
                ORDER BY p.name";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ProductViewModel
                            {
                                Id = reader.GetInt32(0),
                                Article = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                Name = reader.GetString(2),
                                CategoryName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                ManufacturerName = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                SupplierName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                Price = reader.GetDecimal(7),
                                UnitName = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                StockQuantity = reader.GetInt32(9),
                                DiscountPercent = reader.GetDecimal(10),
                                ImagePath = reader.IsDBNull(11) ? null : reader.GetString(11)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static bool AuthUser(string login, string password, out string userName, out string roleName)
        {
            userName = null;
            roleName = null;
            string sql = @"
                SELECT u.name, u.surname, r.name 
                FROM users u 
                JOIN roles r ON u.role_id = r.id 
                WHERE u.login = @l AND u.password_hash = @p";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("l", login);
                    cmd.Parameters.AddWithValue("p", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userName = $"{reader.GetString(1)} {reader.GetString(0)}";
                            roleName = reader.GetString(2);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static void AddProduct(ProductViewModel product)
        {
            string sql = @"
        INSERT INTO products (name, description, category_id, manufacturer_id, supplier_id, unit_id, price, stock_quantity, discount_percent, image_path, article)
        VALUES (@name, @desc, 1, 1, 1, 1, @price, @qty, @discount, @img, @article)";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("desc", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("price", product.Price);
                    cmd.Parameters.AddWithValue("qty", product.StockQuantity);
                    cmd.Parameters.AddWithValue("discount", product.DiscountPercent);
                    cmd.Parameters.AddWithValue("img", (object)product.ImagePath ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("article", "ART-" + Guid.NewGuid().ToString().Substring(0, 5).ToUpper());

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateProduct(ProductViewModel product)
        {
            string sql = @"
        UPDATE products 
        SET name = @name, 
            description = @desc, 
            price = @price, 
            stock_quantity = @qty, 
            discount_percent = @discount, 
            image_path = @img
        WHERE id = @id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", product.Id);
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("desc", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("price", product.Price);
                    cmd.Parameters.AddWithValue("qty", product.StockQuantity);
                    cmd.Parameters.AddWithValue("discount", product.DiscountPercent);
                    cmd.Parameters.AddWithValue("img", (object)product.ImagePath ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<OrderViewModel> GetOrders()
        {
            List<OrderViewModel> list = new List<OrderViewModel>();
            string sql = @"
        SELECT o.id, o.article, os.name as status, 
               COALESCE(c.name || ', ул. ' || s.name || ', д. ' || pa.house, 'Не указан') as address,
               o.order_date, o.delivery_date
        FROM orders o
        LEFT JOIN order_statuses os ON o.status_id = os.id
        LEFT JOIN pickup_address pa ON o.pickup_address_id = pa.id
        LEFT JOIN cities c ON pa.id_cities = c.id
        LEFT JOIN streets s ON pa.id_streets = s.id
        ORDER BY o.order_date DESC";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new OrderViewModel
                        {
                            Id = reader.GetInt32(0),
                            Article = reader.IsDBNull(1) ? "" : reader.GetString(1),
                            StatusName = reader.IsDBNull(2) ? "Не указан" : reader.GetString(2),
                            PickupAddress = reader.IsDBNull(3) ? "Не указан" : reader.GetString(3),
                            OrderDate = reader.GetDateTime(4),
                            DeliveryDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                        });
                    }
                }
            }
            return list;
        }

        public static List<string> GetOrderStatuses()
        {
            List<string> list = new List<string>();
            string sql = "SELECT name FROM order_statuses ORDER BY id";
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(reader.GetString(0));
                }
            }
            return list;
        }

        public static void AddOrder(string article, string statusName, string address, DateTime orderDate, DateTime? deliveryDate)
        {
            string sql = @"
        INSERT INTO orders (article, status_id, pickup_address_id, user_id, order_date, delivery_date)
        VALUES (@art, (SELECT id FROM order_statuses WHERE name = @status LIMIT 1), 1, 1, @odate, @ddate)";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("art", article);
                    cmd.Parameters.AddWithValue("status", statusName);
                    cmd.Parameters.AddWithValue("odate", orderDate);
                    cmd.Parameters.AddWithValue("ddate", (object)deliveryDate ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpdateOrder(int id, string article, string statusName, DateTime orderDate, DateTime? deliveryDate)
        {
            string sql = @"
        UPDATE orders 
        SET article = @art, 
            status_id = (SELECT id FROM order_statuses WHERE name = @status LIMIT 1), 
            order_date = @odate, 
            delivery_date = @ddate
        WHERE id = @id";

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.Parameters.AddWithValue("art", article);
                    cmd.Parameters.AddWithValue("status", statusName);
                    cmd.Parameters.AddWithValue("odate", orderDate);
                    cmd.Parameters.AddWithValue("ddate", (object)deliveryDate ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void DeleteOrder(int id)
        {
            string sql = "DELETE FROM orders WHERE id = @id";
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
