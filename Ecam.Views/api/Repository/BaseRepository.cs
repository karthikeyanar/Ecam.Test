using Ecam.Framework;
using Ecam.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace Ecam.Views.Repository
{
    public class RepositoryFactory<Contract, Model>
        where Contract : BaseContract
        where Model : BaseEntity<Model>
    {
        public static BaseRepository<Contract, Model> Instance {
            get {
                // See if there is an instance of ContractRepository available
                Type repositoryType = Type.GetType(string.Format("{0}Repository", typeof(Contract).Name));
                if (repositoryType == null)
                {
                    repositoryType = typeof(BaseRepository<,>).MakeGenericType(typeof(Contract), typeof(Model));
                }
                return (BaseRepository<Contract, Model>)Activator.CreateInstance(repositoryType);
            }
        }
    }

    public class BaseRepository : IRepository
    {
        public IEnumerable<ErrorInfo> Errors { get; set; }

        public virtual int id {
            get {
                HttpContextBase currentHttpContext = HttpContextFactory.GetHttpContext();
                int id = int.MinValue;
                if (currentHttpContext.Request.RequestContext.RouteData.Values.ContainsKey("id") &&
                    !string.IsNullOrEmpty(currentHttpContext.Request.RequestContext.RouteData.Values["id"].ToString()))
                {
                    int.TryParse(currentHttpContext.Request.RequestContext.RouteData.Values["id"].ToString(), out id);
                }
                return id;
            }
        }
    }

    public interface IRepository
    {
        IEnumerable<ErrorInfo> Errors { get; set; }
        int id { get; }
    }

    public interface IRepository<Contract> : IRepository where Contract : BaseContract
    {
        List<Contract> Get(int? id = null);
        Contract Validate(Contract contract);
        Contract Save(Contract contract);
        List<ErrorInfo> Delete(int id);
        List<Contract> Find(Contract criteria);
        PaginatedListResult<Contract> Search(Contract criteria, Paging paging);
    }

    public class BaseRepository<Contract, Model> : BaseRepository, IRepository<Contract>
        where Contract : BaseContract
        where Model : BaseEntity<Model>
    {

        /// <summary>
        /// Validates that ID is valid for the current entity
        /// </summary>
        public virtual void ValidateID()
        {
            if (this.id > 0)
            {
                var model = this.CurrentModel;
            }
        }

        public virtual Contract CurrentContract {
            get { return this.Get(id).SingleOr404(); }
        }

        public virtual Model CurrentModel {
            get { return BaseEntity<Model>.GetById(id).SingleOr404(); }
        }

        public virtual bool Validate()
        {
            return CurrentModel != null;
        }

        public virtual NameValueCollection ModelToContractMapping {
            get { return null; }
        }

        public virtual NameValueCollection ContractToModelMapping {
            get {
                NameValueCollection nvc = new NameValueCollection();
                return nvc;
            }
        }

        public virtual List<Contract> Get(int? id = null)
        {
            return BaseEntity<Model>.GetById(id).Execute().Select(x => ToContract(x)).ToList();
        }

        public virtual List<Contract> Find(Contract criteria)
        {
            Model criteriaModel = ToModel(criteria);
            IQueryable<Model> query = BaseEntity<Model>.Where(criteriaModel);
            return
                query.ToList().Select(
                    x => Helper.CopyValues<Contract, Model>(x, ModelToContractMapping)).ToList();
        }

        public PaginatedListResult<Contract> Search(Contract criteria, Paging paging)
        {
            Model criteriaModel = ToModel(criteria);
            IQueryable<Model> query = BaseEntity<Model>.Where(criteriaModel);
            paging.Total = query.Count();
            if (string.IsNullOrEmpty(paging.SortOrder))
            {
                paging.SortOrder = "asc";
            }
            if (string.IsNullOrEmpty(paging.SortName) == false)
            {
                query = query.OrderBy(paging.SortName, (paging.SortOrder == "asc"));
            }
            if (paging.PageSize > 0)
            {
                query = query.Skip((paging.PageIndex - 1) * paging.PageSize).Take(paging.PageSize);
            }
            PaginatedListResult<Contract> paginatedList = new PaginatedListResult<Contract>();
            paginatedList.rows = query.ToList().Select(x => Helper.CopyValues<Contract, Model>(x, ModelToContractMapping)).ToList();
            paginatedList.total = paging.Total;
            return paginatedList;
        }

        public virtual Contract Validate(Contract contract)
        {
            Model copiedModel;
            if (contract.id.IsNew())
            {
                copiedModel = ToModel(contract);
            }
            else
            {
                copiedModel = UpdateModel(contract);
            }
            copiedModel.ValidateChecking();
            if (copiedModel.Errors != null)
            {
                if (copiedModel.Errors.Errors.Count() > 0)
                {
                    contract.Errors = this.CloneErrors(copiedModel.Errors.Errors);
                    return contract;
                }
            }
            return contract;
        }

        public virtual Contract Save(Contract contract)
        {
            if (contract.id.IsNew())
            {
                Model copiedModel = ToModel(contract);
                copiedModel.Save();
                if (copiedModel.Errors != null)
                {
                    if (copiedModel.Errors.Errors.Count() > 0)
                    {
                        contract.Errors = this.CloneErrors(copiedModel.Errors.Errors);
                        return contract;
                    }
                }
                //copiedModel.Errors 
                contract.id = copiedModel.id;
                //return contract;
                return contract; // Get(contract.id).FirstOrDefault();
            }
            else
            {
                // Attempt to get the Model from the database first. We do this so that things like
                // CreatedBy, CreatedDate for existing entities is not overwritten
                // Get the original fund
                Model updatedModel = UpdateModel(contract);
                updatedModel.Save();
                if (updatedModel.Errors != null)
                {
                    if (updatedModel.Errors.Errors.Count() > 0)
                    {
                        contract.Errors = this.CloneErrors(updatedModel.Errors.Errors);
                        return contract;
                    }
                }
                return contract; // Get(contract.id).FirstOrDefault();
            }
        }

        private List<ErrorInfo> CloneErrors(List<ErrorInfo> errors)
        {
            List<ErrorInfo> cloneErrros = new List<ErrorInfo>();
            foreach (ErrorInfo err in errors)
            {
                cloneErrros.Add(new ErrorInfo { ErrorMessage = err.ErrorMessage, PropertyName = err.PropertyName });
            }
            return cloneErrros;
        }

        /// <summary>
        /// Creates a Model with the values mapped from the contract
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public virtual Model ToModel(Contract contract)
        {
            // We dont need to do this(commented code), as we are doing this in BaseRepository.Save
            //Model currentModel = EntityFactory<Model>.GetById(ID).FirstOrDefault();
            //Model copiedModel = null;
            //if (currentModel != null) {
            //    copiedModel = Helper.CopyValues<Model, Contract>(contract, currentModel, ContractToModelMapping);
            //} else {
            //    copiedModel = Helper.CopyValues<Model, Contract>(contract, ContractToModelMapping);
            //}
            Model copiedModel = Helper.CopyValues<Model, Contract>(contract, ContractToModelMapping);
            return copiedModel;
        }

        /// <summary>
        /// Updates the Model with the values from the contract. This method first gets the existing Model
        /// from the database, and then applies the values from the contract onto the model
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        public virtual Model UpdateModel(Contract contract)
        {
            Model originalModel = BaseEntity<Model>.GetById(contract.id).SingleOr404();
            Model updatedModel = Helper.CopyValues(contract, originalModel, ContractToModelMapping);
            return updatedModel;
        }

        public virtual Contract ToContract(Model model)
        {
            Contract copiedContract = Helper.CopyValues<Contract, Model>(model, ModelToContractMapping);
            return copiedContract;
        }

        public List<ErrorInfo> Delete(int id)
        {
            var record = BaseEntity<Model>.All().Where(q => q.id == id).FirstOrDefault();
            record.Delete();
            return record.Errors.Errors;
        }



    }
}