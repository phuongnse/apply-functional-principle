using ApplyFunctionalPrinciple.Logic.Common;

namespace ApplyFunctionalPrinciple.Logic.Model
{
    public class Industry : Entity
    {
        public const string CarsIndustry = "Cars";
        public const string PharmacyIndustry = "Pharmacy";

        public virtual string Name { get; protected set; }
    }
}