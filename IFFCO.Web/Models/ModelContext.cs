using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Devart.Data.Oracle;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Repository.Pattern;
using IFFCO.HRMS.Repository.Pattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IFFCO.TECHPROD.Web.Models
{
    public partial class ModelContext : DbContext
    {

        readonly string conn = new AppConfiguration().ConnectionString;
        readonly string SchemaName = new AppConfiguration().SchemaName;

        public ModelContext()
        {
        }        

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }
        public virtual DbSet<AdmEmpprgAccess> AdmEmpprgAccess { get; set; }
        public virtual DbSet<AdmEmpUnitAccess> AdmEmpUnitAccess { get; set; }
        public virtual DbSet<AdmPrgMaster> AdmPrgMaster { get; set; }
        public virtual DbSet<AdmSubMenuMsts> AdmSubMenuMsts { get; set; }
        public virtual DbSet<DailyCodensateDtls> DailyCodensateDtls { get; set; }
        public virtual DbSet<DailyGasAlloc> DailyGasAlloc { get; set; }
        public virtual DbSet<DailyPlantInput> DailyPlantInput { get; set; }
        public virtual DbSet<DailyPlantOutput> DailyPlantOutput { get; set; }
        public virtual DbSet<DailyPurgeGasDtls> DailyPurgeGasDtls { get; set; }
        public virtual DbSet<DailySpEnergy> DailySpEnergy { get; set; }
        public virtual DbSet<DailyTechInput> DailyTechInput { get; set; }
        public virtual DbSet<DailytechOutput> DailytechOutput { get; set; }
        public virtual DbSet<DailyTechOutput1> DailyTechOutput1 { get; set; }
        public virtual DbSet<FactorMaster> FactorMaster { get; set; }
        public virtual DbSet<GasMaster> GasMaster { get; set; }
        public virtual DbSet<OutputMaster> OutputMaster { get; set; }
        public virtual DbSet<ParameterMaster> ParameterMaster { get; set; }
        public virtual DbSet<PlantMaster> PlantMaster { get; set; }
        public virtual DbSet<RefEnergy> RefEnergy { get; set; }
        public DataTable GetSQLQuery(string sqlquery)
        {
            DataTable dt = new DataTable();

            OracleConnection connection = new OracleConnection(conn);

            OracleDataAdapter oraAdapter = new OracleDataAdapter(sqlquery, connection);

            OracleCommandBuilder oraBuilder = new OracleCommandBuilder(oraAdapter);

            oraAdapter.Fill(dt);

            return dt;
        }

        public int insertUpdateToDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                cmd.CommandType = CommandType.Text;
                connection.Open();
                i = cmd.ExecuteNonQuery();
                connection.Close();
                return i;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return i = 0;
            }
        }

        public int GetScalerFromDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                connection.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return i = 0;
            }
        }

        public string GetCharScalerFromDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                connection.Open();
                string result = Convert.ToString(cmd.ExecuteScalar());
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return null;
            }
        }

        public int ExecuteProcedure(string procedure, params object[] parameters)
        {
            List<OracleParameter> oracleParameterList = ((List<OracleParameter>)parameters[0]);
            string[] oracleParameters = new string[oracleParameterList.Count];
            string query = "BEGIN " + procedure + "(";
            for (int i = 0; i < oracleParameterList.Count; i++)
            {
                OracleParameter parameter = oracleParameterList[i] as OracleParameter;
                oracleParameters[i] = ":" + parameter.ParameterName;
            }
            query += string.Join(",", oracleParameters);
            query += "); end;";
            //Database.OpenConnection()
            return Database.ExecuteSqlCommand(query, oracleParameterList);
        }



        public int ExecuteProcedureForRefCursor(string procedure, params object[] parameters)
        {
            List<OracleParameter> oracleParameterList = ((List<OracleParameter>)parameters[0]);
            string[] oracleParameters = new string[oracleParameterList.Count];
            string query = "BEGIN " + procedure + "(";
            for (int i = 0; i < oracleParameterList.Count; i++)
            {
                OracleParameter parameter = oracleParameterList[i] as OracleParameter;
                oracleParameters[i] = ":" + parameter.ParameterName;
            }
            query += string.Join(",", oracleParameters);
            query += "); end;";


            Database.OpenConnection();
            int x = Database.ExecuteSqlCommand(query, oracleParameterList);
            Database.CloseConnection();
            return x;
        }

        public Task<int> ExecuteProcedureAsync<TElement>(string procedure, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteQuery<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteQueryAsync<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> ExecuteSelectProcedure<TElement>(string procedure, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> ExecuteSelectQuery<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            //SyncObjectsStatePreCommit();
            var changesAsync = await base.SaveChangesAsync(cancellationToken);
            SyncObjectsStatePostCommit();
            return changesAsync;
        }

        public void SyncObjectsStatePostCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }
        private void SyncObjectsStatePreCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseOracle(conn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AdmEmpprgAccess>(entity =>
            {
                entity.HasKey(e => new { e.Empid, e.Projectid, e.Moduleid, e.Programid, e.Programtype });

                entity.ToTable("ADM_EMPPRG_ACCESS", "TECHANA");

                entity.HasIndex(e => new { e.Empid, e.Programtype, e.Programid, e.Projectid, e.Moduleid })
                    .HasName("PK_ADMEMPPRGACS")
                    .IsUnique();

                entity.Property(e => e.Empid)
                    .HasColumnName("EMPID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.Projectid)
                    .HasColumnName("PROJECTID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Moduleid)
                    .HasColumnName("MODULEID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.Programid)
                    .HasColumnName("PROGRAMID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.Programtype)
                    .HasColumnName("PROGRAMTYPE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("CREATED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("MODIFIED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.PrivDelete)
                    .HasColumnName("PRIV_DELETE")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrivInsert)
                    .HasColumnName("PRIV_INSERT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrivSelect)
                    .HasColumnName("PRIV_SELECT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrivUpdate)
                    .HasColumnName("PRIV_UPDATE")
                    .HasColumnType("char")
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<AdmEmpUnitAccess>(entity =>
            {
                entity.HasKey(e => new { e.Empid, e.Moduleid, e.UnitCode });

                entity.ToTable("ADM_EMP_UNIT_ACCESS", "TECHANA");

                entity.HasIndex(e => new { e.Empid, e.Moduleid, e.UnitCode })
                    .HasName("ADM_EMP_UNIT_ACCESS_PK")
                    .IsUnique();

                entity.Property(e => e.Empid).HasColumnName("EMPID");

                entity.Property(e => e.Moduleid)
                    .HasColumnName("MODULEID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");

                entity.Property(e => e.AllDeptAccess)
                    .HasColumnName("ALL_DEPT_ACCESS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.AllSectionAccess)
                    .HasColumnName("ALL_SECTION_ACCESS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("char")
                    .HasMaxLength(30);

                entity.Property(e => e.DatetimeCreated)
                    .HasColumnName("DATETIME_CREATED")
                    .HasColumnType("date");

                entity.Property(e => e.DatetimeModified)
                    .HasColumnName("DATETIME_MODIFIED")
                    .HasColumnType("date");

                entity.Property(e => e.DefaultUnit)
                    .IsRequired()
                    .HasColumnName("DEFAULT_UNIT")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.HierYn)
                    .IsRequired()
                    .HasColumnName("HIER_YN")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("char")
                    .HasMaxLength(30);

                entity.Property(e => e.OnlyAreaAccess)
                    .HasColumnName("ONLY_AREA_ACCESS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.Projectid)
                    .HasColumnName("PROJECTID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<AdmPrgMaster>(entity =>
            {
                entity.HasKey(e => new { e.Projectid, e.Moduleid, e.Programtype, e.Programid });

                entity.ToTable("ADM_PRG_MASTER", "TECHANA");

                entity.HasIndex(e => new { e.Moduleid, e.Projectid, e.Programtype, e.Programid })
                    .HasName("PK_ADMPRGMAS")
                    .IsUnique();

                entity.Property(e => e.Projectid)
                    .HasColumnName("PROJECTID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Moduleid)
                    .HasColumnName("MODULEID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.Programtype)
                    .HasColumnName("PROGRAMTYPE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Programid)
                    .HasColumnName("PROGRAMID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.ActiveInactive)
                    .HasColumnName("ACTIVE_INACTIVE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(5);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedDate)
                    .HasColumnName("CREATED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.DisplayOrder).HasColumnName("DISPLAY_ORDER");

                entity.Property(e => e.FreeAccess)
                    .HasColumnName("FREE_ACCESS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(1);

                entity.Property(e => e.Ismainform)
                    .HasColumnName("ISMAINFORM")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.ModifiedDate)
                    .HasColumnName("MODIFIED_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.Programname)
                    .IsRequired()
                    .HasColumnName("PROGRAMNAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(255);

                entity.Property(e => e.SubMenuName)
                    .HasColumnName("SUB_MENU_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<AdmSubMenuMsts>(entity =>
            {
                entity.HasKey(e => new { e.Moduleid, e.SubMenuId });

                entity.ToTable("ADM_SUB_MENU_MSTS", "TECHANA");

                entity.HasIndex(e => new { e.SubMenuId, e.Moduleid })
                    .HasName("ADM_SUB_MENU_MSTS_PK")
                    .IsUnique();

                entity.Property(e => e.Moduleid)
                    .HasColumnName("MODULEID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.SubMenuId)
                    .HasColumnName("SUB_MENU_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.DisplayOrder).HasColumnName("DISPLAY_ORDER");

                entity.Property(e => e.ParentMenuId)
                    .HasColumnName("PARENT_MENU_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.Projectid)
                    .HasColumnName("PROJECTID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.SubMenuName)
                    .HasColumnName("SUB_MENU_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<DailyCodensateDtls>(entity =>
            {
                entity.HasKey(e => e.DataDate);

                entity.ToTable("DAILY_CODENSATE_DTLS", "TECHANA");

                entity.HasIndex(e => e.DataDate)
                    .HasName("DAILY_CODENSATE_DTLS_PK")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.A1601)
                    .HasColumnName("A1_601")
                    .HasColumnType("double");

                entity.Property(e => e.A1602)
                    .HasColumnName("A1_602")
                    .HasColumnType("double");

                entity.Property(e => e.A1603)
                    .HasColumnName("A1_603")
                    .HasColumnType("double");

                entity.Property(e => e.A1604)
                    .HasColumnName("A1_604")
                    .HasColumnType("double");

                entity.Property(e => e.A1605)
                    .HasColumnName("A1_605")
                    .HasColumnType("double");

                entity.Property(e => e.A1606)
                    .HasColumnName("A1_606")
                    .HasColumnType("double");

                entity.Property(e => e.A2601)
                    .HasColumnName("A2_601")
                    .HasColumnType("double");

                entity.Property(e => e.A2602)
                    .HasColumnName("A2_602")
                    .HasColumnType("double");

                entity.Property(e => e.A2603)
                    .HasColumnName("A2_603")
                    .HasColumnType("double");

                entity.Property(e => e.A2604)
                    .HasColumnName("A2_604")
                    .HasColumnType("double");

                entity.Property(e => e.A2605)
                    .HasColumnName("A2_605")
                    .HasColumnType("double");

                entity.Property(e => e.A2606)
                    .HasColumnName("A2_606")
                    .HasColumnType("double");

                entity.Property(e => e.A2607)
                    .HasColumnName("A2_607")
                    .HasColumnType("double");

                entity.Property(e => e.A2608)
                    .HasColumnName("A2_608")
                    .HasColumnType("double");

                entity.Property(e => e.A2609)
                    .HasColumnName("A2_609")
                    .HasColumnType("double");

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("CREATION_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.EnGcalHr)
                    .HasColumnName("EN_GCAL_HR")
                    .HasColumnType("double");

                entity.Property(e => e.EnGcalMt)
                    .HasColumnName("EN_GCAL_MT")
                    .HasColumnType("double");

                entity.Property(e => e.U1601)
                    .HasColumnName("U1_601")
                    .HasColumnType("double");

                entity.Property(e => e.U1602)
                    .HasColumnName("U1_602")
                    .HasColumnType("double");

                entity.Property(e => e.U2601)
                    .HasColumnName("U2_601")
                    .HasColumnType("double");

                entity.Property(e => e.U2602)
                    .HasColumnName("U2_602")
                    .HasColumnType("double");
            });

            modelBuilder.Entity<DailyGasAlloc>(entity =>
            {
                entity.HasKey(e => new { e.GasCode, e.DataDate });

                entity.ToTable("DAILY_GAS_ALLOC", "TECHANA");

                entity.HasIndex(e => new { e.GasCode, e.DataDate })
                    .HasName("PK_GASALLOC")
                    .IsUnique();

                entity.Property(e => e.GasCode)
                    .HasColumnName("GAS_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(3);

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.AllocatedQty)
                    .HasColumnName("ALLOCATED_QTY")
                    .HasColumnType("double");

                entity.Property(e => e.Basis)
                    .HasColumnName("BASIS")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("CREATION_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.DrawnQty).HasColumnName("DRAWN_QTY");

                entity.Property(e => e.GcvEnergy)
                    .HasColumnName("GCV_ENERGY")
                    .HasColumnType("double");

                entity.Property(e => e.LcvEnergy)
                    .HasColumnName("LCV_ENERGY")
                    .HasColumnType("double");

                entity.Property(e => e.PriorityLevel).HasColumnName("PRIORITY_LEVEL");

                entity.HasOne(d => d.GasCodeNavigation)
                    .WithMany(p => p.DailyGasAlloc)
                    .HasForeignKey(d => d.GasCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GALLOC");
            });

            modelBuilder.Entity<DailyPlantInput>(entity =>
            {
                entity.HasKey(e => new { e.DateTime, e.FeedDateTime, e.PrCode, e.Revised });

                entity.ToTable("DAILY_PLANT_INPUT", "TECHANA");

                entity.HasIndex(e => new { e.FeedDateTime, e.PrCode, e.DateTime, e.Revised })
                    .HasName("PK_DAILY_PLANT_INPUT")
                    .IsUnique();

                entity.Property(e => e.DateTime)
                    .HasColumnName("DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.FeedDateTime)
                    .HasColumnName("FEED_DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.PrCode)
                    .HasColumnName("PR_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Revised)
                    .HasColumnName("REVISED")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.Freeze)
                    .IsRequired()
                    .HasColumnName("FREEZE")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrValue)
                    .HasColumnName("PR_VALUE")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 9);

                entity.Property(e => e.Shift)
                    .IsRequired()
                    .HasColumnName("SHIFT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.HasOne(d => d.PrCodeNavigation)
                    .WithMany(p => p.DailyPlantInput)
                    .HasForeignKey(d => d.PrCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PLANT_INPUT");
            });

            modelBuilder.Entity<DailyPlantOutput>(entity =>
            {
                entity.HasKey(e => new { e.DateTime, e.FeedDateTime, e.PrCode, e.Revised });

                entity.ToTable("DAILY_PLANT_OUTPUT", "TECHANA");

                entity.HasIndex(e => new { e.PrCode, e.FeedDateTime, e.Revised, e.DateTime })
                    .HasName("PK_PLANT_OUTPUT")
                    .IsUnique();

                entity.Property(e => e.DateTime)
                    .HasColumnName("DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.FeedDateTime)
                    .HasColumnName("FEED_DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.PrCode)
                    .HasColumnName("PR_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(7);

                entity.Property(e => e.Revised)
                    .HasColumnName("REVISED")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.Freeze)
                    .HasColumnName("FREEZE")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrValue)
                    .HasColumnName("PR_VALUE")
                    .HasColumnType("double");

                entity.Property(e => e.Shift)
                    .HasColumnName("SHIFT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.HasOne(d => d.PrCodeNavigation)
                    .WithMany(p => p.DailyPlantOutput)
                    .HasForeignKey(d => d.PrCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DAILYPLANT_OUTPUT");
            });

            modelBuilder.Entity<DailyPurgeGasDtls>(entity =>
            {
                entity.HasKey(e => e.DataDate);

                entity.ToTable("DAILY_PURGE_GAS_DTLS", "TECHANA");

                entity.HasIndex(e => e.DataDate)
                    .HasName("SYS_C0036872")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("CREATION_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.ProdAmm1Pg).HasColumnName("PROD_AMM1_PG");

                entity.Property(e => e.ProdAmm2Pg).HasColumnName("PROD_AMM2_PG");

                entity.Property(e => e.PurgeGasAmm1).HasColumnName("PURGE_GAS_AMM1");

                entity.Property(e => e.PurgeGasAmm2).HasColumnName("PURGE_GAS_AMM2");
            });

            modelBuilder.Entity<DailySpEnergy>(entity =>
            {
                entity.HasKey(e => new { e.DataDate, e.OpCode });

                entity.ToTable("DAILY_SP_ENERGY", "TECHANA");

                entity.HasIndex(e => new { e.OpCode, e.DataDate })
                    .HasName("PK_DAILY_SP_ENERGY")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.OpCode)
                    .HasColumnName("OP_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(8);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.FeedDateTime)
                    .HasColumnName("FEED_DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.OpValue)
                    .HasColumnName("OP_VALUE")
                    .HasAnnotation("Precision", 16)
                    .HasAnnotation("Scale", 4);
            });

            modelBuilder.Entity<DailyTechInput>(entity =>
            {
                entity.HasKey(e => new { e.DataDate, e.FeedDtTime, e.PrCode, e.Revised });

                entity.ToTable("DAILY_TECH_INPUT", "TECHANA");

                entity.HasIndex(e => new { e.Revised, e.FeedDtTime, e.PrCode, e.DataDate })
                    .HasName("PK_TECH_INPUT")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.FeedDtTime)
                    .HasColumnName("FEED_DT_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.PrCode)
                    .HasColumnName("PR_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Revised)
                    .HasColumnName("REVISED")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.Freeze)
                    .IsRequired()
                    .HasColumnName("FREEZE")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrValue)
                    .HasColumnName("PR_VALUE")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 9);

                entity.Property(e => e.Shift)
                    .IsRequired()
                    .HasColumnName("SHIFT")
                    .HasColumnType("char")
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<DailytechOutput>(entity =>
            {
                entity.HasKey(e => e.DataDate);

                entity.ToTable("DAILYTECH_OUTPUT", "TECHANA");

                entity.HasIndex(e => e.DataDate)
                    .HasName("PK_DAILYTECH_OUTPUT")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.A1501)
                    .HasColumnName("A1_501")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1506)
                    .HasColumnName("A1_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1507)
                    .HasColumnName("A1_507")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1508)
                    .HasColumnName("A1_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1509)
                    .HasColumnName("A1_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1511)
                    .HasColumnName("A1_511")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1512)
                    .HasColumnName("A1_512")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1513)
                    .HasColumnName("A1_513")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1514)
                    .HasColumnName("A1_514")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1515)
                    .HasColumnName("A1_515")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1516)
                    .HasColumnName("A1_516")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1517)
                    .HasColumnName("A1_517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1518)
                    .HasColumnName("A1_518")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1519)
                    .HasColumnName("A1_519")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1520)
                    .HasColumnName("A1_520")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1521)
                    .HasColumnName("A1_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1522)
                    .HasColumnName("A1_522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1524)
                    .HasColumnName("A1_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1525)
                    .HasColumnName("A1_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1538)
                    .HasColumnName("A1_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1540)
                    .HasColumnName("A1_540")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1541)
                    .HasColumnName("A1_541")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1542)
                    .HasColumnName("A1_542")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1543)
                    .HasColumnName("A1_543")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1544)
                    .HasColumnName("A1_544")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1545)
                    .HasColumnName("A1_545")
                    .HasAnnotation("Precision", 29)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1546)
                    .HasColumnName("A1_546")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1547)
                    .HasColumnName("A1_547")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1548)
                    .HasColumnName("A1_548")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1549)
                    .HasColumnName("A1_549")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1550)
                    .HasColumnName("A1_550")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1552)
                    .HasColumnName("A1_552")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1568)
                    .HasColumnName("A1_568")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1569)
                    .HasColumnName("A1_569")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1570)
                    .HasColumnName("A1_570")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A1571).HasColumnName("A1_571");

                entity.Property(e => e.A1572).HasColumnName("A1_572");

                entity.Property(e => e.A2502)
                    .HasColumnName("A2_502")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2506)
                    .HasColumnName("A2_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2507)
                    .HasColumnName("A2_507")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2508)
                    .HasColumnName("A2_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2509)
                    .HasColumnName("A2_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2511)
                    .HasColumnName("A2_511")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2517)
                    .HasColumnName("A2_517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2518)
                    .HasColumnName("A2_518")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2519)
                    .HasColumnName("A2_519")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2520)
                    .HasColumnName("A2_520")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2521)
                    .HasColumnName("A2_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2522)
                    .HasColumnName("A2_522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2523)
                    .HasColumnName("A2_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2524)
                    .HasColumnName("A2_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2527)
                    .HasColumnName("A2_527")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2528)
                    .HasColumnName("A2_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2529)
                    .HasColumnName("A2_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2530)
                    .HasColumnName("A2_530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2532)
                    .HasColumnName("A2_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2535)
                    .HasColumnName("A2_535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2536)
                    .HasColumnName("A2_536")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2551)
                    .HasColumnName("A2_551")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2554)
                    .HasColumnName("A2_554")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2555)
                    .HasColumnName("A2_555")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2556)
                    .HasColumnName("A2_556")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2557)
                    .HasColumnName("A2_557")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2558)
                    .HasColumnName("A2_558")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2559)
                    .HasColumnName("A2_559")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2560)
                    .HasColumnName("A2_560")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2561)
                    .HasColumnName("A2_561")
                    .HasAnnotation("Precision", 29)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2562)
                    .HasColumnName("A2_562")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2563)
                    .HasColumnName("A2_563")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2564)
                    .HasColumnName("A2_564")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2565)
                    .HasColumnName("A2_565")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2567)
                    .HasColumnName("A2_567")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.A2582).HasColumnName("A2_582");

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.Fislng)
                    .HasColumnName("FISLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Flng)
                    .HasColumnName("FLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Gspcl)
                    .HasColumnName("GSPCL")
                    .HasColumnType("double");

                entity.Property(e => e.Imgspcl).HasColumnName("IMGSPCL");

                entity.Property(e => e.Islng)
                    .HasColumnName("ISLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Ng)
                    .HasColumnName("NG")
                    .HasColumnType("double");

                entity.Property(e => e.OldIslng).HasColumnName("OLD_ISLNG");

                entity.Property(e => e.Os501)
                    .HasColumnName("OS_501")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os504)
                    .HasColumnName("OS_504")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os505)
                    .HasColumnName("OS_505")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os506)
                    .HasColumnName("OS_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os508)
                    .HasColumnName("OS_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os509)
                    .HasColumnName("OS_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os511)
                    .HasColumnName("OS_511")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os512)
                    .HasColumnName("OS_512")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os514)
                    .HasColumnName("OS_514")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os515)
                    .HasColumnName("OS_515")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os517)
                    .HasColumnName("OS_517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os521)
                    .HasColumnName("OS_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os523)
                    .HasColumnName("OS_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os524)
                    .HasColumnName("OS_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os525)
                    .HasColumnName("OS_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os526)
                    .HasColumnName("OS_526")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os533)
                    .HasColumnName("OS_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os535)
                    .HasColumnName("OS_535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os536)
                    .HasColumnName("OS_536")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os537)
                    .HasColumnName("OS_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os538)
                    .HasColumnName("OS_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os545)
                    .HasColumnName("OS_545")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os547)
                    .HasColumnName("OS_547")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os548)
                    .HasColumnName("OS_548")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os549)
                    .HasColumnName("OS_549")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os550)
                    .HasColumnName("OS_550")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os551)
                    .HasColumnName("OS_551")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os552)
                    .HasColumnName("OS_552")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os553)
                    .HasColumnName("OS_553")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os554)
                    .HasColumnName("OS_554")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os555)
                    .HasColumnName("OS_555")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os556)
                    .HasColumnName("OS_556")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os557)
                    .HasColumnName("OS_557")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os558)
                    .HasColumnName("OS_558")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os559)
                    .HasColumnName("OS_559")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os560)
                    .HasColumnName("OS_560")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os561)
                    .HasColumnName("OS_561")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os562)
                    .HasColumnName("OS_562")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os563)
                    .HasColumnName("OS_563")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os564)
                    .HasColumnName("OS_564")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os565)
                    .HasColumnName("OS_565")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Os566)
                    .HasColumnName("OS_566")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1523)
                    .HasColumnName("PH1_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1523nm).HasColumnName("PH1_523NM");

                entity.Property(e => e.Ph1523p45).HasColumnName("PH1_523P45");

                entity.Property(e => e.Ph1523pp).HasColumnName("PH1_523PP");

                entity.Property(e => e.Ph1525)
                    .HasColumnName("PH1_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1528)
                    .HasColumnName("PH1_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1528nm).HasColumnName("PH1_528NM");

                entity.Property(e => e.Ph1528p45).HasColumnName("PH1_528P45");

                entity.Property(e => e.Ph1528pp).HasColumnName("PH1_528PP");

                entity.Property(e => e.Ph1529)
                    .HasColumnName("PH1_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1530)
                    .HasColumnName("PH1_530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1531)
                    .HasColumnName("PH1_531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1532)
                    .HasColumnName("PH1_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1532nm).HasColumnName("PH1_532NM");

                entity.Property(e => e.Ph1533)
                    .HasColumnName("PH1_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1533nm).HasColumnName("PH1_533NM");

                entity.Property(e => e.Ph1534)
                    .HasColumnName("PH1_534")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1534nm).HasColumnName("PH1_534NM");

                entity.Property(e => e.Ph1534p45).HasColumnName("PH1_534P45");

                entity.Property(e => e.Ph1534pp).HasColumnName("PH1_534PP");

                entity.Property(e => e.Ph1535)
                    .HasColumnName("PH1_535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1536)
                    .HasColumnName("PH1_536")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1536nm).HasColumnName("PH1_536NM");

                entity.Property(e => e.Ph1536p45).HasColumnName("PH1_536P45");

                entity.Property(e => e.Ph1536pp).HasColumnName("PH1_536PP");

                entity.Property(e => e.Ph1537)
                    .HasColumnName("PH1_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1538)
                    .HasColumnName("PH1_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1538nm).HasColumnName("PH1_538NM");

                entity.Property(e => e.Ph1538p45).HasColumnName("PH1_538P45");

                entity.Property(e => e.Ph1538pp).HasColumnName("PH1_538PP");

                entity.Property(e => e.Ph1539)
                    .HasColumnName("PH1_539")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph1623).HasColumnName("PH1_623");

                entity.Property(e => e.Ph1628).HasColumnName("PH1_628");

                entity.Property(e => e.Ph1634).HasColumnName("PH1_634");

                entity.Property(e => e.Ph1636).HasColumnName("PH1_636");

                entity.Property(e => e.Ph1638).HasColumnName("PH1_638");

                entity.Property(e => e.Ph2523)
                    .HasColumnName("PH2_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2523nm).HasColumnName("PH2_523NM");

                entity.Property(e => e.Ph2523p45).HasColumnName("PH2_523P45");

                entity.Property(e => e.Ph2523pp).HasColumnName("PH2_523PP");

                entity.Property(e => e.Ph2525)
                    .HasColumnName("PH2_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2528)
                    .HasColumnName("PH2_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2528nm).HasColumnName("PH2_528NM");

                entity.Property(e => e.Ph2528p45).HasColumnName("PH2_528P45");

                entity.Property(e => e.Ph2528pp).HasColumnName("PH2_528PP");

                entity.Property(e => e.Ph2529)
                    .HasColumnName("PH2_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2530)
                    .HasColumnName("PH2_530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2531)
                    .HasColumnName("PH2_531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2532)
                    .HasColumnName("PH2_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2532nm).HasColumnName("PH2_532NM");

                entity.Property(e => e.Ph2533)
                    .HasColumnName("PH2_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2533nm).HasColumnName("PH2_533NM");

                entity.Property(e => e.Ph2534)
                    .HasColumnName("PH2_534")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2534nm).HasColumnName("PH2_534NM");

                entity.Property(e => e.Ph2534p45).HasColumnName("PH2_534P45");

                entity.Property(e => e.Ph2534pp).HasColumnName("PH2_534PP");

                entity.Property(e => e.Ph2535)
                    .HasColumnName("PH2_535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2536)
                    .HasColumnName("PH2_536")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2536nm).HasColumnName("PH2_536NM");

                entity.Property(e => e.Ph2536p45).HasColumnName("PH2_536P45");

                entity.Property(e => e.Ph2536pp).HasColumnName("PH2_536PP");

                entity.Property(e => e.Ph2537)
                    .HasColumnName("PH2_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2538)
                    .HasColumnName("PH2_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2538nm).HasColumnName("PH2_538NM");

                entity.Property(e => e.Ph2538p45).HasColumnName("PH2_538P45");

                entity.Property(e => e.Ph2538pp).HasColumnName("PH2_538PP");

                entity.Property(e => e.Ph2539)
                    .HasColumnName("PH2_539")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Ph2623).HasColumnName("PH2_623");

                entity.Property(e => e.Ph2628).HasColumnName("PH2_628");

                entity.Property(e => e.Ph2634).HasColumnName("PH2_634");

                entity.Property(e => e.Ph2636).HasColumnName("PH2_636");

                entity.Property(e => e.Ph2638).HasColumnName("PH2_638");

                entity.Property(e => e.Pmtg)
                    .HasColumnName("PMTG")
                    .HasColumnType("double");

                entity.Property(e => e.Rlng)
                    .HasColumnName("RLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Sb01)
                    .HasColumnName("SB01")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb02)
                    .HasColumnName("SB02")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb03)
                    .HasColumnName("SB03")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb04)
                    .HasColumnName("SB04")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb06)
                    .HasColumnName("SB06")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb07)
                    .HasColumnName("SB07")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb08)
                    .HasColumnName("SB08")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb09)
                    .HasColumnName("SB09")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb11)
                    .HasColumnName("SB11")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb12)
                    .HasColumnName("SB12")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb13)
                    .HasColumnName("SB13")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb14)
                    .HasColumnName("SB14")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb15)
                    .HasColumnName("SB15")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb18)
                    .HasColumnName("SB18")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb19)
                    .HasColumnName("SB19")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb20)
                    .HasColumnName("SB20")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb21)
                    .HasColumnName("SB21")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sb22)
                    .HasColumnName("SB22")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg503)
                    .HasColumnName("SG503")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg504)
                    .HasColumnName("SG504")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg508)
                    .HasColumnName("SG508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg509)
                    .HasColumnName("SG509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg510)
                    .HasColumnName("SG510")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg511)
                    .HasColumnName("SG511")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg512)
                    .HasColumnName("SG512")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg514)
                    .HasColumnName("SG514")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg515)
                    .HasColumnName("SG515")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg517)
                    .HasColumnName("SG517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg522)
                    .HasColumnName("SG522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg524)
                    .HasColumnName("SG524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg525)
                    .HasColumnName("SG525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg527)
                    .HasColumnName("SG527")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg528)
                    .HasColumnName("SG528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg529)
                    .HasColumnName("SG529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg530)
                    .HasColumnName("SG530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg531)
                    .HasColumnName("SG531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg533)
                    .HasColumnName("SG533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg534)
                    .HasColumnName("SG534")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg535)
                    .HasColumnName("SG535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg541)
                    .HasColumnName("SG541")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg542)
                    .HasColumnName("SG542")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg543)
                    .HasColumnName("SG543")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg544)
                    .HasColumnName("SG544")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg545)
                    .HasColumnName("SG545")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg546)
                    .HasColumnName("SG546")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg547)
                    .HasColumnName("SG547")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg548)
                    .HasColumnName("SG548")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg549)
                    .HasColumnName("SG549")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg550)
                    .HasColumnName("SG550")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Sg560)
                    .HasColumnName("SG560")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Slng)
                    .HasColumnName("SLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Spmtg)
                    .HasColumnName("SPMTG")
                    .HasColumnType("double");

                entity.Property(e => e.Srlng)
                    .HasColumnName("SRLNG")
                    .HasColumnType("double");

                entity.Property(e => e.Tp501)
                    .HasColumnName("TP_501")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp501n).HasColumnName("TP_501N");

                entity.Property(e => e.Tp501nm).HasColumnName("TP_501NM");

                entity.Property(e => e.Tp501nm45).HasColumnName("TP_501NM45");

                entity.Property(e => e.Tp502)
                    .HasColumnName("TP_502")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp502n).HasColumnName("TP_502N");

                entity.Property(e => e.Tp502nm).HasColumnName("TP_502NM");

                entity.Property(e => e.Tp502nm45).HasColumnName("TP_502NM45");

                entity.Property(e => e.Tp503)
                    .HasColumnName("TP_503")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp503n).HasColumnName("TP_503N");

                entity.Property(e => e.Tp503nm).HasColumnName("TP_503NM");

                entity.Property(e => e.Tp503nm45).HasColumnName("TP_503NM45");

                entity.Property(e => e.Tp504)
                    .HasColumnName("TP_504")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp504n).HasColumnName("TP_504N");

                entity.Property(e => e.Tp504nm).HasColumnName("TP_504NM");

                entity.Property(e => e.Tp504nm45).HasColumnName("TP_504NM45");

                entity.Property(e => e.Tp505)
                    .HasColumnName("TP_505")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp505n).HasColumnName("TP_505N");

                entity.Property(e => e.Tp505nm).HasColumnName("TP_505NM");

                entity.Property(e => e.Tp505nm45).HasColumnName("TP_505NM45");

                entity.Property(e => e.Tp506)
                    .HasColumnName("TP_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp506n).HasColumnName("TP_506N");

                entity.Property(e => e.Tp506nm).HasColumnName("TP_506NM");

                entity.Property(e => e.Tp506nm45).HasColumnName("TP_506NM45");

                entity.Property(e => e.Tp507)
                    .HasColumnName("TP_507")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp507n).HasColumnName("TP_507N");

                entity.Property(e => e.Tp507nm).HasColumnName("TP_507NM");

                entity.Property(e => e.Tp507nm45).HasColumnName("TP_507NM45");

                entity.Property(e => e.Tp508)
                    .HasColumnName("TP_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp508n).HasColumnName("TP_508N");

                entity.Property(e => e.Tp508nm).HasColumnName("TP_508NM");

                entity.Property(e => e.Tp508nm45).HasColumnName("TP_508NM45");

                entity.Property(e => e.Tp509)
                    .HasColumnName("TP_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp509n).HasColumnName("TP_509N");

                entity.Property(e => e.Tp509nm).HasColumnName("TP_509NM");

                entity.Property(e => e.Tp509nm45).HasColumnName("TP_509NM45");

                entity.Property(e => e.Tp510)
                    .HasColumnName("TP_510")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp510n).HasColumnName("TP_510N");

                entity.Property(e => e.Tp510nm).HasColumnName("TP_510NM");

                entity.Property(e => e.Tp510nm45).HasColumnName("TP_510NM45");

                entity.Property(e => e.Tp511)
                    .HasColumnName("TP_511")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp512)
                    .HasColumnName("TP_512")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp513)
                    .HasColumnName("TP_513")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp514)
                    .HasColumnName("TP_514")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp515)
                    .HasColumnName("TP_515")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp515n).HasColumnName("TP_515N");

                entity.Property(e => e.Tp515nm).HasColumnName("TP_515NM");

                entity.Property(e => e.Tp515nm45).HasColumnName("TP_515NM45");

                entity.Property(e => e.Tp516)
                    .HasColumnName("TP_516")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp516n).HasColumnName("TP_516N");

                entity.Property(e => e.Tp516nm).HasColumnName("TP_516NM");

                entity.Property(e => e.Tp516nm45).HasColumnName("TP_516NM45");

                entity.Property(e => e.Tp517)
                    .HasColumnName("TP_517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp517n).HasColumnName("TP_517N");

                entity.Property(e => e.Tp517nm).HasColumnName("TP_517NM");

                entity.Property(e => e.Tp517nm45).HasColumnName("TP_517NM45");

                entity.Property(e => e.Tp518)
                    .HasColumnName("TP_518")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp518n).HasColumnName("TP_518N");

                entity.Property(e => e.Tp518nm).HasColumnName("TP_518NM");

                entity.Property(e => e.Tp518nm45).HasColumnName("TP_518NM45");

                entity.Property(e => e.Tp519)
                    .HasColumnName("TP_519")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp519n).HasColumnName("TP_519N");

                entity.Property(e => e.Tp519nm).HasColumnName("TP_519NM");

                entity.Property(e => e.Tp519nm45).HasColumnName("TP_519NM45");

                entity.Property(e => e.Tp520)
                    .HasColumnName("TP_520")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp520n).HasColumnName("TP_520N");

                entity.Property(e => e.Tp520nm).HasColumnName("TP_520NM");

                entity.Property(e => e.Tp520nm45).HasColumnName("TP_520NM45");

                entity.Property(e => e.Tp521)
                    .HasColumnName("TP_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp521n).HasColumnName("TP_521N");

                entity.Property(e => e.Tp521nm).HasColumnName("TP_521NM");

                entity.Property(e => e.Tp521nm45).HasColumnName("TP_521NM45");

                entity.Property(e => e.Tp522)
                    .HasColumnName("TP_522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp522n).HasColumnName("TP_522N");

                entity.Property(e => e.Tp522nm).HasColumnName("TP_522NM");

                entity.Property(e => e.Tp522nm45).HasColumnName("TP_522NM45");

                entity.Property(e => e.Tp523)
                    .HasColumnName("TP_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp523n).HasColumnName("TP_523N");

                entity.Property(e => e.Tp523nm).HasColumnName("TP_523NM");

                entity.Property(e => e.Tp523nm45).HasColumnName("TP_523NM45");

                entity.Property(e => e.Tp524)
                    .HasColumnName("TP_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp524n).HasColumnName("TP_524N");

                entity.Property(e => e.Tp524nm).HasColumnName("TP_524NM");

                entity.Property(e => e.Tp524nm45).HasColumnName("TP_524NM45");

                entity.Property(e => e.Tp525)
                    .HasColumnName("TP_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp525n).HasColumnName("TP_525N");

                entity.Property(e => e.Tp525nm).HasColumnName("TP_525NM");

                entity.Property(e => e.Tp525nm45).HasColumnName("TP_525NM45");

                entity.Property(e => e.Tp526)
                    .HasColumnName("TP_526")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp526n).HasColumnName("TP_526N");

                entity.Property(e => e.Tp526nm).HasColumnName("TP_526NM");

                entity.Property(e => e.Tp526nm45).HasColumnName("TP_526NM45");

                entity.Property(e => e.Tp527)
                    .HasColumnName("TP_527")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp527n).HasColumnName("TP_527N");

                entity.Property(e => e.Tp527nm).HasColumnName("TP_527NM");

                entity.Property(e => e.Tp527nm45).HasColumnName("TP_527NM45");

                entity.Property(e => e.Tp528)
                    .HasColumnName("TP_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp528n).HasColumnName("TP_528N");

                entity.Property(e => e.Tp528nm).HasColumnName("TP_528NM");

                entity.Property(e => e.Tp528nm45).HasColumnName("TP_528NM45");

                entity.Property(e => e.Tp529)
                    .HasColumnName("TP_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp529n).HasColumnName("TP_529N");

                entity.Property(e => e.Tp529nm).HasColumnName("TP_529NM");

                entity.Property(e => e.Tp529nm45).HasColumnName("TP_529NM45");

                entity.Property(e => e.Tp530)
                    .HasColumnName("TP_530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp530n).HasColumnName("TP_530N");

                entity.Property(e => e.Tp530nm).HasColumnName("TP_530NM");

                entity.Property(e => e.Tp530nm45).HasColumnName("TP_530NM45");

                entity.Property(e => e.Tp531)
                    .HasColumnName("TP_531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp531n).HasColumnName("TP_531N");

                entity.Property(e => e.Tp531nm).HasColumnName("TP_531NM");

                entity.Property(e => e.Tp531nm45).HasColumnName("TP_531NM45");

                entity.Property(e => e.Tp532)
                    .HasColumnName("TP_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp532n).HasColumnName("TP_532N");

                entity.Property(e => e.Tp532nm).HasColumnName("TP_532NM");

                entity.Property(e => e.Tp532nm45).HasColumnName("TP_532NM45");

                entity.Property(e => e.Tp533)
                    .HasColumnName("TP_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp533n).HasColumnName("TP_533N");

                entity.Property(e => e.Tp533nm).HasColumnName("TP_533NM");

                entity.Property(e => e.Tp533nm45).HasColumnName("TP_533NM45");

                entity.Property(e => e.Tp534)
                    .HasColumnName("TP_534")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp534n).HasColumnName("TP_534N");

                entity.Property(e => e.Tp534nm).HasColumnName("TP_534NM");

                entity.Property(e => e.Tp534nm45).HasColumnName("TP_534NM45");

                entity.Property(e => e.Tp535)
                    .HasColumnName("TP_535")
                    .HasAnnotation("Precision", 25)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp535n).HasColumnName("TP_535N");

                entity.Property(e => e.Tp535nm).HasColumnName("TP_535NM");

                entity.Property(e => e.Tp535nm45).HasColumnName("TP_535NM45");

                entity.Property(e => e.Tp536)
                    .HasColumnName("TP_536")
                    .HasAnnotation("Precision", 25)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp536n).HasColumnName("TP_536N");

                entity.Property(e => e.Tp536nm).HasColumnName("TP_536NM");

                entity.Property(e => e.Tp536nm45).HasColumnName("TP_536NM45");

                entity.Property(e => e.Tp537)
                    .HasColumnName("TP_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp538)
                    .HasColumnName("TP_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp539)
                    .HasColumnName("TP_539")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.Tp540)
                    .HasColumnName("TP_540")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1501)
                    .HasColumnName("U1_501")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1502)
                    .HasColumnName("U1_502")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1503)
                    .HasColumnName("U1_503")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1504)
                    .HasColumnName("U1_504")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1504n)
                    .HasColumnName("U1_504N")
                    .HasColumnType("double");

                entity.Property(e => e.U1504nm)
                    .HasColumnName("U1_504NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1505)
                    .HasColumnName("U1_505")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1505n)
                    .HasColumnName("U1_505N")
                    .HasColumnType("double");

                entity.Property(e => e.U1505nm)
                    .HasColumnName("U1_505NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1506)
                    .HasColumnName("U1_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1506n)
                    .HasColumnName("U1_506N")
                    .HasColumnType("double");

                entity.Property(e => e.U1506nm)
                    .HasColumnName("U1_506NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1507)
                    .HasColumnName("U1_507")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1507n)
                    .HasColumnName("U1_507N")
                    .HasColumnType("double");

                entity.Property(e => e.U1507nm)
                    .HasColumnName("U1_507NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1508)
                    .HasColumnName("U1_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1508n)
                    .HasColumnName("U1_508N")
                    .HasColumnType("double");

                entity.Property(e => e.U1508nm)
                    .HasColumnName("U1_508NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1509)
                    .HasColumnName("U1_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1509n)
                    .HasColumnName("U1_509N")
                    .HasColumnType("double");

                entity.Property(e => e.U1509nm)
                    .HasColumnName("U1_509NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1512)
                    .HasColumnName("U1_512")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1516)
                    .HasColumnName("U1_516")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1517)
                    .HasColumnName("U1_517")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1519)
                    .HasColumnName("U1_519")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1520)
                    .HasColumnName("U1_520")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1521)
                    .HasColumnName("U1_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1522)
                    .HasColumnName("U1_522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1522n)
                    .HasColumnName("U1_522N")
                    .HasColumnType("double");

                entity.Property(e => e.U1522nm)
                    .HasColumnName("U1_522NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1523)
                    .HasColumnName("U1_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1523n)
                    .HasColumnName("U1_523N")
                    .HasColumnType("double");

                entity.Property(e => e.U1523nm)
                    .HasColumnName("U1_523NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1524)
                    .HasColumnName("U1_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1524n)
                    .HasColumnName("U1_524N")
                    .HasColumnType("double");

                entity.Property(e => e.U1524nm)
                    .HasColumnName("U1_524NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1525)
                    .HasColumnName("U1_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1525n)
                    .HasColumnName("U1_525N")
                    .HasColumnType("double");

                entity.Property(e => e.U1525nm)
                    .HasColumnName("U1_525NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1526)
                    .HasColumnName("U1_526")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1526n)
                    .HasColumnName("U1_526N")
                    .HasColumnType("double");

                entity.Property(e => e.U1526nm)
                    .HasColumnName("U1_526NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1527)
                    .HasColumnName("U1_527")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1527n)
                    .HasColumnName("U1_527N")
                    .HasColumnType("double");

                entity.Property(e => e.U1527nm)
                    .HasColumnName("U1_527NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1528)
                    .HasColumnName("U1_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1529)
                    .HasColumnName("U1_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1530)
                    .HasColumnName("U1_530")
                    .HasAnnotation("Precision", 25)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1530n)
                    .HasColumnName("U1_530N")
                    .HasColumnType("double");

                entity.Property(e => e.U1530nm)
                    .HasColumnName("U1_530NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1531)
                    .HasColumnName("U1_531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1532)
                    .HasColumnName("U1_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1533)
                    .HasColumnName("U1_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1536)
                    .HasColumnName("U1_536")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1537)
                    .HasColumnName("U1_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1538)
                    .HasColumnName("U1_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1539)
                    .HasColumnName("U1_539")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1540)
                    .HasColumnName("U1_540")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1541)
                    .HasColumnName("U1_541")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1541n)
                    .HasColumnName("U1_541N")
                    .HasColumnType("double");

                entity.Property(e => e.U1541nm)
                    .HasColumnName("U1_541NM")
                    .HasColumnType("double");

                entity.Property(e => e.U1543)
                    .HasColumnName("U1_543")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1544)
                    .HasColumnName("U1_544")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U1545).HasColumnName("U1_545");

                entity.Property(e => e.U1549).HasColumnName("U1_549");

                entity.Property(e => e.U1550).HasColumnName("U1_550");

                entity.Property(e => e.U1551).HasColumnName("U1_551");

                entity.Property(e => e.U1552).HasColumnName("U1_552");

                entity.Property(e => e.U1553).HasColumnName("U1_553");

                entity.Property(e => e.U1555).HasColumnName("U1_555");

                entity.Property(e => e.U2501)
                    .HasColumnName("U2_501")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2502)
                    .HasColumnName("U2_502")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2503)
                    .HasColumnName("U2_503")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2503n)
                    .HasColumnName("U2_503N")
                    .HasColumnType("double");

                entity.Property(e => e.U2503nm)
                    .HasColumnName("U2_503NM")
                    .HasColumnType("double");

                entity.Property(e => e.U2504)
                    .HasColumnName("U2_504")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2504n).HasColumnName("U2_504N");

                entity.Property(e => e.U2504nm).HasColumnName("U2_504NM");

                entity.Property(e => e.U2505)
                    .HasColumnName("U2_505")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2505n).HasColumnName("U2_505N");

                entity.Property(e => e.U2505nm).HasColumnName("U2_505NM");

                entity.Property(e => e.U2506)
                    .HasColumnName("U2_506")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2506n).HasColumnName("U2_506N");

                entity.Property(e => e.U2506nm).HasColumnName("U2_506NM");

                entity.Property(e => e.U2507)
                    .HasColumnName("U2_507")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2507n).HasColumnName("U2_507N");

                entity.Property(e => e.U2507nm).HasColumnName("U2_507NM");

                entity.Property(e => e.U2508)
                    .HasColumnName("U2_508")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2508n).HasColumnName("U2_508N");

                entity.Property(e => e.U2508nm).HasColumnName("U2_508NM");

                entity.Property(e => e.U2509)
                    .HasColumnName("U2_509")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2509n).HasColumnName("U2_509N");

                entity.Property(e => e.U2509nm).HasColumnName("U2_509NM");

                entity.Property(e => e.U2510)
                    .HasColumnName("U2_510")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2510n).HasColumnName("U2_510N");

                entity.Property(e => e.U2510nm).HasColumnName("U2_510NM");

                entity.Property(e => e.U2513)
                    .HasColumnName("U2_513")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2519)
                    .HasColumnName("U2_519")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2520)
                    .HasColumnName("U2_520")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2521)
                    .HasColumnName("U2_521")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2522)
                    .HasColumnName("U2_522")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2523)
                    .HasColumnName("U2_523")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2524)
                    .HasColumnName("U2_524")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2525)
                    .HasColumnName("U2_525")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2526)
                    .HasColumnName("U2_526")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2526n).HasColumnName("U2_526N");

                entity.Property(e => e.U2526nm).HasColumnName("U2_526NM");

                entity.Property(e => e.U2527)
                    .HasColumnName("U2_527")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2527n).HasColumnName("U2_527N");

                entity.Property(e => e.U2527nm).HasColumnName("U2_527NM");

                entity.Property(e => e.U2528)
                    .HasColumnName("U2_528")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2528n).HasColumnName("U2_528N");

                entity.Property(e => e.U2528nm).HasColumnName("U2_528NM");

                entity.Property(e => e.U2529)
                    .HasColumnName("U2_529")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2529n).HasColumnName("U2_529N");

                entity.Property(e => e.U2529nm).HasColumnName("U2_529NM");

                entity.Property(e => e.U2530)
                    .HasColumnName("U2_530")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2530n).HasColumnName("U2_530N");

                entity.Property(e => e.U2530nm).HasColumnName("U2_530NM");

                entity.Property(e => e.U2531)
                    .HasColumnName("U2_531")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2531n).HasColumnName("U2_531N");

                entity.Property(e => e.U2531nm).HasColumnName("U2_531NM");

                entity.Property(e => e.U2532)
                    .HasColumnName("U2_532")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2532n).HasColumnName("U2_532N");

                entity.Property(e => e.U2532nm).HasColumnName("U2_532NM");

                entity.Property(e => e.U2533)
                    .HasColumnName("U2_533")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2533n).HasColumnName("U2_533N");

                entity.Property(e => e.U2533nm).HasColumnName("U2_533NM");

                entity.Property(e => e.U2534)
                    .HasColumnName("U2_534")
                    .HasAnnotation("Precision", 25)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2534n).HasColumnName("U2_534N");

                entity.Property(e => e.U2534nm).HasColumnName("U2_534NM");

                entity.Property(e => e.U2535)
                    .HasColumnName("U2_535")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2537)
                    .HasColumnName("U2_537")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2538)
                    .HasColumnName("U2_538")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2540)
                    .HasColumnName("U2_540")
                    .HasAnnotation("Precision", 22)
                    .HasAnnotation("Scale", 6);

                entity.Property(e => e.U2541)
                    .HasColumnName("U2_541")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2543)
                    .HasColumnName("U2_543")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.Property(e => e.U2544).HasColumnName("U2_544");

                entity.Property(e => e.U2550).HasColumnName("U2_550");

                entity.Property(e => e.U2551).HasColumnName("U2_551");

                entity.Property(e => e.U2552).HasColumnName("U2_552");

                entity.Property(e => e.U2553).HasColumnName("U2_553");

                entity.Property(e => e.U2555).HasColumnName("U2_555");
            });

            modelBuilder.Entity<DailyTechOutput1>(entity =>
            {
                entity.HasKey(e => new { e.DataDate, e.OpCode, e.FeedDateTime, e.Revised });

                entity.ToTable("DAILY_TECH_OUTPUT", "TECHANA");

                entity.HasIndex(e => e.DataDate)
                    .HasName("DATA_DATE_IDX");

                entity.HasIndex(e => new { e.OpCode, e.Revised, e.FeedDateTime, e.DataDate })
                    .HasName("PK_TECH_OUTPUT")
                    .IsUnique();

                entity.Property(e => e.DataDate)
                    .HasColumnName("DATA_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.OpCode)
                    .HasColumnName("OP_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(7);

                entity.Property(e => e.FeedDateTime)
                    .HasColumnName("FEED_DATE_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.Revised)
                    .HasColumnName("REVISED")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.Freeze)
                    .HasColumnName("FREEZE")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.OpValue)
                    .HasColumnName("OP_VALUE")
                    .HasAnnotation("Precision", 20)
                    .HasAnnotation("Scale", 4);

                entity.HasOne(d => d.OpCodeNavigation)
                    .WithMany(p => p.DailyTechOutput1)
                    .HasForeignKey(d => d.OpCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DAILYTECH_OUTPUT");
            });

            modelBuilder.Entity<FactorMaster>(entity =>
            {
                entity.HasKey(e => new { e.FrCode, e.EffectiveFromDate });

                entity.ToTable("FACTOR_MASTER", "TECHANA");

                entity.HasIndex(e => new { e.EffectiveFromDate, e.FrCode })
                    .HasName("FACTOR_MASTER_PK")
                    .IsUnique();

                entity.Property(e => e.FrCode)
                    .HasColumnName("FR_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(13);

                entity.Property(e => e.EffectiveFromDate)
                    .HasColumnName("EFFECTIVE_FROM_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.EffectiveToDate)
                    .HasColumnName("EFFECTIVE_TO_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.FrName)
                    .HasColumnName("FR_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(75);

                entity.Property(e => e.FrUnit)
                    .HasColumnName("FR_UNIT")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.FrValue)
                    .HasColumnName("FR_VALUE")
                    .HasAnnotation("Precision", 16)
                    .HasAnnotation("Scale", 9);
            });

            modelBuilder.Entity<GasMaster>(entity =>
            {
                entity.HasKey(e => e.GasCode);

                entity.ToTable("GAS_MASTER", "TECHANA");

                entity.HasIndex(e => e.GasCode)
                    .HasName("PK_GMASTER")
                    .IsUnique();

                entity.Property(e => e.GasCode)
                    .HasColumnName("GAS_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(3);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("CREATION_TIME")
                    .HasColumnType("date");

                entity.Property(e => e.GasName)
                    .HasColumnName("GAS_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.GasNameInDatabase)
                    .HasColumnName("GAS NAME IN DATABASE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.PriorityLevel).HasColumnName("PRIORITY_LEVEL");

                entity.Property(e => e.RmCode)
                    .HasColumnName("RM_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(12);
            });

            modelBuilder.Entity<OutputMaster>(entity =>
            {
                entity.HasKey(e => e.OpCode);

                entity.ToTable("OUTPUT_MASTER", "TECHANA");

                entity.HasIndex(e => e.OpCode)
                    .HasName("PK_OUTPUT_MASTER")
                    .IsUnique();

                entity.Property(e => e.OpCode)
                    .HasColumnName("OP_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(7);

                entity.Property(e => e.FoCode)
                    .HasColumnName("FO_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(4);

                entity.Property(e => e.OpName)
                    .HasColumnName("OP_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.OpUnit)
                    .HasColumnName("OP_UNIT")
                    .HasColumnType("varchar2")
                    .HasMaxLength(8);

                entity.Property(e => e.PlCode)
                    .HasColumnName("PL_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(3);

                entity.Property(e => e.PlOutput)
                    .HasColumnName("PL_OUTPUT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.HasOne(d => d.PlCodeNavigation)
                    .WithMany(p => p.OutputMaster)
                    .HasForeignKey(d => d.PlCode)
                    .HasConstraintName("FK_PL_CODE_OUTPUT");
            });

            modelBuilder.Entity<ParameterMaster>(entity =>
            {
                entity.HasKey(e => e.PrCode);

                entity.ToTable("PARAMETER_MASTER", "TECHANA");

                entity.HasIndex(e => e.PrCode)
                    .HasName("PK_PARAMETER_MASTER")
                    .IsUnique();

                entity.Property(e => e.PrCode)
                    .HasColumnName("PR_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.PlCode)
                    .IsRequired()
                    .HasColumnName("PL_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(3);

                entity.Property(e => e.PlInput)
                    .HasColumnName("PL_INPUT")
                    .HasColumnType("char")
                    .HasMaxLength(1);

                entity.Property(e => e.PrName)
                    .IsRequired()
                    .HasColumnName("PR_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(65);

                entity.Property(e => e.PrUnit)
                    .HasColumnName("PR_UNIT")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.HasOne(d => d.PlCodeNavigation)
                    .WithMany(p => p.ParameterMaster)
                    .HasForeignKey(d => d.PlCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PLANT_CODE");
            });

            modelBuilder.Entity<PlantMaster>(entity =>
            {
                entity.HasKey(e => e.PlCode);

                entity.ToTable("PLANT_MASTER", "TECHANA");

                entity.HasIndex(e => e.PlCode)
                    .HasName("PK_PLANT_MASTER")
                    .IsUnique();

                entity.Property(e => e.PlCode)
                    .HasColumnName("PL_CODE")
                    .HasColumnType("char")
                    .HasMaxLength(3);

                entity.Property(e => e.PlName)
                    .HasColumnName("PL_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<RefEnergy>(entity =>
            {
                entity.HasKey(e => e.PlantCode);

                entity.ToTable("REF_ENERGY", "TECHANA");

                entity.HasIndex(e => e.PlantCode)
                    .HasName("REF_ENERGY_PK")
                    .IsUnique();

                entity.Property(e => e.PlantCode)
                    .HasColumnName("PLANT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(5);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.Plant)
                    .HasColumnName("PLANT")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.ReEnergy)
                    .HasColumnName("RE_ENERGY")
                    .HasColumnType("double");

                entity.Property(e => e.ModifiedBy).HasColumnName("MODIFIED_BY");

                entity.Property(e => e.ModifiedDatetime)
                    .HasColumnName("MODIFIED_DATETIME")
                    .HasColumnType("date");
            });
        }

    }
}
