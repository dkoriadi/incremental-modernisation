# Incremental Modernisation

This repository showcases incremental modernisation of a legacy ASP.NET MVC application to a modern ASP.NET Core MVC application through the [strangler fig pattern](https://learn.microsoft.com/en-us/azure/architecture/patterns/strangler-fig) approach.

## Table of contents
   * [Implementation](#Implementation)
   * [Acknowledgements](#Acknowledgements)

## Implementation

The approach for the incremental modernisation is outlined [here](https://learn.microsoft.com/en-us/aspnet/core/migration/inc/overview?view=aspnetcore-7.0#app-migration-to-aspnet-core), which allows both the legacy ASP.NET application and the modern ASP.NET Core MVC application to run side-by-side. 

As more features are ported over to the modernised application, both applications will eventually reach feature parity. After which, the legacy application may eventually be retired since all usage will be routed to the modernised application.  

### 01. NET Framework Upgrade

The legacy ASP.NET MVC application and its libraries are upgraded to the latest .NET Framework as the class libraries will eventually be targeting .NET Standard 2.0

### 02. Install YARP and System.Web.Adapters in modernised application

Create the modernised ASP.NET Core MVC application and install the libraries:
* [YARP](https://github.com/microsoft/reverse-proxy)
* [SystemWebAdapters](https://github.com/dotnet/systemweb-adapters)

YARP is a reverse proxy library that handles all incoming requests going to the modernised ASP.NET Core application, and forwards the request to the legacy ASP.NET application if there is no available handling of the request in the modernised application.

SystemWebAdapters allows business logic that are reliant on `System.Web` such as session variables to be interoperable by both the legacy ASP.NET application and the modernised ASP.NET Core application.

### 03. Upgrade class libraries to .NET Standard 2.0

Class libraries, including their dependencies, are targeted to .NET Standard 2.0 so that common business logic (such as data access layer) can be used by both applications.

### 04. Implement shared state across applications

In order for both applications to run side-by-side and transition seamlessly, both applications must be able to share [session state](https://learn.microsoft.com/en-us/aspnet/core/migration/inc/session) and [authentication](https://learn.microsoft.com/en-us/aspnet/core/migration/inc/remote-authentication).

Example: if the user is logged in via the legacy application, when the user browses a page on the modernised application, the user must also remain logged in.

### 05. Complete modernised application

Port the controllers and views incrementally from the legacy application until all features have been successfully implemented in the modernised application. As the process continues, more and more requests will be routed away from the legacy application into the modernised application.

Finally, the legacy application may be retired since it is no longer handling any incoming requests.

The last remaining step is to remove the two libraries installed above.

<p align="left">(<a href="#top">back to top</a>)</p>

## Acknowledgements

- [Microsoft Documentation](https://learn.microsoft.com/en-us/aspnet/core/migration/inc/overview)
- [dotnet Youtube Playlist - Migrating from ASP.NET to ASP.NET Core](https://www.youtube.com/playlist?list=PLdo4fOcmZ0oWiK8r9OkJM3MUUL7_bOT9z)
- [Migrate your Web Apps to ASP.NET Core, one page at a time](https://www.youtube.com/watch?v=xZ0_NwxDJxc), also check out the speaker's [implementation](https://www.youtube.com/watch?v=1DEXDKQguK8&list=PLS0ozk6DfmMSy2xv67eoCn12Pkw-K8orl) 

<p align="left">(<a href="#top">back to top</a>)</p>
