using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb2.Helpers;
using ParkyWeb2.Models;
using ParkyWeb2.Repository.IRepository;

namespace ParkyWeb2.Controllers
{
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository _nationalParkRepository;
        private readonly ITrailRepository _trailRepository;

        public TrailsController(INationalParkRepository nationalParkRepository, ITrailRepository trailRepository)
        {
            _nationalParkRepository = nationalParkRepository;
            _trailRepository = trailRepository;
        }
        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> nationalParkList = await _nationalParkRepository.GetAllAsync(SD.NationalParkApiPath);

            TrailsVM trailsVM = new TrailsVM()
            {
                NationalParkList = nationalParkList.Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Trail = new Trail()
            };

            if(id == null)
            {
                // insert/create
                return View(trailsVM);
            }

            trailsVM.Trail = await _trailRepository.GetAsync(SD.TrailAPIPAth, id.GetValueOrDefault());

            if(trailsVM.Trail == null)
            {
                return NotFound();
            }

            return View(trailsVM);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM trailReturned)
        {
            if (ModelState.IsValid)
            {
                              

                if(trailReturned.Trail.Id== 0)
                {
                    await _trailRepository.CreateAsync(SD.NationalParkApiPath, trailReturned.Trail);                    
                }
                else
                {
                    await _trailRepository.UpdateAsync(SD.NationalParkApiPath+trailReturned.Trail.Id, trailReturned.Trail);
                }
                return RedirectToAction(nameof(Index));
            }

          
            return View(trailReturned);
        }
        

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await _trailRepository.DeleteAsync(SD.TrailAPIPAth, id);

            if (status)
            {
                return Json(new { Success = true, Message = "Delete Successful"});
            }
            return Json(new { Success = false, Message = "Delete Not Successful" });
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await _trailRepository.GetAllAsync(SD.TrailAPIPAth) });
        }
    }
}
