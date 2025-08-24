using Riok.Mapperly.Abstractions;
using Unitta.Application.DTOs;
using Unitta.Domain.Entities;

namespace Unitta.Application.Mappers;

[Mapper]
public static partial class BookingMapper
{
    public static partial BookingDto ToBookingDto(this Booking booking);
    public static partial IEnumerable<BookingDto> ToBookingDtos(this IEnumerable<Booking> bookings);
    public static partial Booking ToBooking(this BookingDto bookingDto);
    public static partial UserDto ToUserDto(this User user);
    public static partial User ToUser(this UserDto userDto);

}
