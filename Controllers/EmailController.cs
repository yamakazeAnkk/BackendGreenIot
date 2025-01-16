using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GreenIotApi.Services;
using GreenIotApi.Models;
using GreenIotApi.DTOs;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public EmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto emailDto)
        {
            if (string.IsNullOrEmpty(emailDto.To) || string.IsNullOrEmpty(emailDto.Subject) || string.IsNullOrEmpty(emailDto.Body))
            {
                return BadRequest("Email details are incomplete.");
            }

            await _emailService.SendEmailAsync(emailDto.To, emailDto.Subject, emailDto.Body);
            return Ok("Email sent successfully.");
        }
    }
}