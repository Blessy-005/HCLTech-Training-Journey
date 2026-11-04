using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Authorization
{
    public class BookingAuthorizationHandler
        : AuthorizationHandler<BookingOwnerRequirement, Booking>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            BookingOwnerRequirement requirement,
            Booking booking)
        {
            // Admin can access every booking.
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Get the logged-in user's ID.
            var userId = context.User.FindFirst(
                ClaimTypes.NameIdentifier)?.Value;

            // Allow access only to the booking owner.
            if (!string.IsNullOrEmpty(userId) &&
                booking.UserId == userId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
