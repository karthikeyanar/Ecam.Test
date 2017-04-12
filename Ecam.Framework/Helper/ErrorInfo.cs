using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework
{
    public enum ErrorType
    {
        Error = 1,
        Warning = 2,
        NotFound = 3
    }

    public class ErrorInfo
    {
        public ErrorType ErrorType = ErrorType.Error;
        /// <summary>
        /// To support serialization
        /// </summary>
        public ErrorInfo()
        {

        }


        public ErrorInfo(string propertyName, string errorMessage)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
        }
        public ErrorInfo(string propertyName, string errorMessage, object onObject)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
            this.Object = onObject;
        }
        public ErrorInfo(string propertyName, string errorMessage, object onObject, ErrorType errorType)
        {
            this.PropertyName = propertyName;
            this.ErrorMessage = errorMessage;
            this.Object = onObject;
            this.ErrorType = errorType;
        }

        public string ErrorMessage { get; set; }
        public object Object { get; set; }
        public string PropertyName { get; set; }
        public bool? IsWarning { get; set; }
    }

    public interface IErrorProvider
    {
        List<ErrorInfo> Errors { get; set; }
        void Add(string propertyName, string error);
        void Add(ErrorInfo error);
        string Name { get; }
    }

    public class ErrorProvider : IErrorProvider
    {

        //[ThreadStatic]
        //private static IErrorProvider _current = null;
        /// <summary>
        /// Returns the default error provider
        /// </summary>
       // public static IErrorProvider Current
       // {
           // get
           // {
                //if (_current == null) {
                //    // Dont use HttpContextErrorProvider. The problem is that the Context Provider adds the Error to
                //    // the Context using Context.AddError. MVC API throws InternalServerError in that case, even if you explicitly 
                //    // set the StatusCode on the HttpResponseMessage
                //    //_current = new HttpContextErrorProvider();
                //    _current = new ErrorProvider();
                //}
                //return _current;

			//	return new ErrorProvider();
				/*
                IErrorProvider _current = HttpContextFactory.GetHttpContext().Items["ErrorProvider"] as IErrorProvider;
                if (_current == null)
                {
                    _current = new ErrorProvider();
                    HttpContextFactory.GetHttpContext().Items.Add("ErrorProvider", _current);
                }
                //Debug.WriteLine("Name:ErrorInfo.Current:" + _current.Name);
                return _current;
				 * */
           // }
       // }


        private List<ErrorInfo> _errors = null;
        public List<ErrorInfo> Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
        }

        public ErrorProvider()
        {
            _name = Guid.NewGuid().ToString() + " " + DateTime.Now.Ticks.ToString();
            _errors = new List<ErrorInfo>();
            //Debug.WriteLine(string.Format("Name: {0}", _name));
        }

        public ErrorProvider(string propertyName, string error)
        {
            Errors.Add(new ErrorInfo(propertyName, error));
        }

        public virtual void Add(string propertyName, string error)
        {
            Errors.Add(new ErrorInfo(propertyName, error));
        }

        public virtual void Add(ErrorInfo error)
        {
            Errors.Add(error);
        }

        public virtual void CopyErrorsToModelState(System.Web.Mvc.ModelStateDictionary ModelState)
        {
            if (Errors.Any())
            {
                foreach (var err in Errors)
                {
                    ModelState.AddModelError(err.PropertyName, err.ErrorMessage);
                }
            }
        }
    }

}
