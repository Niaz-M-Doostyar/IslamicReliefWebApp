﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IR_Admin.Db
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class IR_DBEntities : DbContext
    {
        public IR_DBEntities()
            : base("name=IR_DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<BeneficiariesRecord> BeneficiariesRecords { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CountryBasicProjectData> CountryBasicProjectDatas { get; set; }
        public virtual DbSet<CountryProjectSectorInformation> CountryProjectSectorInformations { get; set; }
        public virtual DbSet<CountrySustainableDevelopmentGoalInformation> CountrySustainableDevelopmentGoalInformations { get; set; }
        public virtual DbSet<Province> Provinces { get; set; }
        public virtual DbSet<AssignUser> AssignUsers { get; set; }
        public virtual DbSet<ProjectRelatedCategory> ProjectRelatedCategories { get; set; }
        public virtual DbSet<PredictedFlexibleInterventionsData> PredictedFlexibleInterventionsDatas { get; set; }
        public virtual DbSet<FlexibleIntervention> FlexibleInterventions { get; set; }
        public virtual DbSet<BeneficiaryRecord> BeneficiaryRecords { get; set; }
    }
}