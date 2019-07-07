using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ztbiyesheji.EntityFramework;
using ztbiyesheji.Models;
namespace ztbiyesheji.Controllers
{
    public class LunboController : Controller
    {
        private readonly DemoDbContext _context;
        public LunboController(DemoDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetClass()
        {
            List<RotationImage> ClassList = await _context.RotationImage.ToListAsync();
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
            return Json(ClassList, settings);
        }
    }
}