using Riok.Mapperly.Abstractions;
using Unitta.Application.DTOs;
using Unitta.Domain.Entities;

namespace Unitta.Application.Mappers;

[Mapper]
public static partial class UnitNoMapper
{
    public static partial UnitNo ToUnitNo(this UnitNoCreateDto unitNoCreateDto);
    public static partial UnitNoDto ToUnitNoDto(this UnitNo unitNo);
    public static partial IEnumerable<UnitNoDto> ToUnitNoDtoList(this IEnumerable<UnitNo> unitNos);
    public static partial UnitNoUpdateDto ToUnitNoUpdateDto(this UnitNo unitNo);
    public static partial void UpdateUnitNoFromDto(this UnitNoUpdateDto dto, UnitNo unitNo);

}
