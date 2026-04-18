using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository;

namespace Services
{
    public class RatingService: IRatingService
    {
        readonly IRatingRepository _ratingRepository;
        public RatingService(IRatingRepository ratingRepository) 
        {
            _ratingRepository = ratingRepository;
        }
        public async Task<Rating> AddRating(Rating rating)
        {
            return await _ratingRepository.AddRating(rating);
        }
    }
}
