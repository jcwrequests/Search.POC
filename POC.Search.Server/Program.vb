Imports System.Text
Imports System.Threading.Tasks
Imports System.Web.Http
Imports System.Web.Http.SelfHost
Imports Castle.Windsor
Imports Castle.Windsor.Installer

Module Program

    Sub Main()

        Dim container = New WindsorContainer()
        container.Install(FromAssembly.This())
        Dim publisher = container.Resolve(Of Domain.Services.DomainEventPublisher)()

        Dim config = New HttpSelfHostConfiguration("http://localhost")

        config.DependencyResolver = New WindsorDependencyResolver(container)

        config.Routes.MapHttpRoute("Food Routes",
                                   "api/utilities/search/foods/{cat}/{term}",
                                   New With {.controller = "foods",
                                             .cat = RouteParameter.Optional,
                                             .term = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Food Routes Actions",
                                   "api/utilities/search/foods/{action}/{food}",
                                   New With {.controller = "foods",
                                             .food = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Catergory Routes",
                                  "api/utilities/search/categories/{pcat}/{scat}",
                                  New With {.controller = "categories",
                                            .pcat = RouteParameter.Optional,
                                            .scat = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Category Action Routes",
                                   "api/utilities/search/categories/{action}/{category}",
                                   New With {.controller = "categories",
                                             .category = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Product Action",
                                   "api/utilities/search/products/{brand}/{name}/{id}",
                                   New With {.controller = "products",
                                             .brand = RouteParameter.Optional,
                                             .name = RouteParameter.Optional,
                                             .id = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Product Action Routes",
                                   "api/utilities/search/products/{action}/{product}",
                                   New With {.controller = "products",
                                             .product = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Brand Routes",
                                   "api/utilities/search/brands/{term}",
                                   New With {.controller = "brands",
                                             .term = RouteParameter.Optional})

        config.Routes.MapHttpRoute("Brand Action Routes",
                                   "api/utilities/search/brands/{action}/{brand}",
                                   New With {.controller = "brands",
                                             .brand = RouteParameter.Optional})

        Using server = New HttpSelfHostServer(config)
            publisher.Start()
            server.OpenAsync().Wait()

            Console.WriteLine("Press Enter to quit")
            Console.ReadLine()
            publisher.StopProcessingEvents()
        End Using



    End Sub

End Module
