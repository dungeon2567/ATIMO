using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace ATIMO.Models.Faturamento
{
    public class FATURAMENTOEntities : DbContext
    {
        public FATURAMENTOEntities() : base("name=FATURAMENTOEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<FATURAMENTOEntities>(null);

            string l_DefaultSchema = ConfigurationManager.AppSettings["db_default_schema"];

            if (!string.IsNullOrEmpty(l_DefaultSchema))
                modelBuilder.HasDefaultSchema(l_DefaultSchema);

            base.OnModelCreating(modelBuilder);
            
        }

        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<ServicoPMSP> ServicosPMSP { get; set; }
        public DbSet<Faturamento> Faturamentos { get; set; }
        public DbSet<FaturamentoRetencoes> FaturamentoRetencoes { get; set; }
        public DbSet<FaturamentoDescricao> FaturamentoDescricoes { get; set; }
        public DbSet<TipoImposto> TiposImposto { get; set; }
        public DbSet<EmpresaServicoPMSP> EmpresaServicoPMSP { get; set; }
        public DbSet<EmailLogEnvio> EmailLogEnvio { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<EmailSMTP> EmailSMTP { get; set; }

        public DbSet<SituacaoPagamento> SituacoesPagamento { get; set; }

    }
}