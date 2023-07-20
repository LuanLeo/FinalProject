using System.Runtime.Serialization;
using System.Xml.Linq;

namespace TablesideOrdering.Areas.StoreOwner.StatisticModels
{
    public class FoodStatisticModel
    {
        public string FoodName { get; set; }
        public float Value { get; set; }
    }
    [DataContract]
    public class TopFoodLogicModel
    {
        public TopFoodLogicModel(string label, float y)
        {
            this.Label = label;
            this.Y = y;
        }

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "label")]
        public string Label = "";

        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<float> Y = null;
    }
}
