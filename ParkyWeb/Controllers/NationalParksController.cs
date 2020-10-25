using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ParkyApi.Models;
using ParkyWeb.Helpers;
using ParkyWeb.Repository.RepositoryInterfaces;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;

        public NationalParksController(INationalParkRepository nationalParkRepository)
        {
            _nationalParkRepository = nationalParkRepository;
        }

        public IActionResult Index()
        {
            return View( new NationalPark() { });
        }

        public async Task<IActionResult> GetAllNationalParks()
        {
            return Json(new { data = await _nationalParkRepository.GetAllAsync(StaticData.NationalParkApiPath) });
        }
    }
}
