// See https://aka.ms/new-console-template for more information

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Data;

Console.WriteLine("Hello, World!");
using (var db = new MixyBoosContext())
{
    var reporter = new OperationReporter(
        new OperationReportHandler(
            m => Console.WriteLine("  error: " + m),
            m => Console.WriteLine("   warn: " + m),
            m => Console.WriteLine("   info: " + m),
            m => Console.WriteLine("verbose: " + m)));

    var designTimeServices = new ServiceCollection()
        .AddSingleton(db.GetService<IHistoryRepository>())
        .AddSingleton(db.GetService<IMigrationsIdGenerator>())
        .AddSingleton(db.GetService<IMigrationsModelDiffer>())
        .AddSingleton(db.GetService<IMigrationsAssembly>())
        .AddSingleton(db.Model)
        .AddSingleton(db.GetService<ICurrentDbContext>())
        .AddSingleton(db.GetService<IDatabaseProvider>())
        .AddSingleton<MigrationsCodeGeneratorDependencies>()
        .AddSingleton<ICSharpHelper, CSharpHelper>()
        .AddSingleton<CSharpMigrationOperationGeneratorDependencies>()
        .AddSingleton<ICSharpMigrationOperationGenerator, CSharpMigrationOperationGenerator>()
        .AddSingleton<CSharpSnapshotGeneratorDependencies>()
        .AddSingleton<ICSharpSnapshotGenerator, CSharpSnapshotGenerator>()
        .AddSingleton<CSharpMigrationsGeneratorDependencies>()
        .AddSingleton<IMigrationsCodeGenerator, CSharpMigrationsGenerator>()
        .AddSingleton<IOperationReporter>(reporter)
        .AddSingleton<MigrationsScaffolderDependencies>()
        .AddSingleton<MigrationsScaffolder>()
        .BuildServiceProvider();

    var scaffolder = designTimeServices.GetRequiredService<MigrationsScaffolder>();

    var migration = scaffolder.ScaffoldMigration(
        "MyMigration",
        "MyApp.Data");

    File.WriteAllText(
        migration.MigrationId + migration.FileExtension,
        migration.MigrationCode);
    File.WriteAllText(
        migration.MigrationId + ".Designer" + migration.FileExtension,
        migration.MetadataCode);
    File.WriteAllText(migration.SnapshotName + migration.FileExtension,
        migration.SnapshotCode);
}
