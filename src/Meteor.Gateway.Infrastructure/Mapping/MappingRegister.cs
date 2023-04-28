using Mapster;

namespace Meteor.Gateway.Infrastructure.Mapping;

public class MappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Grpc.Session, Core.Models.Session>()
            .Map(coreModel => coreModel.ExpireDate, grpcModel => grpcModel.ExpireDate.ToDateTimeOffset())
            .Map(coreModel => coreModel.LastRefreshDate, grpcModel => grpcModel.LastRefreshDate.ToDateTimeOffset())
            .Map(coreModel => coreModel.CreateDate, grpcModel => grpcModel.CreateDate.ToDateTimeOffset());
    }
}