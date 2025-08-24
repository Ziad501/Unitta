using Riok.Mapperly.Abstractions;
using Unitta.Application.DTOs;
using Unitta.Domain.Entities;

namespace Unitta.Application.Mappers;

[Mapper]
public static partial class UnitMapper
{
    [MapperIgnoreSource(nameof(UnitCreateDto.Image))]
    public static partial Unit ToUnit(this UnitCreateDto unitCreateDto);
    public static partial UnitDto ToUnitDto(this Unit unit);
    public static partial IEnumerable<UnitDto> ToUnitDtoList(this IEnumerable<Unit> units);

    [MapProperty(nameof(Unit.ImageUrl), nameof(UnitUpdateDto.CurrentImageUrl))]
    public static partial UnitUpdateDto ToUnitUpdateDto(this Unit unit);

    [MapperIgnoreSource(nameof(UnitUpdateDto.CurrentImageUrl))]
    [MapperIgnoreSource(nameof(UnitUpdateDto.NewImage))]
    public static partial void UpdateUnitFromDto(this UnitUpdateDto dto, Unit unit);
}
