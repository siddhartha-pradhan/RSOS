﻿using Application.DTOs.Authentication;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Data.Implementation.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IGenericRepository _genericRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationService(IGenericRepository genericRepository, IConfiguration configuration)
    {
        _genericRepository = genericRepository;
        _configuration = configuration;
    }

    public async Task<AuthenticationResponseDTO> Authenticate(AuthenticationRequestDTO authenticationRequest)
    {
        var httpClient = new HttpClient();

        var rsosToken = _configuration["RSOS_Token"];

        var rsosUrl = _configuration["RSOS_URL"];

        var apiUrl = $"{rsosUrl}api_student_login?ssoid={authenticationRequest.SSOID}&dob={authenticationRequest.DateOfBirth}&token={rsosToken}";

        string baseUrl = $"{rsosUrl}api_student_login";

        // Query parameters
        var queryParams = new System.Collections.Specialized.NameValueCollection
        {
            { "ssoid", authenticationRequest.SSOID },
            { "dob", authenticationRequest.DateOfBirth },
            { "token", rsosToken }
        };

        // Construct the full URL with query parameters
        var uriBuilder = new UriBuilder(baseUrl);
        uriBuilder.Query = string.Join("&", Array.ConvertAll(queryParams.AllKeys,
                                        key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(queryParams[key])}"));

        // Content for POST request
        var postData = new StringContent("{\"key\": \"value\"}", Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(uriBuilder.Uri, postData);

        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<LoginResponseDTO>(responseData);

            if (apiResponse.Status)
            {
                var studentloginData = apiResponse.Data.Student;

                var studentEntity = new tblStudentLoginDetail
                {
                    LoginTime = DateTime.Now,
                    SSOID = studentloginData.SsoId,
                    DeviceRegistrationToken = authenticationRequest.DeviceRegistrationToken
                };

                await _genericRepository.InsertAsync<tblStudentLoginDetail>(studentEntity);


                var authenticationResponse = new AuthenticationResponseDTO
                {
                    Id = studentloginData.Id,
                    Enrollment = studentloginData.Enrollment,
                    Name = studentloginData.Name,
                    DateOfBirth = studentloginData.Dob,
                    SSOID = studentloginData.SsoId,
                    JWT = GenerateJwtToken(studentloginData)
                };

                return authenticationResponse;
            }
            else
            {
                return new AuthenticationResponseDTO();
            }
        } else
        {
            return new AuthenticationResponseDTO();
        }
    }

    private string GenerateJwtToken(StudentInfoDTO studentInfo)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Key"]);

        var issuer = _configuration["JWT:Issuer"];

        var audience = _configuration["JWT:Audience"];

        var durationInDays = Convert.ToInt32(_configuration["JWT:DurationInDays"]);

        var authClaims = new List<Claim>
        {
            new("studentid", studentInfo.Id.ToString()),
            new("enrollment", studentInfo.Enrollment.ToString()),
            new("ssoid", studentInfo.SsoId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var symmetricSigningKey = new SymmetricSecurityKey(key);

        var signingCredentials = new SigningCredentials(symmetricSigningKey, SecurityAlgorithms.HmacSha256);

        var expirationTime = DateTime.UtcNow.AddDays(durationInDays);

        var accessToken = new JwtSecurityToken(
            issuer,
            audience,
            claims: authClaims,
            signingCredentials: signingCredentials,
            expires: expirationTime
        );

        var token = new JwtSecurityTokenHandler().WriteToken(accessToken);

        return token;
    }
}