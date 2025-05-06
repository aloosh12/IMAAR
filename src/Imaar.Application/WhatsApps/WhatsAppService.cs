using Imaar.MobileResponses;
using Imaar.UserProfiles;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Imaar.WhatsApps
{
    public abstract class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WhatsAppService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<MobileResponseDto> SendSecurityCodeAsync(string phone)
        {
            Random random = new Random();
            int securityNum = random.Next(1000, 10000);

            MobileResponseDto mobileResponse = new MobileResponseDto();
            var apiKey = _configuration["CallMeBot:ApiKey"];
            var url = $"https://api.callmebot.com/whatsapp.php?phone={phone}&text={Uri.EscapeDataString(securityNum.ToString())}&apikey={apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send WhatsApp message.");
                mobileResponse.Code = 501;
                mobileResponse.Message = "Failed to send WhatsApp message.";
                mobileResponse.Data = null;
                return mobileResponse;
            }
            mobileResponse.Code = 200;
            mobileResponse.Message = "Success";
            mobileResponse.Data = securityNum.ToString();
            return mobileResponse;
        }

        public async Task<MobileResponseDto> SendMessageAsync(WhatsAppMessageDto input)
        {
            MobileResponseDto mobileResponse = new MobileResponseDto();
            var apiKey = _configuration["CallMeBot:ApiKey"];
            var url = $"https://api.callmebot.com/whatsapp.php?phone={input.PhoneNumber}&text={Uri.EscapeDataString(input.Message)}&apikey={apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send WhatsApp message.");
                mobileResponse.Code = 501;
                mobileResponse.Message = "Failed to send WhatsApp message.";
                mobileResponse.Data = null;
                return mobileResponse;
            }
            mobileResponse.Code = 200;
            mobileResponse.Message = "Success";
            mobileResponse.Data = input.Message;
            return mobileResponse;
        }
    }
}
