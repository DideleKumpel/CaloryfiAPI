using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caloryfi.Model.DTO
{
    public class MealComponentDTO
    {
        public int MealId { get; set; }
        public int IngredientId { get; set; }

        public double Weight { get; set; }

    }
}
