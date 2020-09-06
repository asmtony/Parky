using Microsoft.EntityFrameworkCore;
using ParkyApi.Data;
using ParkyApi.Models;
using ParkyApi.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyApi.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext dbContext)
        {
            _db = dbContext;
        }
        public bool CreateTrail(Trail trail)
        {
            trail.DateCreated = DateTime.Now;
            _db.Trails.Add(trail);
            return Save();
        }

        public bool DeleteTrail(Trail trail)
        {
            _db.Trails.Remove(trail);
            return Save();
        }

        public Trail GetTrail(int trailId)
        {
            var trail = _db.Trails
                //.FirstOrDefault(a => a.Id == TrailId)
                .Include(trail => trail.NationalPark)
                .Where(nationalPark => nationalPark.Id == trailId)
                .OrderBy(a => a.Name).ToList();

            return _db.Trails.FirstOrDefault(a => a.Id == trailId);
        }

        public ICollection<Trail> GetTrails()
        {
            var trails = _db.Trails
                .Include(trail => trail.NationalPark)
                .OrderBy(a => a.Name).ToList();
            return trails;
        }

        public bool TrailExists(string name)
        {
            bool value = _db.Trails.Any(a => a.Name.ToLower().Trim() == name.ToLower().Trim());
            return value;
        }

        public bool TrailExists(int id)
        {
            bool value = _db.Trails.Any(a => a.Id== id);
            return value;
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTrail(Trail trail)
        {
            _db.Trails.Update(trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int nationalParkId)
        {
            return _db.Trails.Include(t => t.NationalPark).Where(t => t.NationalParkId == nationalParkId).ToList();
        }
    }
}
