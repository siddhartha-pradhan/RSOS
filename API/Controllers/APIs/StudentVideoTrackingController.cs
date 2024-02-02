﻿using Application.DTOs.Base;
using Application.DTOs.Student;
using Application.DTOs.StudentVideoTracking;
using Application.Interfaces.Services;
using Data.Implementation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[Route("api/student-video-tracking")]
[ApiController]
public class StudentVideoTrackingController : ControllerBase
{
    private readonly IStudentVideoTrackingService _studentVideoTrackingService;

    public StudentVideoTrackingController(IStudentVideoTrackingService studentVideoTracking)
    {
        _studentVideoTrackingService = studentVideoTracking;
    }

    [HttpPost("insert-student-video-tracking")]
    public async Task<IActionResult> InsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.InsertStudentVideoTracking(studentVideoTrackingRequest);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [HttpGet("get-student-video-tracking/{studentId:int}")]
    public async Task<IActionResult> GetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-student-video-tracking")]
    public async Task<IActionResult> PostGetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPut("update-student-video-tracking")]
    public async Task<IActionResult> UpdateStudentVideoTracking(StudentVideoTrackingResponseDTO studentVideoTrackingResponse)
    {
        await _studentVideoTrackingService.UpdateStudentVideoTracking(studentVideoTrackingResponse);

        var response = new ResponseDTO<object>
        {
            Status = "Success",
            Message = "Successfully Updated",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }
}
