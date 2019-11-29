﻿using Esquio;
using Esquio.EntityFrameworkCore.Store;
using Esquio.EntityFrameworkCore.Store.Diagnostics;
using Esquio.EntityFrameworkCore.Store.Entities;
using Esquio.EntityFrameworkCore.Store.Options;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalTests.Esquio.EntityFramework.Store
{
    [Collection(nameof(CollectionExecutionFixture))]
    public class entityframeworkcore_store_should
    {
        private const string Users = nameof(Users);
        private readonly Fixture _fixture;

        public entityframeworkcore_store_should(Fixture fixture)
        {
            _fixture = fixture;
            _fixture.Options = Data.SelectMany(d => d.Select(o => (DbContextOptions<StoreDbContext>)o)).ToList();

            foreach (var options in _fixture.Options)
            {
                using (var context = new StoreDbContext(options, new StoreOptions()))
                {
                    context.Database.EnsureCreated();

                    AddData(context);
                }
            }
        }

        void AddData(StoreDbContext dbContext)
        {
            if (!dbContext.Products.Any())
            {
                var product = new ProductEntity("default", "description");
                dbContext.Products.Add(product);

                dbContext.SaveChanges();

                var appFeature = new FeatureEntity(product.Id, "app-feature", enabled: true);

                var toggle = new ToggleEntity(appFeature.Id, "Esquio.Toggles.XToggle");

                var stringparameter = new ParameterEntity(toggle.Id, "strparam", "value1");
                toggle.Parameters.Add(stringparameter);

                var intparameter = new ParameterEntity(toggle.Id, "intparam", "1");
                toggle.Parameters.Add(intparameter);

                appFeature.Toggles.Add(toggle);

                dbContext.Features.Add(appFeature);

                dbContext.SaveChanges();
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task get_null_when_product_not_exist(DbContextOptions<StoreDbContext> options)
        {
            var store = new EntityFrameworkCoreStoreBuilder(options)
                .Build();

            var expected = await store.FindFeatureAsync("non-existing-feature", "non-existing-product");

            expected.Should()
                .BeNull();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task get_null_when_feature_is_not_configured_on_product(DbContextOptions<StoreDbContext> options)
        {
            var store = new EntityFrameworkCoreStoreBuilder(options)
                .Build();

            var expected = await store.FindFeatureAsync("non-existing-feature", EsquioConstants.DEFAULT_PRODUCT_NAME);

            expected.Should()
                .BeNull();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task get_null_when_feature_exist_on_diferent_product(DbContextOptions<StoreDbContext> options)
        {
            var store = new EntityFrameworkCoreStoreBuilder(options)
                .Build();

            var expected = await store.FindFeatureAsync("app-feature", "non-existing-product");

            expected.Should()
                .BeNull();
        }

        [Theory]
        [MemberData(nameof(Data))]
        public async Task get_back_feature_when_is_configured(DbContextOptions<StoreDbContext> options)
        {
            var store = new EntityFrameworkCoreStoreBuilder(options)
                .Build();

            var expected = await store.FindFeatureAsync("app-feature", EsquioConstants.DEFAULT_PRODUCT_NAME);

            expected.Should()
                .NotBeNull();

            expected.GetToggles()
                .Count().Should().Be(1);

            expected.GetToggles()
                .First()
                .GetParameters()
                .Count().Should().Be(2);

            expected.GetToggles()
                .First()
                .GetParameters()
                .First()
                .Name.Should().Be("strparam");
            expected.GetToggles()
                .First()
                .GetParameters()
                .First()
                .Value.Should().Be("value1");
        }

        public static TheoryData<DbContextOptions<StoreDbContext>> Data =>
            new TheoryData<DbContextOptions<StoreDbContext>>
            {
                DatabaseProviderBuilder.BuildSqlServer<StoreDbContext>(nameof(entityframeworkcore_store_should)),
            };
    }

    class EntityFrameworkCoreStoreBuilder
    {
        private readonly DbContextOptions<StoreDbContext> _options;

        public EntityFrameworkCoreStoreBuilder(DbContextOptions<StoreDbContext> options)
        {
            _options = options;
        }

        public EntityFrameworkCoreFeaturesStore Build()
        {
            var loggerFactory = new LoggerFactory();
            var diagnostics = new EsquioEntityFrameworkCoreStoreDiagnostics(loggerFactory);

            var dbContext = new StoreDbContext(_options, new StoreOptions());
            var store = new EntityFrameworkCoreFeaturesStore(dbContext, diagnostics);

            return store;
        }
    }
}
