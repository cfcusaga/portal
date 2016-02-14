using System;
using System.Collections.Generic;

namespace  Cfcusaga.Web.Configuration
{
    interface IConfiguration
    {
        String OrderEmail { get; set; }

        String EmailApiKey { get; set; }

        String DomainForApiKey { get; set; }

        String FromName { get; set; }

        String FromEmail { get; set; }

        List<String> CreditCardType { get; set; }

    }
}
