using AutoMapper;
using ScwSvc.Models;
using ScwSvc.SvcModels;

namespace ScwSvc;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<CreateDataSet, Table>();
        CreateMap<CreateSheet, Table>();

        CreateMap<ColumnDefinition, DataSetColumn>();
    }
}
