namespace Products.Domain
{
    public static class DbContextExtenstion
    {

        public static void EnsureSeeded(this ProductContext context)
        {
            DataSeeder.SeedData(context);
        }

    }
}
