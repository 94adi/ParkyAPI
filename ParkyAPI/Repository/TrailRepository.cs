using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public class TrailRepository : ITrailRepository
    {

        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateTrail(Trail trail)
        {
            trail.DateCreated = DateTime.Now;
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail Trail)
        {
            _db.Trails.Remove(Trail);
            return Save();
        }

        public Trail GetTrail(int TrailId)
        {
            return _db.Trails.Include(t => t.NationalPark).FirstOrDefault(a => a.Id == TrailId);

        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(t => t.NationalPark).OrderBy(a => a.Name).ToList();
        }

        public bool TrailExists(string name)
        {
            bool result = _db.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());

            return result;
        }

        public bool TrailExists(int id)
        {
            bool result = _db.Trails.Any(a => a.Id == id);

            return result;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTrail(Trail Trail)
        {
            _db.Trails.Update(Trail);
            return Save();
        }

        ICollection<Trail> ITrailRepository.GetTrailsInNationalPark(int nationalParkId)
        {
            return _db.Trails.Include(t => t.NationalPark).Where(t => t.NationalParkId == nationalParkId).ToList();
        }
    }
}
