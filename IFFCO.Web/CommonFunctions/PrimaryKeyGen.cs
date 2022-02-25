using IFFCO.TECHPROD.Web.Models;
using IFFCO.HRMS.Repository.Pattern.Core.Factories;
using IFFCO.HRMS.Repository.Pattern.UnitOfWork;
using System.Data;

namespace IFFCO.TECHPROD.Web.CommonFunctions
{
    public class PrimaryKeyGen
    {
        private readonly IRepositoryProvider _repositoryProvider = new RepositoryProvider(new RepositoryFactories());

        private readonly IUnitOfWorkAsync _unitOfWork;

        //IDataContextAsync context;
        private readonly ModelContext _context;
        DataTable _dt = new DataTable();
        public PrimaryKeyGen()
        {
            _context = new ModelContext();
        }

        
    }
}
