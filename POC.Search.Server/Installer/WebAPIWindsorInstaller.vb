Imports Castle.MicroKernel.Registration
Imports System.Net
Imports System.Web.Http
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports POC.Search.Domain.Storage
Imports POC.Search.Domain.Contracts
Imports POC.Search.Domain.Services
Imports Castle.Windsor
Imports POC.Search.Domain.Aggregates

Public Class WebAPIWindsorInstaller
    Implements IWindsorInstaller


    Public Sub Install(container As Castle.Windsor.IWindsorContainer, store As Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore) Implements IWindsorInstaller.Install
        container.
            Register(Component.
                     For(Of BrandsController).
                     DependsOn(Dependency.OnComponent(Of IApplicationService, BrandApplicationService)).
                     LifestyleScoped,
                     Component.For(Of CategoriesController).
                     DependsOn(Dependency.OnComponent(Of IApplicationService, CategoryApplicationService)).
                     LifestyleScoped,
                     Component.
                     For(Of FoodsController).
                     DependsOn(Dependency.OnComponent(Of IApplicationService, FoodApplicationService)).
                     LifestyleScoped,
                     Component.
                     For(Of ProductsController).
                     DependsOn(Dependency.OnComponent(Of IApplicationService, ProductApplicationService)).
                     LifestyleScoped,
                     Component.
                     For(Of IEventStore).
                     ImplementedBy(Of EventStore).
                     LifestyleSingleton,
                     Component.
                     For(Of POC.Search.Domain.Services.ILogger).
                     ImplementedBy(Of SimpleLogger).
                     LifestyleSingleton,
                     Component.
                     For(Of IAppendOnlyStore).
                     ImplementedBy(Of FileAppendOnlyStore).
                     DependsOn(Dependency.OnConfigValue("fileAppendOnlyStorePath", My.MySettings.Default.ViewPath)).
                     LifestyleSingleton,
                     Component.
                     For(Of IDocumentStrategy).
                     ImplementedBy(Of DocumentStrategy).
                     DependsOn(Dependency.OnConfigValue("projectionPath", My.MySettings.Default.ViewPath)).
                     LifestyleSingleton,
                     Component.
                     For(Of FileDocumentReaderWriter(Of brandLookUpunit, BrandLookUp)).
                     DependsOn(Dependency.OnConfigValue("fileDocumentReaderWriterDirectoryPath", My.MySettings.Default.ProjectionsPath),
                               Dependency.OnComponent(Of IDocumentStrategy, DocumentStrategy)).
                     LifestyleSingleton,
                     Component.
                     For(Of IDocumentWriter(Of brandLookUpunit, BrandLookUp)).
                     UsingFactoryMethod(Function(k)
                                            Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of brandLookUpunit, BrandLookUp))()
                                            Return CType(_store, IDocumentWriter(Of brandLookUpunit, BrandLookUp))
                                        End Function).
                     LifestyleSingleton,
                     Component.
                     For(Of IDocumentReader(Of brandLookUpunit, BrandLookUp)).
                     UsingFactoryMethod(Function(k)
                                            Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of brandLookUpunit, BrandLookUp))()
                                            Return CType(_store, IDocumentReader(Of brandLookUpunit, BrandLookUp))
                                        End Function).
                     LifestyleSingleton,
                     Component.
                     For(Of FileDocumentReaderWriter(Of unit, BrandTermLookUp)).
                     DependsOn(Dependency.OnConfigValue("fileDocumentReaderWriterDirectoryPath", My.MySettings.Default.ViewPath),
                               Dependency.OnComponent(Of IDocumentStrategy, DocumentStrategy)).
                     LifestyleSingleton,
                     Component.
                     For(Of IDocumentWriter(Of unit, BrandTermLookUp)).
                     UsingFactoryMethod(Function(k)
                                            Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, BrandTermLookUp))()
                                            Return CType(_store, IDocumentWriter(Of unit, BrandTermLookUp))
                                        End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IDocumentReader(Of unit, BrandTermLookUp)).
                    UsingFactoryMethod(Function(k)
                                           Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, BrandTermLookUp))()
                                           Return CType(_store, IDocumentReader(Of unit, BrandTermLookUp))
                                       End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IProjection).
                    ImplementedBy(Of BrandTermLookUpProjection).
                    Named("BrandTermLookUpProjection").
                    LifestyleSingleton,
                    Component.
                    For(Of IProjection).
                    ImplementedBy(Of BrandLookUpProjection).
                    Named("BrandLookUpProjection").
                    LifestyleSingleton,
                    Component.
                    For(Of FileDocumentReaderWriter(Of unit, CategoryLookUp)).
                    DependsOn(Dependency.OnConfigValue("fileDocumentReaderWriterDirectoryPath", My.MySettings.Default.ViewPath),
                              Dependency.OnComponent(Of IDocumentStrategy, DocumentStrategy)).
                    LifestyleSingleton,
                    Component.
                    For(Of IDocumentReader(Of unit, CategoryLookUp)).
                    UsingFactoryMethod(Function(k)
                                           Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, CategoryLookUp))()
                                           Return CType(_store, IDocumentReader(Of unit, CategoryLookUp))
                                       End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IDocumentWriter(Of unit, CategoryLookUp)).
                    UsingFactoryMethod(Function(k)
                                           Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, CategoryLookUp))()
                                           Return CType(_store, IDocumentWriter(Of unit, CategoryLookUp))
                                       End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IProjection).
                    ImplementedBy(Of CategoryLookUpProjection).
                    Named("CategoryLookUpProjection").
                    LifestyleSingleton,
                    Component.
                    For(Of FileDocumentReaderWriter(Of unit, FoodLookup)).
                    DependsOn(Dependency.OnConfigValue("fileDocumentReaderWriterDirectoryPath", My.MySettings.Default.ViewPath),
                              Dependency.OnComponent(Of IDocumentStrategy, DocumentStrategy)).
                    LifestyleSingleton,
                    Component.
                    For(Of IDocumentReader(Of unit, FoodLookup)).
                    UsingFactoryMethod(Function(k)
                                           Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, FoodLookUp))()
                                           Return CType(_store, IDocumentReader(Of unit, FoodLookUp))
                                       End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IDocumentWriter(Of unit, FoodLookup)).
                    UsingFactoryMethod(Function(k)
                                           Dim _store = k.Resolve(Of FileDocumentReaderWriter(Of unit, FoodLookUp))()
                                           Return CType(_store, IDocumentWriter(Of unit, FoodLookUp))
                                       End Function).
                    LifestyleSingleton,
                    Component.
                    For(Of IProjection).
                    ImplementedBy(Of FoodLookUpProjection).
                    Named("FoodLookUpProjection").
                    LifestyleSingleton,
                    Component.
                    For(Of DomainEventPublisher).
                    UsingFactoryMethod(Function(k)
                                           Return InitializePublisher(container)()
                                       End Function).
                    LifestyleSingleton,
                    Classes.
                    FromAssemblyContaining(Of IApplicationService).
                    BasedOn(Of IApplicationService).
                    WithServiceAllInterfaces().
                    LifestyleSingleton)




    End Sub

    Private Function InitializePublisher(_container As IWindsorContainer) As Func(Of DomainEventPublisher)
        Dim intializer =
            Function()
                Dim eventStore = _container.Resolve(Of IEventStore)()
                Dim domainLogger = _container.Resolve(Of ILogger)()
                Dim _currentIndexValue = My.MySettings.Default.CurrentIndexValue
                _currentIndexValue = -1

                Dim DomainEventPublisher =
                    New DomainEventPublisher(eventStore,
                                            currentIndexValue:=_currentIndexValue,
                                            log:=domainLogger,
                                            numberOfEventsForRebuild:=My.MySettings.Default.NoOfEventsForRebuild)

                Dim projections = _container.ResolveAll(Of IProjection)()

                For Each projection In projections
                    DomainEventPublisher.RegisterProjection(projection)
                Next

                AddHandler DomainEventPublisher.UpdateIndex, Sub(currentIndexValue As Integer)
                                                                 If My.Settings.CurrentIndexValue < currentIndexValue Then
                                                                     My.Settings.CurrentIndexValue = currentIndexValue
                                                                 End If

                                                                 Console.WriteLine(currentIndexValue)
                                                             End Sub

                Return DomainEventPublisher
            End Function
        Return intializer
    End Function

End Class
