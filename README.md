# [Enterspeed Delivery .NET SDK](https://www.enterspeed.com/) &middot; [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE) [![NuGet version](https://img.shields.io/nuget/v/Enterspeed.Delivery.Sdk)](https://www.nuget.org/packages/Enterspeed.Delivery.Sdk/) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/enterspeedhq/enterspeed-sdk-delivery-dotnet/pulls) [![Twitter](https://img.shields.io/twitter/follow/enterspeedhq?style=social)](https://twitter.com/enterspeedhq)

## Installation

With .NET CLI

```bash
dotnet add package Enterspeed.Delivery.Sdk --version <version>
```

With Package Manager

```bash
Install-Package Enterspeed.Delivery.Sdk -Version <version>
```
## How to use

### Register services
Service has to be added to the service collection. This can be one by using the following extension method.
```c#
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddEnterspeedDeliveryService())
    .Build();
```
### Examples of usage
Example of a common implementation where the delivery service is being utilized.
```c#
using Enterspeed.Delivery.Sdk.Api.Models;
using Enterspeed.Delivery.Sdk.Api.Services;

namespace DeliverySdkStronglyTypeTest; 

public class TestService
{
    private readonly IEnterspeedDeliveryService _enterspeedDeliveryService;
    
    public TestService(IEnterspeedDeliveryService enterspeedDeliveryService)
    {
        _enterspeedDeliveryService = enterspeedDeliveryService;
    }

    // Handle that has been setup to return a view in Enterspeed.
    public async  Task<DeliveryApiResponse> WithHandle()
    {
        var response = await _enterspeedDeliveryService.Fetch("environment-******-****-****-****-**********", builder => builder.WithHandle("navigation"));
        return response;
    }

    // Example method that calls a Url route in Enterspeed.
    public async Task<DeliveryApiResponse> WithUrl()
    {
        var response = await _enterspeedDeliveryService.Fetch("environment-******-****-****-****-**********", builder => builder.WithUrl("http://localhost:3000/"));
        return response;
    }
    
    // Example method that calls a fully qualified url in Enterspeed. This is typically used in cojunction with a webhook from Enterspeed.
    // Weebhook typically returns an Delivery Api Url. This means that we do not need to construct a Delivery Api Url in code.  
    public async Task<DeliveryApiResponse> WithDeliveryApiUrl()
    {
        var response = await _enterspeedDeliveryService.Fetch("environment-******-****-****-****-**********", builder => builder.WithDeliveryApiUrl("absolute url returned from delivery api"));
        return response;
    }
    
    // Example of how to fetch many view in one request.
    public async Task<DeliveryApiResponse> WithDeliveryApiUrl()
    {
        var response = await _enterspeedDeliveryService.FetchMany("environment-******-****-****-****-**********", 
                                                                        new GetByIdsOrHandle { Handles = new List<string> { "R7034112", "R7034108" }, Ids = new List<string> { "id1", "id2" } });
        return response;
    }
    
    // Example of how to fetch strongly typed views.
    public async Task WithHandlesAsStronglyTyped()
    {
        var typedResponse  = await _enterspeedDeliveryService.FetchTyped("environment-******-****-****-****-**********", builder => builder
                                                                                                                                        .WithHandle("navigation")
                                                                                                                                        .WithHandle("footer"));

        NavigationModel navigation = typedResult.Response.Views["navigation"].GetContent<NavigationModel>();
        FooterModel navigation = typedResult.Response.Views["footer"].GetContent<FooterModel>();

        ...
    }

    public class NavigationModel
    {
        public MuenuItem[] MenuItems { get; set; }
        ...
    }

    public class FooterModel
    {
        [JsonPropertyName("footerLinks")]
        public Link[] Links { get; set; }
        public string CopyRightText { get; set; }
        ...
    }
}
```




## Contributing

Pull requests are very welcome.  
Please fork this repository and make a PR when you are ready.  

Otherwise you are welcome to open an Issue in our [issue tracker](https://github.com/enterspeedhq/enterspeed-sdk-delivery-dotnet/issues).

## License

Enterspeed .NET SDK is [MIT licensed](./LICENSE)
