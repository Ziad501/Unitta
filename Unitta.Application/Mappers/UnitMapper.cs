using Riok.Mapperly.Abstractions;
using Unitta.Application.DTOs;
using Unitta.Domain.Entities;

namespace Unitta.Application.Mappers;

[Mapper]
public static partial class UnitMapper
{
    public static partial Unit ToUnit(this UnitCreateDto unitCreateDto);
    public static partial UnitDto ToUnitDto(this Unit unit);
    public static partial IEnumerable<UnitDto> ToUnitDtoList(this IEnumerable<Unit> units);
    // -- UPDATE METHODS --

    // Maps an existing entity to the update DTO (for pre-filling the edit form)
    public static partial UnitUpdateDto ToUnitUpdateDto(this Unit unit);

    // Maps the DTO onto an existing entity
    public static partial void UpdateUnitFromDto(this UnitUpdateDto dto, Unit unit);
}
