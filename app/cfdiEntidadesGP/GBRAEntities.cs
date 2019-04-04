namespace cfdiEntidadesGP
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class GBRAEntities : DbContext
    {
        public GBRAEntities(String connectionString) : base(connectionString)
        {

        }
    }
}
