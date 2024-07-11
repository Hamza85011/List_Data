using List_Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace List_Data.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connectionString;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("cs");
        }

        List<Info> User = new List<Info>
        {
            new Info { Id = 1, Name = "Hamza", Description = "asdfghgfd", Data = "dfghjkl"},
            new Info { Id = 2, Name = "Ali", Description = "fghjkl;", Data = "Num"},
            new Info { Id = 3, Name = "Hamza", Description = "asdfghgfdddddddd", Data = "dfghjkl"},
            new Info { Id = 4, Name = "Ali", Description = "fghjkl;", Data = "Num"},
            new Info { Id = 5, Name = "Hamza", Description = "asdfghgfd", Data = "dfghjkl"},
            new Info { Id = 6, Name = "Ali", Description = "fghjkl;", Data = "Num"},
            new Info { Id = 1237, Name = "Hamza", Description = "asdfghgfdddddddd", Data = "dfghjkl"},
            new Info { Id = 0, Name = "Ali", Description = "fghjkl;", Data = "Num"}
        };

        public RedirectToActionResult AddListToDatabase()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var info in User)
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO List_Data (Id, Name, Description, Data) VALUES (@Id, @Name, @Description, @Data)", conn);
                    cmd.Parameters.AddWithValue("@Id", info.Id);
                    cmd.Parameters.AddWithValue("@Name", info.Name);
                    cmd.Parameters.AddWithValue("@Description", info.Description);
                    cmd.Parameters.AddWithValue("@Data", info.Data);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("GetList");
        }
        public IActionResult GetList()
        {
            List<Info> users = new List<Info>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_List", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    Info user = new Info();
                    user.Id = Convert.ToInt32(dr["Id"]);
                    user.Name = Convert.ToString(dr["Name"]);
                    user.Description = Convert.ToString(dr["Description"]);
                    user.Data = Convert.ToString(dr["Data"]);
                    users.Add(user);
                }
            }
            return View(users);
        }
        public RedirectToActionResult Update(Info model)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_List_Update", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", model.Name);
                cmd.Parameters.AddWithValue("@Description", model.Description);
                cmd.Parameters.AddWithValue("@Data", model.Data);
                cmd.Parameters.AddWithValue("@Id", model.Id);
                con.Open();
                int r = cmd.ExecuteNonQuery();
            }
            return RedirectToAction("GetList");
        }

        public IActionResult GetDetails(int Id)
        {
            Info model = new Info();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("Sp_ListDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataAdapter adapterr = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapterr.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    model.Name = Convert.ToString(dr["Name"]);
                    model.Description = Convert.ToString(dr["Description"]);
                    model.Data = Convert.ToString(dr["Data"]);
                }
            }
            return View(model);
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
