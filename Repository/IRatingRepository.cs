using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Repository
{
    public interface IRatingRepository
    {
         Task<Rating> AddRating(Rating rating);
    }
}
