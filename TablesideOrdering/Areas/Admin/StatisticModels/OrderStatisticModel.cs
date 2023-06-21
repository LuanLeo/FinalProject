using AspNetCore;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace TablesideOrdering.Areas.Admin.StatisticModels
{
    public class OrderStatisticModel
    {
        public string Date { get; set; }
        public float Price { get; set; }
    }

    [DataContract]
    public class OrderLogicModel
    {
        public OrderLogicModel(string label, float y)
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
