using TableDependency.SqlClient.Base.Exceptions;
using TablesideOrdering.Areas.Admin.Models;

namespace TablesideOrdering.Models.VNPay
{
    public class CustomerInformation
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string TableNo {  get; set; }
    }
}
