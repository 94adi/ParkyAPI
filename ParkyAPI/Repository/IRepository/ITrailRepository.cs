using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrails();

        Trail GetTrail(int TrailId);

        bool TrailExists(string name);

        bool TrailExists(int id);

        bool CreateTrail(Trail Trail);

        bool UpdateTrail(Trail Trail);

        bool DeleteTrail(Trail Trail);

        bool Save();

        ICollection<Trail> GetTrailsInNationalPark(int nationalParkId);
    }
}
