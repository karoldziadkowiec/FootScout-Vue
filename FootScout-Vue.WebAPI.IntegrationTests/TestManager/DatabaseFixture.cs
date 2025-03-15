using FootScout_Vue.WebAPI.DbManager;

namespace FootScout_Vue.WebAPI.IntegrationTests.TestManager
{
    public class DatabaseFixture : IDisposable
    {
        public AppDbContext DbContext { get; private set; }
        public string UserTokenJWT { get; private set; }
        public string AdminTokenJWT { get; private set; }

        public DatabaseFixture()
        {
            var dbName = $"FootScoutTests_{Guid.NewGuid()}";
            var options = TestBase.GetDbContextOptions($"Server=.; Database={dbName}; Integrated Security=true; TrustServerCertificate=True; MultipleActiveResultSets=true");
            DbContext = new AppDbContext(options);

            UserTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjdjYTA5ZDM2LTQzNzEtNDVkZS1iZjk3LThkM2QwZGU0Zjk4ZiIsImp0aSI6IjcyZDViZTMwLWEyYjQtNDM2YS1iZTVmLWM1MWYzMWQxY2QwMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE3NDIxMzU5NzksImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcyMzYiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjUxNzMifQ.j8mag99aCoDJjhpTFNnGbtE99vWVH1bXunjGZg9htfw";
            AdminTokenJWT = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEwZDU0MDEyLTBjNGUtNGI4Ni1hZjM4LTk4NmMyYjc1NTc2YiIsImp0aSI6ImRhYzk5NDYyLTVmNTgtNGQ5MS05NTI2LWE4ODhmNTJhNmMyYiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMTM1OTQ4LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MjM2IiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MTczIn0.CyKOC2fSTXUlpMmKagqBdM6ahTuZoZ0vZLxaVDwGNEg";

            InitializeDatabase().GetAwaiter().GetResult();
        }

        private async Task InitializeDatabase()
        {
            await DbContext.Database.EnsureCreatedAsync();

            await TestBase.SeedRoleTestBase(DbContext);
            await TestBase.SeedPlayerPositionTestBase(DbContext);
            await TestBase.SeedPlayerFootTestBase(DbContext);
            await TestBase.SeedOfferStatusTestBase(DbContext);
            await TestBase.SeedUserTestBase(DbContext, TestBase.CreateUserManager(DbContext));
            await TestBase.SeedClubHistoryTestBase(DbContext);
            await TestBase.SeedProblemTestBase(DbContext);
            await TestBase.SeedChatTestBase(DbContext);
            await TestBase.SeedMessageTestBase(DbContext);
            await TestBase.SeedPlayerAdvertisementTestBase(DbContext);
            await TestBase.SeedClubOfferTestBase(DbContext);
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            DbContext.Dispose();
        }
    }
}
