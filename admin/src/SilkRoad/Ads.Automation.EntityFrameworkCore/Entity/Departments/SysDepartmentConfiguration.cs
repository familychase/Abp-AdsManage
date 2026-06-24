using Ads.Automation.Domain.Departments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ads.Automation.EntityFrameworkCore.Entity.Departments
{
    public class SysDepartmentConfiguration : IEntityTypeConfiguration<SysDepartment>
    {
        public void Configure(EntityTypeBuilder<SysDepartment> builder)
        {
            builder.ToTable("sys_departments");
        }
    }
}
