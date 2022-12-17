using CheckoutMerchant.Infrastructure;
using CheckoutMerchant.Models.Merchant;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutMerchant.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IMockBankAuth _mockBankAuth;
        private readonly IValidator<MerchantDetails> _validator;

        public MerchantController(
            IMerchantService merchantService,
            IMockBankAuth mockBankAuth,
            IValidator<MerchantDetails> validator)
        {
            _merchantService = merchantService;
            _mockBankAuth = mockBankAuth;
            _validator = validator;
        }

        [HttpPost]
        public async Task<ActionResult<string>> CreateMerchantAsync(MerchantDetails merchantDetails)
        {
            ValidationResult result = await _validator.ValidateAsync(merchantDetails);

            try
            {
                var accessToken = await _mockBankAuth.GenerateClientCredentialsAsync(merchantDetails);

                bool isMerchantCreated = await _merchantService.UpsertMerchantDetailsAsync(merchantDetails,
                    new Models.TokenInfo
                    {
                        AccessToken = accessToken.Token,
                        ExpiresIn = accessToken.ExpiresIn
                    });

                if (isMerchantCreated)
                    return Ok(accessToken.Token);
            }
            catch
            {
                return BadRequest();
            }

            return Unauthorized();
        }
    }
}
