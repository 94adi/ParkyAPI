using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    public class NationalParksController : Controller
    {

        private readonly INationalParkRepository _nationalParkRepo;

        public NationalParksController(INationalParkRepository nationalParkRepo)
        {
            _nationalParkRepo = nationalParkRepo;
        }

        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark nationalParkObj = new NationalPark();
            if(id == null)
            {
                return View(nationalParkObj);
            }

            nationalParkObj = await _nationalParkRepo.GetAsync(StaticDetails.NationalParkAPIPath, id.GetValueOrDefault());

            if (nationalParkObj == null)
                return NotFound();

            return View(nationalParkObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark nationalPark)
        {
            var files2 = HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] picture = null;
                    using(var fileStream = files[0].OpenReadStream())
                    {
                        using(var memoryStream = new MemoryStream())
                        {
                            fileStream.CopyTo(memoryStream);
                            picture = memoryStream.ToArray();
                        }
                    }
                    nationalPark.Picture = picture;
                }
                else
                {
                    var nationalParkFromDB = await _nationalParkRepo.GetAsync(StaticDetails.NationalParkAPIPath, nationalPark.Id);
                    nationalPark.Picture = nationalParkFromDB.Picture;
                }

                if(nationalPark.Id == 0)
                {
                    await _nationalParkRepo.CreateAsync(StaticDetails.NationalParkAPIPath, nationalPark);
                }
                else
                {
                    await _nationalParkRepo.UpdateAsync(StaticDetails.NationalParkAPIPath + nationalPark.Id, nationalPark);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nationalPark);
        }


        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await _nationalParkRepo.GetAllAsync(StaticDetails.NationalParkAPIPath) });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _nationalParkRepo.DeleteAsync(StaticDetails.NationalParkAPIPath, id);
            if (deleteResult)
                return Json(new { success = true, message = "Delete Successful" });

            return Json(new { success = false, message = "Operation Failed" });
        }

    }
}
