﻿using BusinessLogic.Services.Interfaces;
using DataAccess.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;


        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        //[Authorize(Roles = "STAFF")]
        [HttpGet("get-schedule-for-current-user-and-class-id")]
        public async Task<IActionResult> GetScheduleForCurrentUserAndClassId([FromQuery] long classId)
        {
            string personId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(personId))
                return BadRequest(new ApiFormatResponse(StatusCodes.Status404NotFound, false, "Login pls"));
            var result =await _scheduleService.GetScheduleForCurrentUserAndClassId(long.Parse(personId),classId).ConfigureAwait(false);
            if (result == null) return NotFound(new ApiFormatResponse(StatusCodes.Status404NotFound, false, result));
            return Ok(new ApiFormatResponse(StatusCodes.Status200OK, true, result));
        }

        [HttpGet("get-filter-schedule")]
        public async Task<IActionResult> GetFilterSchedule([FromQuery] string from, [FromQuery] string to)
        {
            try
            {
                TimeSpan fromTime;
                TimeSpan toTime;

                if (!TimeSpan.TryParse(from, out fromTime))
                {
                    return BadRequest("Invalid 'from' time format. The correct format is 'hh:mm:ss'.");
                }

                if (!TimeSpan.TryParse(to, out toTime))
                {
                    return BadRequest("Invalid 'to' time format. The correct format is 'hh:mm:ss'.");
                }
                var result = await _scheduleService.FilterScheduleFromTo(fromTime, toTime).ConfigureAwait(false);

                if (result == null) return NotFound(new ApiFormatResponse(StatusCodes.Status404NotFound, false, result));
                return Ok(new ApiFormatResponse(StatusCodes.Status200OK, true, result));
            }
            catch (Exception e)
            {
                if (e is ArgumentException)
                {
                    return BadRequest(e.Message);
                }

                throw;
            }
        }

    }
}
