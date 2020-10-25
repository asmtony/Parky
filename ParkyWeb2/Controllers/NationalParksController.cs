using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb2.Helpers;
using ParkyWeb2.Models;
using ParkyWeb2.Repository.IRepository;

namespace ParkyWeb2.Controllers
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
            return View(new NationalPark() { });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();

            if(id == null)
            {
                return View(obj);
            }
           
            obj = await _nationalParkRepository.GetAsync(SD.NationalParkApiPath, id.GetValueOrDefault());

            if(obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        
        [HttpPost]
        public async Task<IActionResult> Upsert(NationalPark nationalParkReturned)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                        nationalParkReturned.Picture = p1;
                        
                    }
                }
                else
                {
                    if (nationalParkReturned.Id > 0)
                    {
                        var objFromDb = await _nationalParkRepository.GetAsync(SD.NationalParkApiPath, nationalParkReturned.Id);
                        nationalParkReturned.Picture = objFromDb.Picture;
                    }
                }

                if(nationalParkReturned.Id== 0)
                {
                    await _nationalParkRepository.CreateAsync(SD.NationalParkApiPath, nationalParkReturned);                    
                }
                else
                {
                    await _nationalParkRepository.UpdateAsync(SD.NationalParkApiPath+nationalParkReturned.Id, nationalParkReturned);
                }
                return RedirectToAction(nameof(Index));
            }

          
            return View(nationalParkReturned);
        }
        
        public async Task<IActionResult> GetAllNationalParks()
        {
            return Json(new { data = await _nationalParkRepository.GetAllAsync(SD.NationalParkApiPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _nationalParkRepository.DeleteAsync(SD.NationalParkApiPath, id);

            if (status)
            {
                return Json(new { Success = true, Message = "Delete Successful"});
            }
            return Json(new { Success = false, Message = "Delete Not Successful" });
        }
    }
}
