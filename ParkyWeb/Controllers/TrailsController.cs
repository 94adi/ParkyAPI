using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ParkyWeb.Controllers
{
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

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(StaticDetails.NationalParkAPIPath);
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

            trailObj.Trail = await _trailRepository.GetAsync(StaticDetails.TrialAPIPath, id.GetValueOrDefault());

            if (trailObj.Trail == null)
                return NotFound();

            return View(trailObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailsVM trailVM)
        {
            if (ModelState.IsValid)
            {
                if(trailVM.Trail.Id == 0)
                {
                    await _trailRepository.CreateAsync(StaticDetails.TrialAPIPath, trailVM.Trail);
                }
                else
                {
                    await _trailRepository.UpdateAsync(StaticDetails.TrialAPIPath + trailVM.Trail.Id, trailVM.Trail);
                }
                return RedirectToAction(nameof(Index));
            }
            IEnumerable<NationalPark> npList = await _nationalParkRepository.GetAllAsync(StaticDetails.NationalParkAPIPath);
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
            var result = Json(new { data = await _trailRepository.GetAllAsync(StaticDetails.TrialAPIPath) });
            return result;
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _trailRepository.DeleteAsync(StaticDetails.TrialAPIPath, id);
            if (deleteResult)
                return Json(new { success = true, message = "Delete Successful" });

            return Json(new { success = false, message = "Operation Failed" });
        }

    }
}
