﻿namespace Application.DTOs.Password;

public class ChangePasswordRequestDTO
{
    public string CurrentPassword { get; set;} = string.Empty;
    
    public string NewPassword { get; set; } = string.Empty;
    
    public string ConfirmNewPassword { get; set; } = string.Empty; 
}