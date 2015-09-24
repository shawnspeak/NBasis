using System.Web.Mvc;

namespace NBasis.Web.Mvc
{
    /// <summary>
    /// Indicate that the controller action should complete the unit of work
    /// </summary>
    public class CompleteWorkAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            // if no exceptions, then complete work
            if ((!filterContext.Canceled) && (filterContext.Exception == null))
            {
                // get the unit or work
                IUnitOfWork unitOfWork = WebContext.Current.Container.Resolve<IUnitOfWork>();
                unitOfWork.Complete();
            }
        }
    }
}
