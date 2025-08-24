using Riok.Mapperly.Abstractions;
using Unitta.Application.DTOs;
using Unitta.Domain.Entities;

namespace Unitta.Application.Mappers;

[Mapper]
public static partial class FeatureMapper
{
    public static partial Feature ToFeature(this FeatureCreateDto createDto);

    [MapProperty(nameof(Feature.Unit.Name), nameof(FeatureDto.UnitName))]
    public static partial FeatureDto ToFeatureDto(this Feature feature);

    public static partial IEnumerable<FeatureDto> ToFeatureDtoList(this IEnumerable<Feature> features);


    public static partial FeatureUpdateDto ToUpdateFeatureDto(this Feature feature);

    [MapperIgnoreTarget(nameof(Feature.Id))]
    public static partial void UpdateFeatureFromDto(this FeatureUpdateDto dto, Feature feature);
}