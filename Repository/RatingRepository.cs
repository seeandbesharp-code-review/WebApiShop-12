using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repository
{
    public class RatingRepository: IRatingRepository
    {
        WebApiShopContext _webApiShopContext;
        public RatingRepository(WebApiShopContext webApiShopContext) 
        { 
            _webApiShopContext = webApiShopContext;
        }
        public async Task<Rating> AddRating(Rating rating)
        {
            _ = _webApiShopContext.Ratings.AddAsync(rating);
            await _webApiShopContext.SaveChangesAsync();
            return rating; 
        }
    }
}
