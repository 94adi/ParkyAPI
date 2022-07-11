using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {

        private readonly ITrailRepository _trailRepository;
        private readonly INationalParkRepository _nationalParkRepository;

        public TrailsController(ITrailRepository trailRepository, INationalParkRepository nationalParkRepository)
        {
            _trailRepository = trailRepository;
            _nationalParkRepository = nationalParkRepository;
        }

        public IActionResult Index()
        {
            return View(new Trail() { });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(StaticDetails.NationalParkAPIPath, token);
            TrailsVM trailObj = new TrailsVM();
            trailObj.Trail = new Trail();

            trailObj.NationalParkList = npList.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            if (id == null)
            {
                return View(trailObj);
            }

            trailObj.Trail = await _trailRepository.GetAsync(StaticDetails.TrialAPIPath, id.GetValueOrDefault(), token);

            if (trailObj.Trail == null)
                return NotFound();

            return View(trailObj);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM trailVM)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (ModelState.IsValid)
            {
                if (trailVM.Trail.Id == 0)
                {
                    await _trailRepository.CreateAsync(StaticDetails.TrialAPIPath, trailVM.Trail, token);
                }
                else
                {
                    await _trailRepository.UpdateAsync(StaticDetails.TrialAPIPath + trailVM.Trail.Id, trailVM.Trail, token);
                }
                return RedirectToAction(nameof(Index));
            }
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(StaticDetails.NationalParkAPIPath, token);
            TrailsVM trailObj = new TrailsVM();
            trailObj.Trail = trailVM.Trail;

            trailObj.NationalParkList = npList.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });

            return View(trailObj);
        }


        public async Task<IActionResult> GetAllTrail()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var result = Json(new { data = await _trailRepository.GetAllAsync(StaticDetails.TrialAPIPath, token) });
            return result;
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var deleteResult = await _trailRepository.DeleteAsync(StaticDetails.TrialAPIPath, id, token);
            if (deleteResult)
                return Json(new { success = true, message = "Delete Successful" });

            return Json(new { success = false, message = "Operation Failed" });
        }

    }
}
