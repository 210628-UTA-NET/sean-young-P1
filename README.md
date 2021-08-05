
# Million Dollar General WebApp


App is live on Azure Web Services at this [link](https://milliondollargeneral.azurewebsites.net)

## Overview
Million Dollar General is a fake convenience store chain that sells obscenely priced items instead of cheap ones. This webapp was produced for learning purposes to learn ASP.NET Core MVC and other technologies (details in "Tech Stack") presented during our training. Despite only writing a little Javascript, I was able to produce a fairly responsive UI utilizing [Bootstrap](https://getbootstrap.com/) components and server-side rendering using Razor pages. Additionally, I put great effort into making the app look as close to a real store webpage as possible with the limited time that I had. 

Users can:
 - Create a user account or login through a Google account
 - Search for and select a storefront to shop from
 - Search and browse the inventory of a selected storefront
 - Add items to a cart and manage the contents of their cart
 - Place an order for the items in the cart
 - View their order history

Managers can:
 - View and search the user accounts
 - View the order history of a storefront
 - Manage the quantity of items at a storefront

## Tech Stack
 - **Front End:** [Bootstrap 5.1.0](https://getbootstrap.com/)
 - **Backend:** [ASP.NET Core 5.0](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0)
 - **Database**: [MsSqlServer](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
 - **Testing**: [XUnit 2.4.1](https://xunit.net/), [Moq 4.16.1](https://www.nuget.org/packages/Moq/)
 - **CI/CD**: [GithubActions](https://github.com/features/actions)

## Deployment
Requires .Net 5.0.

To run the application: ``dotnet run ``


## Authors
**Sean Young** - syoung908

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
