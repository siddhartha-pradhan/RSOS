﻿using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[Authorize]
[ApiController]
[Route("api/students")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }
    
    [HttpGet("get-student-responses/{studentId:int}")]
    public async Task<IActionResult> GetStudentResponses(int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
    
    [HttpPost("get-student-responses")]
    public async Task<IActionResult> GetStudentResponsesResult(int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    [HttpPost("insert-student-records")]
    public async Task<IActionResult> InsertStudentResponse(StudentRequestDTO studentResponse)
    {
        await _studentService.InsertStudentResponse(studentResponse.StudentResponse);

        await _studentService.InsertStudentScore(studentResponse.StudentScore);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }
}