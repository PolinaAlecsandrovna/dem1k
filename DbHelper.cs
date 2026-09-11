using dem1k.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
