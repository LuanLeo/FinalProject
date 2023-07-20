using TablesideOrdering.Models.Order;
using TablesideOrdering.Models.Momo;


namespace TablesideOrdering.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
